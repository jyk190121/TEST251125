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
public class DragonFSM : MonoBehaviour ,IHitResponder
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
    public GameObject jumpAoeHitBox;
    public GameObject roarAoeHitBox;

    /*───────────────────────────────*
     * 스탯
     *───────────────────────────────*/
    float currentHP;
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
    Vector3 chargeTarget;
    float chargeDuration = 1.0f;
    float chargeSpeedMul = 3f;
    float originalSpeed;
    bool isCharging;

    bool isActing;

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
        originalSpeed = speed;

        agent.isStopped = false;

        if (chargeHitBox) chargeHitBox.SetActive(false);
        if (jumpAoeHitBox) jumpAoeHitBox.SetActive(false);
        if (roarAoeHitBox) roarAoeHitBox.SetActive(false);
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
     * 특수 공격 (포효 / 점프)
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
            case SpecialPattern.Roar:
                Roar();
                break;
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
            if (sp == SpecialPattern.Roar)
                valid.Add(sp);

            if (sp == SpecialPattern.JumpSmash)
                valid.Add(sp);
        }

        if (valid.Count == 0) return default;
        return valid[Random.Range(0, valid.Count)];
    }

    /*───────────────────────────────*
     * 특수 공격 구현부 (비어 있음)
     *───────────────────────────────*/
    void Roar()
    {
        if (roarAoeHitBox == null) return;

        roarAoeHitBox.SetActive(true);
        StartCoroutine(DisableRoarAoeAfterTime(0.5f));
    }

    IEnumerator DisableRoarAoeAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        roarAoeHitBox.SetActive(false);
    }


    void Jump()
    {
        if (jumpAoeHitBox == null) return;

        jumpAoeHitBox.SetActive(true);
        StartCoroutine(DisableJumpAoeAfterTime(0.5f));
    }

    IEnumerator DisableJumpAoeAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        jumpAoeHitBox.SetActive(false);
    }

    IEnumerator ChargeAttack()
    {
        isActing = true;
        isCharging = true;

        agent.isStopped = true;
        anim.applyRootMotion = true;

        // 방향 고정
        FaceTargetOnce();

        anim.SetTrigger("Attack");
        anim.SetInteger("Pattern", 401);

        yield return null;
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);

        float total = info.length;
        if (total <= 0f) total = chargeDuration; // 애니 없을 때 fallback

        float chargeStart = total * 0.3f;
        float chargeEnd = total * 0.85f;

        // 준비 동작
        yield return new WaitForSeconds(chargeStart);

        // === 돌진 시작 ===
        Vector3 dir = transform.forward;
        Vector3 chargeTarget = transform.position + dir * dragonData.chargeDistance;

        agent.speed = originalSpeed * chargeSpeedMul;
        agent.isStopped = false;
        agent.SetDestination(chargeTarget);

        if (chargeHitBox) chargeHitBox.SetActive(true);

        // 돌진 유지
        yield return new WaitForSeconds(chargeEnd - chargeStart);

        // === 돌진 종료 ===
        agent.isStopped = true;
        agent.speed = originalSpeed;

        if (chargeHitBox) chargeHitBox.SetActive(false);

        // 애니메이션 마무리
        yield return new WaitForSeconds(total - chargeEnd);

        anim.applyRootMotion = false;

        isCharging = false;
        chargeTimer = chargeCool;
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

