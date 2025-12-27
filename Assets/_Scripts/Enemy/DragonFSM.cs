using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

/// <summary>
/// 드래곤 보스 FSM
/// - 일반 공격 3종 (근접 / 꼬리 / 브레스 등 랜덤)
/// - 특수 공격
///   1) 돌진 : 사거리 밖 + 전용 쿨
///   2) 포효 : AOE
///   3) 점프 스매시 : 광역 착지 공격 (긴 쿨)
/// 
/// ✔ NavMesh 유지
/// ✔ 히트 판정은 OnAttack 기반
/// </summary>
public class DragonFSM : MonoBehaviour ,IHitResponder , IHPProvider
{
    enum DragonState
    {
        Idle,
        Move,
        Attack,
        GetHit,
        Die
    }

    DragonState state;

    /*───────────────────────────────*
     * 참조
     *───────────────────────────────*/
    Transform target;
    public MonsterData dragonData;
    NavMeshAgent agent;
    public Animator anim;
    RoomController roomController;
    [Header("HitBox")]
    public GameObject chargeHitBox;
    public GameObject jumpAoePrefab;


    /*───────────────────────────────*
     * 스탯
     *───────────────────────────────*/
    public float currentHP;
    float speed;

    //경직
    public int hitThreshold = 5;
    int hitCount = 0;

    /*───────────────────────────────*
     * 쿨타임
     *───────────────────────────────*/
    float normalCool;
    float normalTimer;

    float chargeCool;
    float chargeTimer;

    float specialCool;
    float specialTimer;

    /*───────────────────────────────*
     * 거리
     *───────────────────────────────*/
    float attRange;
    float detRange;
    float minRange;

    /*───────────────────────────────*
     * 패턴 데이터
     *───────────────────────────────*/
    NormalPattern[] normalPatterns;
    SpecialPattern[] specialPatterns;

    int[] normalPatternIDs;


    /*───────────────────────────────*
     * 돌진
     *───────────────────────────────*/
    bool isCharging;

    bool isActing;

    public float CurrentHP => currentHP;
    public float MaxHP => dragonData.HP;
    public bool IsAlive => state != DragonState.Die;



    /*───────────────────────────────*
     * 초기화
     *───────────────────────────────*/
    void Start()
    {
        if (dragonData == null) return;
        state = DragonState.Idle;

        anim = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();

        //기본스텟
        currentHP = dragonData.HP;
        speed = dragonData.Speed;

        //쿨
        normalCool = dragonData.CoolTime;
        chargeCool = dragonData.patternCooldown;
        specialCool = dragonData.specialCoolTime;

        //쿨타임
        normalTimer = normalCool;
        chargeTimer = chargeCool;
        specialTimer = specialCool;

        //거리
        attRange = dragonData.attackRange;
        detRange = dragonData.detectionRange;
        minRange = dragonData.minAttackRange;

        // 패턴
        normalPatterns = dragonData.NormalPatterns;
        specialPatterns = dragonData.SpecialPatterns;
        normalPatternIDs = dragonData.NormalpatternIDs;

        agent.speed = speed;


        if (chargeHitBox) chargeHitBox.SetActive(false);
    }

    void Update()
    {
        // 쿨타임 감소
        if (state == DragonState.Idle || state == DragonState.Move)
        {
            if (normalTimer > 0) normalTimer -= Time.deltaTime;
            if (chargeTimer > 0) chargeTimer -= Time.deltaTime;
            if (specialTimer > 0) specialTimer -= Time.deltaTime;
        }

        switch (state)
        {
            case DragonState.Idle: Idle(); break;
            case DragonState.Move: Move(); break;
            case DragonState.Attack: Attack(); break;
        }
    }

    /*───────────────────────────────*
     * Idle / Move
     *───────────────────────────────*/
    void Idle()
    {
        anim.applyRootMotion = false;

        agent.isStopped = true;
        anim.SetBool("isMove", false);

        if (target == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p == null) return;
            target = p.transform;
        }

