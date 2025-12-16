using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 일반 몬스터용 FSM
/// - 근접 / 원거리 / 복합 몬스터 공용
/// - 일반 공격 / 특수 공격 분리
/// - 특수 패턴은 데이터 기반 + FSM 필터링
/// </summary>
public class NormalMosterFSM : MonoBehaviour
{
    /*───────────────────────────────*
     * 상태 정의
     *───────────────────────────────*/
    enum MonsterState
    {
        Idle,
        Move,
        Attack,
        GetHit,
        Die
    }

    MonsterState state;

    /*───────────────────────────────*
     * 참조
     *───────────────────────────────*/
    Transform target;
    public MonsterData monsterData;
    NavMeshAgent agent;
    public Animator anim;

    /*───────────────────────────────*
     * 기본 스탯
     *───────────────────────────────*/
    float currentHP;
    float att;
    float speed;
    float def;

    /*───────────────────────────────*
     * 공격 쿨타임
     *───────────────────────────────*/
    float coolTime;          // 일반 공격 쿨
    float timer = 0f;

    float specialCoolTime;   // 특수 공격 쿨
    float specialTimer = 0f;

    /*───────────────────────────────*
     * 거리 관련
     *───────────────────────────────*/
    float attRange;
    float detRange;
    float minRangeRange;

    /*───────────────────────────────*
     * 몬스터 분류
     *───────────────────────────────*/
    Type monsterType;
    Race monsterRace;

    /*───────────────────────────────*
     * 패턴 데이터
     *───────────────────────────────*/
    NormalPattern[] normalPatterns;
    SpecialPattern[] specialPatterns;

    int[] normalPatternIDs;

    /*───────────────────────────────*
     * 공통 패턴 파라미터
     *───────────────────────────────*/
    float windupTime;
    float recoveryTime;

    /*───────────────────────────────*
     * 근접 / 범위 공격
     *───────────────────────────────*/
    float meleeRadius;
    float meleeAngle;
    float aoeRange;
    float aoeDamageMultiplier;

    /*───────────────────────────────*
     * 원거리 공격
     *───────────────────────────────*/
    float projectileSpeed;
    int projectileCount;
    float shotInterval;
    int burstCount;
    public Transform firePoint;

    /*───────────────────────────────*
     * FX
     *───────────────────────────────*/
    GameObject attackFX;
    GameObject hitFX;
    GameObject deathFX;

    /*───────────────────────────────*
     * 히트박스
     *───────────────────────────────*/
    public GameObject[] atthitbox;

    public GameObject aoeHitbox;

    public void EnableHitbox()
    {
        foreach (var hb in atthitbox)
            hb.SetActive(true);
    }

    public void DisableHitbox()
    {
        foreach (var hb in atthitbox)
            hb.SetActive(false);
    }

    /*───────────────────────────────*
     * 초기화
     *───────────────────────────────*/
    void Start()
    {
        if (monsterData == null)
        {
            Debug.LogError("MonsterData가 할당되지 않았습니다.");
            return;
        }

        state = MonsterState.Idle;

        anim = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();

        // 기본 스탯
        currentHP = monsterData.HP;
        att = monsterData.Attack;
        speed = monsterData.Speed;
        def = monsterData.Defense;

        // 쿨타임
        coolTime = monsterData.CoolTime;
        specialCoolTime = monsterData.specialCoolTime;

        // 거리
        attRange = monsterData.attackRange;
        detRange = monsterData.detectionRange;
        minRangeRange = monsterData.minAttackRange;

        // 분류
        monsterType = monsterData.Type;
        monsterRace = monsterData.Race;

        // 패턴
        normalPatterns = monsterData.NormalPatterns;
        specialPatterns = monsterData.SpecialPatterns;
        normalPatternIDs = monsterData.NormalpatternIDs;

        // 패턴 파라미터
        windupTime = monsterData.windupTime;
        recoveryTime = monsterData.recoveryTime;

        // 근접 / 범위
        meleeRadius = monsterData.attackRadius;
        meleeAngle = monsterData.attackAngle;
        aoeRange = monsterData.aoeRange;
        aoeDamageMultiplier = monsterData.aoeDamageMultiplier;

        // 원거리
        projectileSpeed = monsterData.projectileSpeed;
        projectileCount = monsterData.projectileCount;
        shotInterval = monsterData.shotInterval;
        burstCount = monsterData.burstCount;

        // FX
        attackFX = monsterData.attackFX;
        hitFX = monsterData.hitFX;
        deathFX = monsterData.deathFX;

        agent.speed = speed;
        agent.isStopped = false;
    }

