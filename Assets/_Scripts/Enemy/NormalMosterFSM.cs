using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 일반 몬스터 공용 FSM
/// - 근접 / 원거리 / 복합 몬스터 대응
/// - 일반 공격과 특수 공격 분리
/// - 패턴은 MonsterData 기반으로 제어
/// 
/// 설계 의도:
/// - 상태 수를 최소화하여 안정적인 전투 흐름 유지
/// - 공격 중 중복 실행 방지를 위해 isActing 사용
/// - 이동은 NavMesh, 공격은 RootMotion 기반
/// </summary>
public class NormalMosterFSM : MonoBehaviour, IHitResponder
{
    /*───────────────────────────────*
     * 상태 정의
     *───────────────────────────────*/
    enum MonsterState
    {
        Idle,       // 대기 상태 (플레이어 탐색)
        Move,       // 추적 / 위치 조정
        Attack,     // 공격 수행 중
        GetHit,     // 피격 반응
        Die         // 사망
    }

    MonsterState state;

    /*───────────────────────────────*
     * 참조 컴포넌트
     *───────────────────────────────*/
    Transform target;            // 플레이어 타겟
    public MonsterData monsterData;
    NavMeshAgent agent;
    public Animator anim;
    RoomController roomController;

    /*───────────────────────────────*
     * 기본 스탯
     *───────────────────────────────*/
    public float currentHP;
    float speed;
    float def;

    /*───────────────────────────────*
     * 공격 쿨타임
     *───────────────────────────────*/
    float coolTime;          // 일반 공격 쿨타임
    float timer;

    float specialCoolTime;   // 특수 공격 쿨타임
    float specialTimer;

    /*───────────────────────────────*
     * 거리 관련 수치
     *───────────────────────────────*/
    float attRange;          // 공격 사거리
    float detRange;          // 인식 거리
    float minRangeRange;     // 최소 유지 거리

    /*───────────────────────────────*
     * 몬스터 분류
     *───────────────────────────────*/
    Type monsterType;        // 근접 / 원거리
    Race monsterRace;

    /*───────────────────────────────*
     * 패턴 데이터
     *───────────────────────────────*/
    NormalPattern[] normalPatterns;
    SpecialPattern[] specialPatterns;
    int[] normalPatternIDs;

    /*───────────────────────────────*
     * 공격 관련 데이터
     *───────────────────────────────*/
    float aoeRange;

    public GameObject projectilePrefab; // 원거리 투사체
    public GameObject laserPrefab;       // 레이저
    public GameObject aoePrefab;         // 범위 공격

    public Transform firePoint;          // 발사 위치

    /*───────────────────────────────*
     * FX (현재는 데이터만 참조)
     *───────────────────────────────*/
    GameObject attackFX;
    GameObject hitFX;
    GameObject deathFX;

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

        // 스탯 초기화
        currentHP = monsterData.HP;
        speed = monsterData.Speed;
        def = monsterData.Defense;

        // 쿨타임 초기화
        coolTime = monsterData.CoolTime;
        specialCoolTime = monsterData.specialCoolTime;

        timer = coolTime;
        specialTimer = specialCoolTime;

        // 거리 수치
        attRange = monsterData.attackRange;
        detRange = monsterData.detectionRange;
        minRangeRange = monsterData.minAttackRange;

        // 몬스터 분류
        monsterType = monsterData.Type;
        monsterRace = monsterData.Race;

        // 패턴 데이터
        normalPatterns = monsterData.NormalPatterns;
        specialPatterns = monsterData.SpecialPatterns;
        normalPatternIDs = monsterData.NormalpatternIDs;

        aoeRange = monsterData.aoeRange;

        // FX 참조
        attackFX = monsterData.attackFX;
        hitFX = monsterData.hitFX;
        deathFX = monsterData.deathFX;

