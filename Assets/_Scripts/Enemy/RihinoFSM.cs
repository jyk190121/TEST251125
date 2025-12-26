using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 코뿔소 보스 전용 FSM
/// - 기본 근접 공격
/// - 특수 공격 : 포효 / 돌진
/// 
/// ✔ 돌진 시작 시 방향 고정
/// ✔ NavMesh 기반 이동
/// ✔ 돌진 중 히트박스 On/Off
/// ✔ 애니메이션 길이 기준 타이밍
/// </summary>
public class RihinoFSM : MonoBehaviour ,IHitResponder, IHPProvider
{
    enum RihinoState
    {
        Idle,
        Move,
        Attack,
        GetHit,
        Die
    }

    RihinoState state;

    /*───────────────────────────────*
     * 참조
     *───────────────────────────────*/
    Transform target;
    public MonsterData rihinoData;
    NavMeshAgent agent;
    public Animator anim;
    RoomController roomController;

    [Header("HitBox")]
    public GameObject chargeHitbox;

    /*───────────────────────────────*
     * 스탯
     *───────────────────────────────*/
    float currentHP;
    float speed;

    /*───────────────────────────────*
     * 쿨타임
     *───────────────────────────────*/
    float normalCool;
    float normalTimer;

    float specialCool;
    float specialTimer;

    float chargeCool;
    float chargeTimer;

    /*───────────────────────────────*
     * 거리
     *───────────────────────────────*/
    float attRange;
    float detRange;
    float minRange;

    /*───────────────────────────────*
     * 패턴
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
    public float MaxHP => rihinoData.HP;
    public bool IsAlive => state != RihinoState.Die;


    /*───────────────────────────────*
     * 초기화
     *───────────────────────────────*/
    void Start()
    {
        if (rihinoData == null) return;
        state = RihinoState.Idle;

        anim = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();

        // 스탯
        currentHP = rihinoData.HP;
        speed = rihinoData.Speed;

        // 쿨타임
        normalCool = rihinoData.CoolTime;
        specialCool = rihinoData.specialCoolTime;
        chargeCool = rihinoData.patternCooldown;  

        normalTimer = normalCool;
        specialTimer = specialCool;
        chargeTimer = chargeCool;

        // 거리
        attRange = rihinoData.attackRange;
        detRange = rihinoData.detectionRange;
        minRange = rihinoData.minAttackRange;

        // 패턴
        normalPatterns = rihinoData.NormalPatterns;
        specialPatterns = rihinoData.SpecialPatterns;
        normalPatternIDs = rihinoData.NormalpatternIDs;

        agent.speed = speed;
    }

    void Update()
    {
        if (state == RihinoState.Idle || state == RihinoState.Move)
        {
            if (normalTimer > 0f) normalTimer -= Time.deltaTime;
            if (specialTimer > 0f) specialTimer -= Time.deltaTime;
            if (chargeTimer > 0f) chargeTimer -= Time.deltaTime;
        }

        switch (state)
        {
            case RihinoState.Idle: Idle(); break;
            case RihinoState.Move: Move(); break;
            case RihinoState.Attack: Attack(); break;
        }
    }

    /*───────────────────────────────*
     * Idle
     *───────────────────────────────*/
    void Idle()
    {
        anim.applyRootMotion = false;
        //agent.isStopped = true;
        anim.SetBool("isMove", false);

        if (target == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p == null) return;
            target = p.transform;
        }

