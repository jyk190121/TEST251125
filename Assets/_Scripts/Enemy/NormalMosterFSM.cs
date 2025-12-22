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
public class NormalMosterFSM : MonoBehaviour ,IHitResponder
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
     * 근접 / 범위 공격
     *───────────────────────────────*/
    float aoeRange;

    //원거리 투사체 레이저 프리팹
    GameObject projectilePrefab;
    GameObject laserPrefab;

    /*───────────────────────────────*
     * 원거리 공격
     *───────────────────────────────*/
    public Transform firePoint;

    /*───────────────────────────────*
     * FX
     *───────────────────────────────*/
    GameObject attackFX;
    GameObject hitFX;
    GameObject deathFX;

    public GameObject aoeHitbox;

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
        speed = monsterData.Speed;
        def = monsterData.Defense;

        // 쿨타임
        coolTime = monsterData.CoolTime;
        specialCoolTime = monsterData.specialCoolTime;

        timer = coolTime;
        specialTimer = specialCoolTime;

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

        // 근접 / 범위
        aoeRange = monsterData.aoeRange;

        //원거리 투사체 프리팹
        projectilePrefab = monsterData.projectilePrefab;
        //원거리 레이저 프리팹
        laserPrefab = monsterData.laserPrefab;

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
        if (state == MonsterState.Idle || state == MonsterState.Move)
        {
            if (timer > 0) timer -= Time.deltaTime;
            if (specialTimer > 0) specialTimer -= Time.deltaTime;
        }


        switch (state)
        {
            case MonsterState.Idle: Idle(); break;
            case MonsterState.Move: Move(); break;
            case MonsterState.Attack: Attack();  break;
            case MonsterState.GetHit: break;
            case MonsterState.Die: break;
        }
    }

    /*───────────────────────────────*
     * Idle
     *───────────────────────────────*/
    void Idle()
    {
        anim.applyRootMotion = false;

        agent.isStopped = true;
        anim.SetBool("isMove", false);

        if (target == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) target = p.transform;
            else return;
        }

        float distance = Vector3.Distance(transform.position, target.position);
        if (distance <= detRange)
        {
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

        /*───────────────────────────────*
         * 원거리 몹
         *───────────────────────────────*/
        if (monsterType == Type.Range)
        {
            if (distance < minRangeRange)
            {
                Vector3 awayDir = (transform.position - target.position).normalized;
                Vector3 retreatPos = transform.position + awayDir * (minRangeRange - distance + 0.5f);

                agent.SetDestination(retreatPos);
                return;
            }

            if (distance <= attRange)
            {
                anim.SetBool("isMove", false);
                agent.isStopped = true;
                state = MonsterState.Attack;
                return;
            }

            agent.SetDestination(target.position);
            return;
        }

        /*───────────────────────────────*
         * 근접 몹
         *───────────────────────────────*/
        if (distance < minRangeRange)
        {
            // 너무 붙어 있음 → 살짝만 거리 벌리기
            Vector3 awayDir = (transform.position - target.position).normalized;
            Vector3 adjustPos = transform.position + awayDir * 1f;

            agent.SetDestination(adjustPos);
            return;
        }

        // 공격 사거리 밖 → 접근
        if (distance > attRange)
        {
            agent.SetDestination(target.position);
            return;
        }

        // 딱 공격 가능한 거리
        anim.SetBool("isMove", false);
        agent.isStopped = true;
        state = MonsterState.Attack;
    }


    /*───────────────────────────────*
     * Attack 분기
     *───────────────────────────────*/
    bool isActing;

    void Attack()
    {
        if (isActing) return;

        isActing = true;
        agent.isStopped = true;
        anim.applyRootMotion = true;

        // ⭐ 공격 시작 시 방향 고정
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

        if (dir.sqrMagnitude < 0.001f) return;

        transform.rotation = Quaternion.LookRotation(dir);
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

            yield return new WaitForSeconds(hitDuration);

            yield return new WaitForSeconds(remainTime);
        }
        else
        {
            yield return StartCoroutine(DoRangedAttack());
        }

        // RootMotion 종료 → NavMesh 복귀
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
        float total = info.length;
        float hitTime = total * 0.4f;
        float remainTime = total - hitTime;

        // 🔹 발사 타이밍 대기
        yield return new WaitForSeconds(hitTime);

        // 🔹 생성
        GameObject proj = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.identity
        );

        // 🔹 즉시 방향 고정
        proj.transform.right = dir;

        // 🔹 즉시 발사
        proj.GetComponent<Projectile>()?.Launch();

        // 🔹 애니메이션 잔여 시간
        yield return new WaitForSeconds(remainTime);
    }



    /*───────────────────────────────*
     * 특수 공격
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
            case SpecialPattern.AOE:
                Aoe();
                break;
            case SpecialPattern.Laser:
                StartLaser();
                break;
        }

        // 애니메이션 종료까지 대기
        yield return new WaitForSeconds(total - hitTime);

        // 종료 처리
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
        StartCoroutine(DisableAoeAfterTime(0.5f));
    }

    IEnumerator DisableAoeAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        aoeHitbox.SetActive(false);
    }


    void StartLaser()
    {
        if (laserPrefab == null || firePoint == null) return;

        Instantiate(
            laserPrefab,
            firePoint.position,
            firePoint.rotation   // 🔥 이 순간 방향만 사용
        );
    }





    /*───────────────────────────────*
     * 데미지 / 피격
     *───────────────────────────────*/
    public void TakeDamage(DamageData data)
    {
        if (state == MonsterState.Die) return;

        currentHP -= data.damageAmount;
        print($"최대 {monsterData.HP}/현재 {currentHP}");

        if (state == MonsterState.GetHit) return;
        // 🔥 공격 중이면 피격 연출 없이 HP만 감소
        if (state == MonsterState.Attack)
        {
            if (currentHP <= 0)
            {
                state = MonsterState.Die;                                                                                           
                Die();
            }   
            return;
        }

        // 공격 중이 아닐 때만 피격 처리
        if (currentHP <= 0)
        {
            state = MonsterState.Die;
            Die();
        }
        else
        {
            StartCoroutine(GetHitProc());
        }

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

        _MasterManager.Instance.DataManager.GetMonster(monsterData);
        Destroy(gameObject);
    }

    void DropItems()
    {
        if (monsterData.DropTable == null || monsterData.DropTable.Length == 0)
            return;

        foreach (var drop in monsterData.DropTable)
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


    //공격중 반환
    public bool OnAttack()
    {
        if (state == MonsterState.Attack) return true;
        else return false;
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