        agent.speed = speed;
        agent.isStopped = false;
    }

    /*───────────────────────────────*
     * Update 루프
     *───────────────────────────────*/
    void Update()
    {
        // 공격 중이 아닐 때만 쿨타임 감소
        if (state == MonsterState.Idle || state == MonsterState.Move)
        {
            if (timer > 0) timer -= Time.deltaTime;
            if (specialTimer > 0) specialTimer -= Time.deltaTime;
        }

        switch (state)
        {
            case MonsterState.Idle: Idle(); break;
            case MonsterState.Move: Move(); break;
            case MonsterState.Attack: Attack(); break;
        }
    }

    /*───────────────────────────────*
     * Idle : 플레이어 탐색
     *───────────────────────────────*/
    void Idle()
    {
        anim.applyRootMotion = false;
        agent.isStopped = true;
        anim.SetBool("isMove", false);

        // 플레이어 탐색
        if (target == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p == null) return;
            target = p.transform;
        }

        // 인식 범위 내 진입 시 이동 상태로 전환
        if (Vector3.Distance(transform.position, target.position) <= detRange)
            state = MonsterState.Move;
    }

    /*───────────────────────────────*
     * Move : 추적 / 거리 조정
     *───────────────────────────────*/
    void Move()
    {
        anim.applyRootMotion = false;
        if (target == null) return;

        anim.SetBool("isMove", true);
        agent.isStopped = false;

        float distance = Vector3.Distance(transform.position, target.position);

        // 원거리 몬스터 로직
        if (monsterType == Type.Range)
        {
            if (distance < minRangeRange)
            {
                Vector3 awayDir = (transform.position - target.position).normalized;
                agent.SetDestination(transform.position + awayDir);
                return;
            }

            if (distance <= attRange)
            {
                state = MonsterState.Attack;
                return;
            }

            agent.SetDestination(target.position);
            return;
        }

        // 근접 몬스터 로직
        if (distance < minRangeRange)
        {
            Vector3 awayDir = (transform.position - target.position).normalized;
            agent.SetDestination(transform.position + awayDir);
            return;
        }

        if (distance > attRange)
        {
            agent.SetDestination(target.position);
            return;
        }

        agent.isStopped = true;
        anim.SetBool("isMove", false);
        state = MonsterState.Attack;
    }

    /*───────────────────────────────*
     * Attack : 공격 분기
     *───────────────────────────────*/
    bool isActing;

    void Attack()
    {
        if (isActing) return;

        isActing = true;
        agent.isStopped = true;
        anim.applyRootMotion = true;

        // 공격 시작 시 방향 고정
        FaceTargetOnce();

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
        state = MonsterState.Idle;
    }

    void FaceTargetOnce()
    {
        if (target == null) return;

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
        return timer <= 0f && normalPatternIDs != null && normalPatternIDs.Length > 0;
    }

    bool CanUseSpecial()
    {
        if (specialTimer > 0f || specialPatterns == null) return false;

        foreach (var sp in specialPatterns)
            if (sp == SpecialPattern.AOE || sp == SpecialPattern.Laser)
                return true;

        return false;
    }

    /*───────────────────────────────*
     * 일반 공격 처리
     *───────────────────────────────*/
    IEnumerator ExecuteNormalAttack()
    {
        int id = normalPatternIDs[Random.Range(0, normalPatternIDs.Length)];
        anim.SetInteger("Pattern", id);
        anim.SetTrigger("Attack");

        yield return null;

        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);

        if (monsterType == Type.Melee)
        {
            float total = info.length;

            // 근접 공격 타이밍 분리
            // - hitTime      : 공격 준비(텔레그래프)
            // - hitDuration  : 실제 판정 구간
            // - remainTime   : 후딜레이
            // 추후 히트박스/이펙트/사운드 확장을 고려한 구조
            float hitTime = total * 0.4f;

            float hitDuration = total * 0.2f;

            float remainTime = total - (hitTime + hitDuration); // == 40%

            yield return new WaitForSeconds(hitTime);

            yield return new WaitForSeconds(hitDuration);

            yield return new WaitForSeconds(remainTime);

        }
        else
        {
            yield return StartCoroutine(DoRangedAttack());
        }

        anim.applyRootMotion = false;
        timer = coolTime;
        isActing = false;
        state = MonsterState.Idle;
    }

    IEnumerator DoRangedAttack()
    {
        if (firePoint == null || projectilePrefab == null)
            yield break;

        Vector3 dir = (target.position - firePoint.position).normalized;

        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(info.length * 0.4f);

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        proj.transform.right = dir;
        proj.GetComponent<Projectile>()?.Launch();
    }

    /*───────────────────────────────*
     * 특수 공격 처리
     *───────────────────────────────*/
    IEnumerator ExecuteSpecialPattern()
    {
        anim.applyRootMotion = true;

        SpecialPattern sp = GetValidSpecialPattern();
        if (sp == default)
        {
            isActing = false;
            state = MonsterState.Idle;
            yield break;
        }

        anim.SetInteger("Pattern", (int)sp);
        anim.SetTrigger("Attack");

        yield return null;

        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(info.length * 0.4f);

        if (sp == SpecialPattern.AOE) Aoe();
        else if (sp == SpecialPattern.Laser) StartLaser();

        yield return new WaitForSeconds(info.length * 0.6f);

        anim.applyRootMotion = false;
        specialTimer = specialCoolTime;
        isActing = false;
        state = MonsterState.Idle;
    }

    SpecialPattern GetValidSpecialPattern()
    {
        List<SpecialPattern> valid = new List<SpecialPattern>();

        foreach (var sp in specialPatterns)
            valid.Add(sp);

        return valid.Count > 0 ? valid[Random.Range(0, valid.Count)] : default;
    }

    /*───────────────────────────────*
     * 특수 공격 구현
     *───────────────────────────────*/
    void Aoe()
    {
        Instantiate(aoePrefab, transform.position, Quaternion.identity);
    }

    void StartLaser()
    {
        if (laserPrefab == null || firePoint == null) return;
        Instantiate(laserPrefab, firePoint.position, firePoint.rotation);
    }

    /*───────────────────────────────*
     * 데미지 처리
     *───────────────────────────────*/
    public void TakeDamage(DamageData data)
    {
        if (state == MonsterState.Die) return;

        currentHP -= data.damageAmount;

        if (state == MonsterState.Attack)
        {
            if (currentHP <= 0) Die();
            return;
        }

        if (currentHP <= 0) Die();
        else StartCoroutine(GetHitProc());
    }

    IEnumerator GetHitProc()
    {
        state = MonsterState.GetHit;
        anim.applyRootMotion = true;
        anim.SetTrigger("Hit");

        yield return null;
        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);

        anim.applyRootMotion = false;
        state = MonsterState.Idle;
    }

    public void SetupRoom(RoomController room)
    {
        roomController = room;
    }

    /*───────────────────────────────*
     * 사망 처리
     *───────────────────────────────*/
    public void Die()
    {
        state = MonsterState.Die;
        anim.applyRootMotion = true;
        agent.isStopped = true;

        anim.SetTrigger("Die");
        StartCoroutine(DieProc());
    }

    IEnumerator DieProc()
    {
        DropItems();
        yield return new WaitForSeconds(3f);

        roomController?.ClearDungeon(gameObject);
        _MasterManager.Instance.DataManager.GetMonster(monsterData);
        Destroy(gameObject);
    }

    void DropItems()
    {
        if (monsterData.DropTable == null) return;

        foreach (var drop in monsterData.DropTable)
        {
            if (Random.value > drop.chance) continue;

            int count = Random.Range(drop.minCount, drop.maxCount + 1);
            for (int i = 0; i < count; i++)
                Instantiate(drop.itemPrefab, transform.position + GetRandomDropOffset(), Quaternion.identity);
        }
    }

    Vector3 GetRandomDropOffset()
    {
        Vector2 rand = Random.insideUnitCircle * 0.5f;
        return new Vector3(rand.x, 0f, rand.y);
    }

    public bool OnAttack()
    {
        return state == MonsterState.Attack;
    }

    /*───────────────────────────────*
     * Gizmos
     *───────────────────────────────*/
    void OnDrawGizmos()
    {
        if (monsterData == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, monsterData.attackRange);
    }
}


