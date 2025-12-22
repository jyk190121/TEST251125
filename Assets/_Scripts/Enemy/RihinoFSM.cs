using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 코뿔소 보스 전용 FSM
/// - 기본 근접 공격
/// - 특수 공격 : 돌진
/// 
/// ✔ 돌진 시작 시 플레이어 위치 고정
/// ✔ NavMesh 유지
/// ✔ 돌진 중에만 히트박스 활성화
/// ✔ 애니메이션 없을 경우 2초 정지 fallback
/// </summary>
public class RihinoFSM : MonoBehaviour
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

    [Header("HitBox")]
    [Tooltip("돌진 중 활성화될 히트박스")]
    public GameObject chargeHitBox;

    /*───────────────────────────────*
     * 스탯 / 쿨타임
     *───────────────────────────────*/
    float currentHP;
    float speed;

    float normalCool;
    float normalTimer;

    float specialCool;
    float specialTimer;

    /*───────────────────────────────*
     * 거리
     *───────────────────────────────*/
    float attRange;
    float detRange;
    float minRange;

    /*───────────────────────────────*
     * 돌진 관련
     *───────────────────────────────*/
    Vector3 chargeTargetPos;
    float chargeDuration = 0.8f;      // 애니메이션 기준
    float chargeTimer;
    float chargeSpeedMul = 3f;
    float originalSpeed;
    float chargeStop;

    bool isCharging;
    bool isActing;

    /*───────────────────────────────*
     * 초기화
     *───────────────────────────────*/
    void Start()
    {
        if (rihinoData == null) return;

        state = RihinoState.Idle;

        anim = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();

        currentHP = rihinoData.HP;
        speed = rihinoData.Speed;

        normalCool = rihinoData.CoolTime;
        specialCool = rihinoData.specialCoolTime;

        normalTimer = normalCool;
        specialTimer = specialCool;

        attRange = rihinoData.attackRange;
        detRange = rihinoData.detectionRange;
        minRange = rihinoData.minAttackRange;

        chargeStop = rihinoData.chargeStoppingTime;

        agent.speed = speed;
        originalSpeed = speed;

        if (chargeHitBox != null)
            chargeHitBox.SetActive(false);
    }

    void Update()
    {
        if (isCharging)
        {
            UpdateCharge();
            return;
        }

        if (state == RihinoState.Idle || state == RihinoState.Move)
        {
            if (normalTimer > 0) normalTimer -= Time.deltaTime;
            if (specialTimer > 0) specialTimer -= Time.deltaTime;
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
        agent.isStopped = true;
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

        FaceTargetOnce();

        if (specialTimer <= 0f)
        {
            StartCoroutine(ExecuteCharge());
            return;
        }

        StartCoroutine(NormalAttackFallback());
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
     * 일반 공격 (fallback)
     *───────────────────────────────*/
    IEnumerator NormalAttackFallback()
    {
        anim.SetTrigger("Attack");

        float wait = 2f; // 애니 없을 경우 fallback
        if (anim != null)
        {
            yield return null;
            wait = anim.GetCurrentAnimatorStateInfo(0).length;
        }

        yield return new WaitForSeconds(wait);

        normalTimer = normalCool;
        isActing = false;
        state = RihinoState.Idle;
    }

    /*───────────────────────────────*
     * 돌진
     *───────────────────────────────*/
    IEnumerator ExecuteCharge()
    {
        anim.SetTrigger("Attack");

        yield return null;

        float animLength = anim != null
            ? anim.GetCurrentAnimatorStateInfo(0).length
            : 2f;

        yield return new WaitForSeconds(animLength * 0.4f);

        StartCharge(animLength);

        yield return new WaitForSeconds(animLength * 0.6f);
    }

    void StartCharge(float animLength)
    {
        if (target == null) return;

        Vector3 dir = target.position - transform.position;
        dir.y = 0f;
        dir.Normalize();

        chargeTargetPos = transform.position + dir * rihinoData.chargeDistance;

        agent.speed = originalSpeed * chargeSpeedMul;
        agent.isStopped = false;
        agent.SetDestination(chargeTargetPos);

        chargeTimer = animLength * 0.6f;
        isCharging = true;

        // 🔥 돌진 히트박스 ON
        if (chargeHitBox != null)
            chargeHitBox.SetActive(true);
    }

    void UpdateCharge()
    {
        chargeTimer -= Time.deltaTime;

        if (chargeTimer <= 0f)
            EndCharge();
    }

    void EndCharge()
    {
        isCharging = false;

        agent.isStopped = true;
        agent.speed = originalSpeed;

        // 🔥 돌진 히트박스 OFF
        if (chargeHitBox != null)
            chargeHitBox.SetActive(false);

        StartCoroutine(ChargeRecovery());
    }

    IEnumerator ChargeRecovery()
    {
        yield return new WaitForSeconds(chargeStop);

        specialTimer = specialCool;
        isActing = false;
        state = RihinoState.Idle;
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

