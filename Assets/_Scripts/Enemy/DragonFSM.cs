using UnityEngine;
using UnityEngine.AI;
using System.Collections;

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
public class DragonFSM : MonoBehaviour
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
    public GameObject aoeHitBox;

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

        currentHP = dragonData.HP;
        speed = dragonData.Speed;

        normalCool = dragonData.CoolTime;
        chargeCool = dragonData.patternCooldown;
        specialCool = dragonData.specialCoolTime;

        normalTimer = normalCool;
        chargeTimer = chargeCool;
        specialTimer = specialCool;

        attRange = dragonData.attackRange;
        detRange = dragonData.detectionRange;
        minRange = dragonData.minAttackRange;

        agent.speed = speed;
        originalSpeed = speed;

        if (chargeHitBox) chargeHitBox.SetActive(false);
        if (aoeHitBox) aoeHitBox.SetActive(false);
    }

    void Update()
    {
        if (isCharging)
        {
            UpdateCharge();
            return;
        }

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
        anim.SetBool("isMove", true);
        agent.isStopped = false;

        float dist = Vector3.Distance(transform.position, target.position);

        if (dist > attRange)
        {
            agent.SetDestination(target.position);
            return;
        }

        agent.isStopped = true;
        anim.SetBool("isMove", false);
        state = DragonState.Attack;
    }

    /*───────────────────────────────*
     * Attack
     *───────────────────────────────*/
    void Attack()
    {
        if (isActing) return;
        isActing = true;

        FaceTargetOnce();

        float dist = Vector3.Distance(transform.position, target.position);

        // 🔥 돌진 우선 조건
        if (dist > attRange && chargeTimer <= 0f)
        {
            StartCoroutine(ChargeAttack());
            return;
        }

        // 🔥 특수 공격
        if (specialTimer <= 0f)
        {
            StartCoroutine(SpecialAttack());
            return;
        }

        // 🔥 일반 공격
        StartCoroutine(NormalAttack());
    }

    void FaceTargetOnce()
    {
        Vector3 dir = target.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(dir);
    }

    /*───────────────────────────────*
     * 일반 공격
     *───────────────────────────────*/
    IEnumerator NormalAttack()
    {
        anim.SetTrigger("Attack");

        yield return null;
        float t = anim.GetCurrentAnimatorStateInfo(0).length;
        if (t <= 0) t = 1.5f;

        yield return new WaitForSeconds(t);

        normalTimer = normalCool;
        isActing = false;
        state = DragonState.Idle;
    }

    /*───────────────────────────────*
     * 특수 공격 (포효 / 점프)
     *───────────────────────────────*/
    IEnumerator SpecialAttack()
    {
        anim.SetTrigger("Attack");

        yield return null;
        float total = anim.GetCurrentAnimatorStateInfo(0).length;
        if (total <= 0) total = 2f;

        yield return new WaitForSeconds(total * 0.4f);

        // 랜덤 선택
        bool jump = Random.value < 0.5f;

        aoeHitBox.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        aoeHitBox.SetActive(false);

        yield return new WaitForSeconds(total * 0.6f);

        specialTimer = jump ? specialCool * 3f : specialCool;
        isActing = false;
        state = DragonState.Idle;
    }

    /*───────────────────────────────*
     * 돌진
     *───────────────────────────────*/
    IEnumerator ChargeAttack()
    {
        anim.SetTrigger("Attack");

        yield return null;
        float total = anim.GetCurrentAnimatorStateInfo(0).length;
        if (total <= 0) total = chargeDuration;

        yield return new WaitForSeconds(total * 0.3f);

        Vector3 dir = (target.position - transform.position).normalized;
        dir.y = 0f;

        chargeTarget = transform.position + dir * dragonData.chargeDistance;

        agent.speed = originalSpeed * chargeSpeedMul;
        agent.SetDestination(chargeTarget);
        isCharging = true;

        if (chargeHitBox) chargeHitBox.SetActive(true);

        yield return new WaitForSeconds(total * 0.7f);
    }

    void UpdateCharge()
    {
        if (!agent.pathPending && agent.remainingDistance <= 0.3f)
            EndCharge();
    }

    void EndCharge()
    {
        isCharging = false;

        agent.isStopped = true;
        agent.speed = originalSpeed;

        if (chargeHitBox) chargeHitBox.SetActive(false);

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

        if (state == DragonState.GetHit) return;
        // 🔥 공격 중이면 피격 연출 없이 HP만 감소
        if (state == DragonState.Attack)
        {
            if (currentHP <= 0)
            {
                state = DragonState.Die;
                Die();
            }
            return;
        }

        // 공격 중이 아닐 때만 피격 처리
        if (currentHP <= 0)
        {
            state = DragonState.Die;
            Die();
        }
        else
        {
            StartCoroutine(GetHitProc());
        }

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

