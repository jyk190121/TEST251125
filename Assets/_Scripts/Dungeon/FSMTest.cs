using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 일반 몬스터 FSM (안정화 리팩터링 버전)
/// - 상태 전이 단일 책임화
/// - 사망 상태 고정 (Update 차단)
/// - 코루틴 중복 실행 방지
/// - NavMesh / RootMotion 충돌 제거
/// - 죽었는데 안 사라지는 문제 해결
/// </summary>
public class FSMTest : MonoBehaviour, IHitResponder
{
    private RoomController currentRoom;

    /* ================= 상태 ================= */
    enum MonsterState { Idle, Move, Attack, GetHit, Die }
    MonsterState state;

    /* ================= 참조 ================= */
    Transform target;
    NavMeshAgent agent;
    Animator anim;
    public MonsterData monsterData;

    /* ================= 스탯 ================= */
    float hp;
    float speed;

    /* ================= 쿨 ================= */
    float atkTimer;
    float spTimer;

    /* ================= 거리 ================= */
    float atkRange;
    float detectRange;
    float minRange;

    /* ================= 분류 ================= */
    Type monsterType;

    /* ================= 패턴 ================= */
    int[] normalPatternIDs;
    SpecialPattern[] specialPatterns;

    /* ================= 내부 플래그 ================= */
    bool isActing;
    bool isDead;
    Coroutine currentRoutine;

    /* ================= 특수 ================= */
    public GameObject aoeHitbox;
    public Transform firePoint;

    /* ================= 초기화 ================= */
    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        if (monsterData == null)
        {
            Debug.LogError("MonsterData 없음");
            enabled = false;
            return;
        }

        hp = monsterData.HP;
        speed = monsterData.Speed;

        atkRange = monsterData.attackRange;
        detectRange = monsterData.detectionRange;
        minRange = monsterData.minAttackRange;

        atkTimer = monsterData.CoolTime;
        spTimer = monsterData.specialCoolTime;

        monsterType = monsterData.Type;
        normalPatternIDs = monsterData.NormalpatternIDs;
        specialPatterns = monsterData.SpecialPatterns;

        agent.speed = speed;
        agent.isStopped = true;

        ChangeState(MonsterState.Idle);
    }

    /* ================= Update ================= */
    void Update()
    {
        if (isDead) return;

        if (state == MonsterState.Idle || state == MonsterState.Move)
        {
            atkTimer -= Time.deltaTime;
            spTimer -= Time.deltaTime;
        }

        switch (state)
        {
            case MonsterState.Idle: UpdateIdle(); break;
            case MonsterState.Move: UpdateMove(); break;
        }
    }

    /* ================= 상태 처리 ================= */
    void UpdateIdle()
    {
        agent.isStopped = true;
        anim.SetBool("isMove", false);

        if (target == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) target = p.transform;
            return;
        }

        if (Vector3.Distance(transform.position, target.position) <= detectRange)
            ChangeState(MonsterState.Move);
    }

    void UpdateMove()
    {
        if (target == null) return;

        agent.isStopped = false;
        anim.SetBool("isMove", true);

        float dist = Vector3.Distance(transform.position, target.position);

        if (monsterType == Type.Range)
        {
            if (dist < minRange)
            {
                Vector3 away = (transform.position - target.position).normalized;
                agent.SetDestination(transform.position + away * 1.2f);
                return;
            }
        }

        if (dist <= atkRange)
        {
            ChangeState(MonsterState.Attack);
            return;
        }

        agent.SetDestination(target.position);
    }

    /* ================= 상태 전이 ================= */
    void ChangeState(MonsterState next)
    {
        if (isDead) return;
        state = next;

        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }

        switch (state)
        {
            case MonsterState.Attack:
                currentRoutine = StartCoroutine(AttackRoutine());
                break;
            case MonsterState.GetHit:
                currentRoutine = StartCoroutine(GetHitRoutine());
                break;
            case MonsterState.Die:
                currentRoutine = StartCoroutine(DieRoutine());
                break;
        }
    }

    /* ================= 공격 ================= */
    IEnumerator AttackRoutine()
    {
        isActing = true;
        agent.isStopped = true;
        anim.applyRootMotion = true;

        FaceTarget();

        anim.SetTrigger("Attack");
        yield return null;

        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(info.length);

        anim.applyRootMotion = false;
        atkTimer = monsterData.CoolTime;
        isActing = false;

        ChangeState(MonsterState.Idle);
    }

    void FaceTarget()
    {
        if (target == null) return;
        Vector3 dir = target.position - transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude > 0.01f)
            transform.rotation = Quaternion.LookRotation(dir);
    }

    /* ================= 피격 ================= */
    public void TakeDamage(DamageData data)
    {
        if (isDead) return;

        hp -= data.damageAmount;

        if (hp <= 0)
        {
            Die();   // 상태 변경 요청만
            return;
        }

        if (state != MonsterState.Attack)
            ChangeState(MonsterState.GetHit);
    }

    IEnumerator GetHitRoutine()
    {
        agent.isStopped = true;
        anim.applyRootMotion = true;
        anim.SetTrigger("Hit");

        yield return null;
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(info.length);

        anim.applyRootMotion = false;
        ChangeState(MonsterState.Idle);
    }

    public void Die()
    {
        if (isDead) return;

        ChangeState(MonsterState.Die);

    }

    /* ================= 사망 ================= */
    IEnumerator DieRoutine()
    {
        isDead = true;
        agent.isStopped = true;
        agent.enabled = false;
        anim.applyRootMotion = true;  

        anim.SetTrigger("Die");

        if (currentRoom != null)
        {
            currentRoom.ClearDungeon(gameObject);
        }

        yield return new WaitForSeconds(2f);
        Destroy(gameObject);
    }

    public void SetupRoom(RoomController room)
    {
        currentRoom = room;
    }
}