        if (Vector3.Distance(transform.position, target.position) <= detRange)
            state = DragonState.Move;
    }

    void Move()
    {
        anim.applyRootMotion = false;
        anim.SetBool("isMove", true);
        agent.isStopped = false;

        float dist = Vector3.Distance(transform.position, target.position);

        if (dist < minRange)
        {
            // 너무 붙어 있음 → 살짝만 거리 벌리기
            Vector3 awayDir = (transform.position - target.position).normalized;
            Vector3 adjustPos = transform.position + awayDir * 1f;

            agent.SetDestination(adjustPos);
            return;
        }

        // 공격 사거리 밖 → 접근
        if (dist > attRange)
        {
            agent.SetDestination(target.position);
            return;
        }

        // 딱 공격 가능한 거리
        anim.SetBool("isMove", false);
        agent.isStopped = true;
        state = DragonState.Attack;
    }

    /*───────────────────────────────*
     * Attack
     *───────────────────────────────*/
    void Attack()
    {
        if (isActing) return;

        isActing = true;
        agent.isStopped = true;
        anim.applyRootMotion = true;

        // ⭐ 공격 시작 시 방향 고정
        FaceTargetOnce();

        float dist = Vector3.Distance(transform.position, target.position);

        // 🔥 돌진 조건
        if (dist > attRange && chargeTimer <= 0f)
        {
            StartCoroutine(ChargeAttack());
            return;
        }

        if (CanUseSpecial())
        {
            StartCoroutine(ExecuteSpecialPattern());
            return;
        }

        if (CanUseNormal())
        {
            StartCoroutine(ExecuteNormalAttack());
            return;
        }

        isActing = false;
        state = DragonState.Idle;
    }

    void FaceTargetOnce()
    {
        Vector3 dir = target.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(dir);
    }

    /*───────────────────────────────*
     * 공격 가능 여부 체크
     *───────────────────────────────*/
    bool CanUseNormal()
    {
        if (normalTimer > 0f) return false;
        if (normalPatternIDs == null || normalPatternIDs.Length == 0) return false;
        return true;
    }

    bool CanUseSpecial()
    {
        if (specialTimer > 0f) return false;
        if (specialPatterns == null || specialPatterns.Length == 0) return false;

        foreach (var sp in specialPatterns)
        {
            if (sp == SpecialPattern.Roar)
                return true;

            if (sp == SpecialPattern.JumpSmash)
                return true;
        }
        return false;
    }

    /*───────────────────────────────*
     * 일반 공격
     *───────────────────────────────*/
    IEnumerator ExecuteNormalAttack()
    {

        int id = normalPatternIDs[Random.Range(0, normalPatternIDs.Length)];
        anim.SetInteger("Pattern", id);
        anim.SetTrigger("Attack");

        // 애니메이터 상태 반영 대기
        yield return null;
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);

        
        float total = info.length;

        float hitTime = total * 0.4f;
        float hitDuration = total * 0.2f;
        float remainTime = total - (hitTime + hitDuration); // == 40%

        yield return new WaitForSeconds(hitTime);

        yield return new WaitForSeconds(hitDuration);

        yield return new WaitForSeconds(remainTime);
        
        

        // RootMotion 종료 → NavMesh 복귀
        anim.applyRootMotion = false;

        normalTimer = normalCool;
        isActing = false;
        state = DragonState.Idle;
    }

    /*───────────────────────────────*
     * 특수 공격 (점프)
     *───────────────────────────────*/
    IEnumerator ExecuteSpecialPattern()
    {
        anim.applyRootMotion = true;

        SpecialPattern sp = GetValidSpecialPattern();
        if (sp == default)
        {
            isActing = false;
            state = DragonState.Idle;
            yield break;
        }


        // 패턴 전달
        anim.SetInteger("Pattern", (int)sp);
        anim.SetTrigger("Attack");
        print((int)sp);

        // Animator 반영 대기
        yield return null;

        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        float total = info.length;
        float hitTime = total * 0.4f;

        // 준비 구간
        yield return new WaitForSeconds(hitTime);

        // 특수 공격 발동
        switch (sp)
        {
            case SpecialPattern.JumpSmash:
                Jump();
                break;
        }

        // 애니메이션 종료까지 대기
        yield return new WaitForSeconds(total - hitTime);

        // 종료 처리
        anim.applyRootMotion = false;

        specialTimer = specialCool;
        isActing = false;
        state = DragonState.Idle;
    }

    SpecialPattern GetValidSpecialPattern()
    {
        List<SpecialPattern> valid = new List<SpecialPattern>();

        foreach (var sp in specialPatterns)
        {
            if (sp == SpecialPattern.JumpSmash)
                valid.Add(sp);
        }

        if (valid.Count == 0) return default;
        return valid[Random.Range(0, valid.Count)];
    }

    /*───────────────────────────────*
     * 특수 공격 구현부 
     *───────────────────────────────*/
    void Jump()
    {
        StartCoroutine(SpawnJumpAoeAfterDelay());
    }

    IEnumerator SpawnJumpAoeAfterDelay()
    {
        yield return new WaitForSeconds(1.12f); // 애니 기반 값
        SpawnJumpAoe();
    }

    void SpawnJumpAoe()
    {
        if (jumpAoePrefab == null)
            return;

        Vector3 pos = transform.position;
        pos.y = 0f;

        GameObject Aoe = Instantiate(jumpAoePrefab, pos, Quaternion.identity);
        DamageDealer jump = jumpAoePrefab.GetComponent<DamageDealer>();

        if (jump != null)
        {
            jump.SetOwner(this.gameObject);

            jump.SetDamage(dragonData.Attack);
        }
    }


    IEnumerator ChargeAttack()
    {
        isCharging = true;
        isActing = true;

        // 🔒 NavMesh 완전 차단
        agent.isStopped = true;
        agent.updatePosition = false;
        agent.updateRotation = false;
        anim.applyRootMotion = false;

        // 돌진 애니
        anim.SetInteger("Pattern", 401);
        anim.SetTrigger("Attack");

        // 텔레그래프
        yield return new WaitForSeconds(0.3f);

        // Animator 반영 대기
        yield return null;

        // 🔥 애니메이션 길이 가져오기
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        float chargeDuration = info.length;
        if (chargeDuration <= 0f)
            chargeDuration = 0.8f; // fallback

        Vector3 chargeDir = transform.forward;

        float chargeDistance = dragonData.chargeDistance;
        float chargeSpeed = chargeDistance / chargeDuration;

        float elapsed = 0f;

        if (chargeHitBox) chargeHitBox.SetActive(true);

        Vector3 rayOriginOffset = Vector3.up * 0.5f;
        float rayExtra = 0.2f;

        while (elapsed < chargeDuration)
        {
            float step = chargeSpeed * Time.deltaTime;

            bool blocked = Physics.Raycast(
                transform.position + rayOriginOffset,
                chargeDir,
                step + rayExtra,
                LayerMask.GetMask("Wall"),
                QueryTriggerInteraction.Ignore
            );

            if (!blocked)
            {
                transform.position += chargeDir * step;
            }
            // ❗ 막혀도 시간은 흐른다 (애니와 동기화)

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 돌진 루프 끝난 직후
        if (chargeHitBox) chargeHitBox.SetActive(false);

        // 🔥 이 한 줄이 핵심
        agent.Warp(transform.position);

        // NavMesh 복구
        agent.updatePosition = true;
        agent.updateRotation = true;
        agent.isStopped = false;

        chargeTimer = chargeCool;
        isCharging = false;
        isActing = false;
        state = DragonState.Idle;
    }




    /*───────────────────────────────*
     * 데미지 / 피격
     *───────────────────────────────*/
    public void TakeDamage(DamageData data)
    {
        if (state == DragonState.Die) return;

        currentHP -= data.damageAmount;
        print($"최대 {dragonData.HP}/현재 {currentHP}");

        // 사망 체크
        if (currentHP <= 0)
        {
            state = DragonState.Die;
            Die();
            return;
        }
        if (state == DragonState.Die) return;
        // 공격 중이면 경직 누적만
        if (state == DragonState.Attack || isCharging)
        {
            hitCount++;
            return;
        }

        // 이미 피격 상태면 무시
        if (state == DragonState.GetHit)
            return;

        // 🔥 피격 누적
        hitCount++;

        // 아직 임계치 미만 → 애니 없이 끝
        if (hitCount < hitThreshold)
            return;

        // 임계치 도달 → 피격 애니
        hitCount = 0;
        StartCoroutine(GetHitProc());
    }


    IEnumerator GetHitProc()
    {
        state = DragonState.GetHit;
        anim.applyRootMotion = true;

        anim.SetTrigger("Hit");

        yield return null;

        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(info.length);

        //  RootMotion 종료 → NavMesh로 복귀
        anim.applyRootMotion = false;

        state = DragonState.Idle;
    }

    public void SetupRoom(RoomController room)
    {
        roomController = room;
    }


    /*───────────────────────────────*
     * 사망
     *───────────────────────────────*/
    public void Die()
    {
        anim.applyRootMotion = true;
        agent.isStopped = true;

        anim.SetTrigger("Die");
        StartCoroutine(DieProc());
    }

    IEnumerator DieProc()
    {
        DropItems();

        yield return new WaitForSeconds(3f);

        if (roomController != null)
        {
            roomController.ClearDungeon(this.gameObject);
        }

        // 나중에 연결

        _MasterManager.Instance.DataManager.GetMonster(dragonData);
        Destroy(gameObject);
    }

    void DropItems()
    {
        if (dragonData.DropTable == null || dragonData.DropTable.Length == 0)
            return;

        foreach (var drop in dragonData.DropTable)
        {
            // 1️⃣ 확률 체크
            float roll = Random.value; // 0.0 ~ 1.0
            if (roll > drop.chance)
                continue;

            // 2️⃣ 드랍 개수 결정
            int count = Random.Range(drop.minCount, drop.maxCount + 1);
            if (count <= 0)
                continue;

            // 3️⃣ 아이템 생성
            for (int i = 0; i < count; i++)
            {
                Vector3 spawnPos = transform.position + GetRandomDropOffset();
                Instantiate(drop.itemPrefab, spawnPos, Quaternion.identity);
            }
        }
    }
    Vector3 GetRandomDropOffset()
    {
        float radius = 0.5f;
        Vector2 rand = Random.insideUnitCircle * radius;
        return new Vector3(rand.x, 0f, rand.y);
    }


    /*───────────────────────────────*
     * 히트 판정용
     *───────────────────────────────*/
    public bool OnAttack()
    {
        return state == DragonState.Attack || isCharging;
    }
    /*───────────────────────────────*
     * Gizmos
     *───────────────────────────────*/
    private void OnDrawGizmos()
    {
        if (dragonData == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, dragonData.attackRange);

        if (dragonData.minAttackRange > 0f)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, dragonData.minAttackRange);
        }

        if (dragonData.aoeRange > 0f)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, dragonData.aoeRange);
        }
    }
}