        if (Vector3.Distance(transform.position, target.position) <= detRange)
            state = RihinoState.Move;
    }

    /*───────────────────────────────*
     * Move
     *───────────────────────────────*/
    void Move()
    {
        anim.applyRootMotion = false;
        anim.SetBool("isMove", true);
        agent.isStopped = false;

        float dist = Vector3.Distance(transform.position, target.position);

        if (dist < minRange)
        {
            Vector3 away = (transform.position - target.position).normalized;
            agent.SetDestination(transform.position + away);
            return;
        }

        if (dist > attRange)
        {
            agent.SetDestination(target.position);
            return;
        }

        agent.isStopped = true;
        anim.SetBool("isMove", false);
        state = RihinoState.Attack;
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

        FaceTargetOnce();

        float dist = Vector3.Distance(transform.position, target.position);

        // 🔥 돌진 조건
        if (dist > attRange && chargeTimer <= 0f)
        {
            StartCoroutine(ChargeAttack());
            return;
        }

        // 
        if (CanUseSpecial())
        {
            StartCoroutine(ExecuteSpecialPattern());
            return;
        }

        // 🔥 일반 공격
        if (CanUseNormal())
        {
            StartCoroutine(ExecuteNormalAttack());
            return;
        }

        isActing = false;
        state = RihinoState.Idle;
    }

    void FaceTargetOnce()
    {
        Vector3 dir = target.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(dir);
    }

    /*───────────────────────────────*
     * 공격 가능 체크
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
            if (sp == SpecialPattern.Roar)
                return true;

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

        yield return null;
        float total = anim.GetCurrentAnimatorStateInfo(0).length;
        if (total <= 0f) total = 2f;

        yield return new WaitForSeconds(total);

        anim.applyRootMotion = false;
        normalTimer = normalCool;
        isActing = false;
        state = RihinoState.Idle;
    }

    /*───────────────────────────────*
     * 포효
     *───────────────────────────────*/
    IEnumerator ExecuteSpecialPattern()
    {
        anim.applyRootMotion = true;

        SpecialPattern sp = GetValidSpecialPattern();
        if (sp == default)
        {
            isActing = false;
            state = RihinoState.Idle;
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
            case SpecialPattern.Charge:
                ChargeAttack();
                break;
            
        }

        // 애니메이션 종료까지 대기
        yield return new WaitForSeconds(total - hitTime);

        // 종료 처리
        anim.applyRootMotion = false;

        specialTimer = specialCool;
        isActing = false;
        state = RihinoState.Idle;
    }
    SpecialPattern GetValidSpecialPattern()
    {
        List<SpecialPattern> valid = new List<SpecialPattern>();

        foreach (var sp in specialPatterns)
        {
            if (sp == SpecialPattern.Charge)
                valid.Add(sp);
        }

        if (valid.Count == 0) return default;
        return valid[Random.Range(0, valid.Count)];
    }
    /*───────────────────────────────*
     * 돌진
     *───────────────────────────────*/
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

        float chargeDistance = rihinoData.chargeDistance;
        float chargeSpeed = chargeDistance / chargeDuration;

        float elapsed = 0f;

        if (chargeHitbox) chargeHitbox.SetActive(true);

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

        if (chargeHitbox) chargeHitbox.SetActive(false);

        // 🔥 이 한 줄이 핵심
        agent.Warp(transform.position);

        // 🔓 NavMesh 복구
        agent.updatePosition = true;
        agent.updateRotation = true;
        agent.isStopped = false;

        chargeTimer = chargeCool;
        isCharging = false;
        isActing = false;
        state = RihinoState.Idle;
    }



    /*───────────────────────────────*
     * 데미지 / 피격
     *───────────────────────────────*/
    public void TakeDamage(DamageData data)
    {
        if (state == RihinoState.Die) return;

        currentHP -= data.damageAmount;
        print($"최대 {rihinoData.HP}/현재 {currentHP}");

        if (state == RihinoState.GetHit) return;
        // 🔥 공격 중이면 피격 연출 없이 HP만 감소
        if (state == RihinoState.Attack)
        {
            if (currentHP <= 0)
            {
                state = RihinoState.Die;
                Die();
            }
            return;
        }

        // 공격 중이 아닐 때만 피격 처리
        if (currentHP <= 0)
        {
            state = RihinoState.Die;
            Die();
        }
        else
        {
            StartCoroutine(GetHitProc());
        }

    }

    IEnumerator GetHitProc()
    {
        state = RihinoState.GetHit;
        anim.applyRootMotion = true;

        anim.SetTrigger("Hit");

        yield return null;

        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(info.length);

        //  RootMotion 종료 → NavMesh로 복귀
        anim.applyRootMotion = false;

        state = RihinoState.Idle;
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

        _MasterManager.Instance.DataManager.GetMonster(rihinoData);
        Destroy(gameObject);
    }

    void DropItems()
    {
        if (rihinoData.DropTable == null || rihinoData.DropTable.Length == 0)
            return;

        foreach (var drop in rihinoData.DropTable)
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
     * 외부 히트 체크용
     *───────────────────────────────*/
    public bool OnAttack()
    {
        return state == RihinoState.Attack || isCharging;
    }

    /*───────────────────────────────*
     * Gizmos
     *───────────────────────────────*/
    private void OnDrawGizmos()
    {
        if (rihinoData == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rihinoData.attackRange);

        if (rihinoData.minAttackRange > 0f)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, rihinoData.minAttackRange);
        }

        if (rihinoData.aoeRange > 0f)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, rihinoData.aoeRange);
        }
    }
}