    /*───────────────────────────────*
     * Update
     *───────────────────────────────*/
    void Update()
    {
        // 쿨타임 감소
        if (timer > 0) timer -= Time.deltaTime;
        if (specialTimer > 0) specialTimer -= Time.deltaTime;

        switch (state)
        {
            case MonsterState.Idle: Idle(); break;
            case MonsterState.Move: Move(); break;
            case MonsterState.Attack: Attack(); break;
            case MonsterState.GetHit:   break;
            case MonsterState.Die: Die();  break;
        }
    }

    /*───────────────────────────────*
     * Idle
     *───────────────────────────────*/
    void Idle()
    {
        anim.applyRootMotion = false;

        anim.SetBool("isIdle", true);

        if (target == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) target = p.transform;
            else return;
        }

        float distance = Vector3.Distance(transform.position, target.position);
        if (distance <= detRange)
        {
            anim.SetBool("isIdle", false);
            state = MonsterState.Move;
        }
    }

    /*───────────────────────────────*
     * Move
     *───────────────────────────────*/
    void Move()
    {
        anim.applyRootMotion = false;

        if (target == null) return;

        anim.SetBool("isMove", true);
        agent.isStopped = false;

        float distance = Vector3.Distance(transform.position, target.position);

        //  원거리 몹: 최소거리 유지 로직
        if (monsterType == Type.Range)
        {
            // 너무 가까우면 뒤로 빠지기(도망 목적지)
            if (distance < minRangeRange)
            {
                Vector3 awayDir = (transform.position - target.position).normalized;
                Vector3 retreatPos = transform.position + awayDir * (minRangeRange - distance + 0.5f);

                agent.SetDestination(retreatPos);
                return; // 계속 Move 유지
            }

            // 적정거리(최소거리 이상 & 공격거리 이하)면 공격
            if (distance <= attRange)
            {
                anim.SetBool("isMove", false);
                agent.isStopped = true;
                state = MonsterState.Attack;
                return;
            }

            // 공격거리 밖이면 다가가기
            agent.SetDestination(target.position);
            return;
        }

        // 근접 몹: 기존대로
        agent.SetDestination(target.position);

        if (distance <= attRange)
        {
            anim.SetBool("isMove", false);
            agent.isStopped = true;
            state = MonsterState.Attack;
        }
    }

    /*───────────────────────────────*
     * Attack 분기
     *───────────────────────────────*/
    bool isActing;

    void Attack()
    {
        anim.applyRootMotion = true;

        agent.isStopped = true;
        if (isActing) return;

        if (CanUseSpecial())
        {
            isActing = true;
            StartCoroutine(ExecuteSpecialPattern());
            return;
        }

        if (CanUseNormal())
        {
            isActing = true;
            StartCoroutine(ExecuteNormalAttack());
            return;
        }

        state = MonsterState.Idle;
    }


    /*───────────────────────────────*
     * 공격 가능 여부 체크
     *───────────────────────────────*/
    bool CanUseNormal()
    {
        if (timer > 0f) return false;
        if (normalPatternIDs == null || normalPatternIDs.Length == 0) return false;
        return true;
    }

    bool CanUseSpecial()
    {
        if (specialTimer > 0f) return false;
        if (specialPatterns == null || specialPatterns.Length == 0) return false;

        foreach (var sp in specialPatterns)
        {
            if (sp == SpecialPattern.AOE && aoeRange > 0f)
                return true;

            if (sp == SpecialPattern.Laser )
                return true;
        }
        return false;
    }

    /*───────────────────────────────*
     * 일반 공격
     *───────────────────────────────*/
    IEnumerator ExecuteNormalAttack()
    {
        state = MonsterState.Attack;

        int id = normalPatternIDs[Random.Range(0, normalPatternIDs.Length)];
        anim.SetInteger("Pattern", id);
        anim.SetTrigger("Attack");

        // 애니메이터 상태 반영 대기
        yield return null;
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);

        if (monsterType == Type.Melee)
        {
            float total = info.length;

            float hitTime = total * 0.4f;
            float hitDuration = total * 0.2f;
            float remainTime = total - (hitTime + hitDuration); // == 40%

            yield return new WaitForSeconds(hitTime);

            EnableHitbox();
            yield return new WaitForSeconds(hitDuration);
            DisableHitbox();

            yield return new WaitForSeconds(remainTime);
        }
        else
        {
            yield return StartCoroutine(DoRangedAttack());
        }

        // RootMotion 종료 → NavMesh 복귀
        anim.applyRootMotion = false;

        // 후딜
        yield return new WaitForSeconds(recoveryTime);

        timer = coolTime;
        isActing = false;
        state = MonsterState.Idle;
    }

    IEnumerator DoRangedAttack()
    {
        if (firePoint == null)
        {
            Debug.LogWarning($"{name} : firePoint 없음 → 원거리 공격 스킵");
            yield break;
        }

        // 투사체 생성
    }


    /*───────────────────────────────*
     * 특수 공격
     *───────────────────────────────*/
    IEnumerator ExecuteSpecialPattern()
    {
        state = MonsterState.Attack;
        anim.applyRootMotion = true;

        SpecialPattern sp = GetValidSpecialPattern();
        if (sp == default)
        {
            state = MonsterState.Idle;
            yield break;
        }

        // 패턴 → 애니메이터 전달
        int id = (int)sp;
        anim.SetInteger("Pattern", id);
        anim.SetTrigger("Attack");

        // Animator 상태 반영 대기 (★ 중요)
        yield return null;

        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        float animLength = info.length;

        // ─────────────────────────
        //  준비 시간 (windup)
        // ─────────────────────────
        yield return new WaitForSeconds(windupTime);

        // ─────────────────────────
        //  특수 공격 로직 (짧게)
        // ─────────────────────────
        switch (sp)
        {
            case SpecialPattern.AOE:
                Aoe();          // 내부에서 잠깐 켜고 끄는 구조
                break;

            case SpecialPattern.Laser:
                StartLaser();   // 레이저 FSM / 프리팹 쪽에서 처리
                break;
        }

        // ─────────────────────────
        // 3️⃣ 애니메이션 종료까지 대기
        // (이미 지난 windup 제외)
        // ─────────────────────────
        float remainTime = Mathf.Max(0f, animLength - windupTime);
        yield return new WaitForSeconds(remainTime);

        // ─────────────────────────
        // 4️⃣ 종료 처리
        // ─────────────────────────
        anim.applyRootMotion = false;

        specialTimer = specialCoolTime;
        isActing = false;
        state = MonsterState.Idle;
    }


    SpecialPattern GetValidSpecialPattern()
    {
        List<SpecialPattern> valid = new List<SpecialPattern>();

        foreach (var sp in specialPatterns)
        {
            if (sp == SpecialPattern.AOE && aoeRange > 0f)
                valid.Add(sp);

            if (sp == SpecialPattern.Laser)
                valid.Add(sp);
        }

        if (valid.Count == 0) return default;
        return valid[Random.Range(0, valid.Count)];
    }

    /*───────────────────────────────*
     * 특수 공격 구현부 (비어 있음)
     *───────────────────────────────*/
    void Aoe()
    {
        if (aoeHitbox == null) return;

        aoeHitbox.SetActive(true);
        StartCoroutine(DisableAoeAfterTime(0.3f));
    }

    IEnumerator DisableAoeAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        aoeHitbox.SetActive(false);
    }


    void StartLaser()
    {
        // TODO: 레이저 프리팹/FSM
    }

    /*───────────────────────────────*
     * 데미지 / 피격
     *───────────────────────────────*/
    public void TakeDamage(DamageData data)
    {
        if (state == MonsterState.Die) return;

        currentHP -= data.damageAmount;

        if (state == MonsterState.Attack)
        {
            // 공격은 계속, HP만 감소
            if (currentHP <= 0)
                state = MonsterState.Die;

            return;
        }
        else
            StartCoroutine(GetHitProc());
    }

    IEnumerator GetHitProc()
    {
        state = MonsterState.GetHit;
        anim.applyRootMotion = true; 

        anim.SetTrigger("Hit");

        yield return null;

        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(info.length);

        //  RootMotion 종료 → NavMesh로 복귀
        anim.applyRootMotion = false;

        state = MonsterState.Idle;
    }

    /*───────────────────────────────*
     * 사망
     *───────────────────────────────*/
    void Die()
    {
        anim.applyRootMotion = true;

        anim.SetTrigger("Die");
        StartCoroutine(DieProc());
    }

    IEnumerator DieProc()
    {
        yield return new WaitForSeconds(2f);

        // 나중에 연결
        // DropItem(transform.position, monsterData);

        Destroy(gameObject);
    }


    /*───────────────────────────────*
     * Gizmos
     *───────────────────────────────*/
    private void OnDrawGizmos()
    {
        if (monsterData == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, monsterData.attackRange);

        if (monsterData.minAttackRange > 0f)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, monsterData.minAttackRange);
        }

        if (monsterData.aoeRange > 0f)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, monsterData.aoeRange);
        }
    }
}

