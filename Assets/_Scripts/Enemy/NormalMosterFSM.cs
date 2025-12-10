using Unity.VisualScripting;
using UnityEditor.Analytics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TestTools;

public class NormalMosterFSM : MonoBehaviour
{
    enum MonsterState
    {
        Idle,
        Move,
        Attack,
        GetHit,  //내부에서 데미지 처리하면 될듯
        Die
    }

    MonsterState state;

    Transform target;

    public MonsterData monsterData;

    //몬스터 일반 변수
    float currentHP;
    float att;
    float speed;
    float def;
    //몬스터 공격 딜레이
    float coolTime;
    float timer = 0f;
    //몬스터 공격/인식 범위
    float attRange;
    float detRange;

    //원거리
    float minRangeRange;

    Type monsterType;   //몬스터 Melee/Range
    Race monsterRace;   //몬스터 종족

    MonsterPattern[] patterns;  //패턴 목록

    //몬스터 공격 세부
    float windupTime;  //공격준비(바람잡기)
    float recoveryTime; //공격 후딜

    //세부 - 근접
    float meleeRadius;  //데이터에서 attackRadius(공격 거리
    float meleeAngle;   //데이터에서 attackAngle(공격 각도(?))
    float aoeRange;     //데이터에서 aoeRange(범위)
    float aoeDamageMultiplier;

    //세부 - 원거리
    float projectileSpeed;
    int projectileCount;
    float shotInterval;
    int burstCount;

    //FX관련
    GameObject attackFX;
    GameObject hitFX;
    GameObject deathFX;

    NavMeshAgent agent;

    //플레이어 레이어
    LayerMask player = 7;

    public Animator anim;

    void Start()
    {
        if (monsterData == null)
        {
            Debug.LogError("monsterData가 NULL이다! 프리팹에 monsterData 넣어야 함");
        }

        state = MonsterState.Idle;

        //target = GameObject.FindWithTag("Player").transform;
        anim = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        //agent.enabled = false;

        //기본스탯
        currentHP = monsterData.HP;
        att = monsterData.Attack;
        speed = monsterData.Speed;
        def = monsterData.Defense;
        //쿨
        coolTime = monsterData.CoolTime;
        //범위
        attRange = monsterData.attackRange;
        detRange = monsterData.detectionRange;
        //원거리
        minRangeRange = monsterData.minAttackRange;

        //몬스터 정보
        monsterType = monsterData.Type;
        monsterRace = monsterData.Race;
        patterns = monsterData.Pattern;
        //패턴 파라미터
        windupTime = monsterData.windupTime;
        recoveryTime = monsterData.recoveryTime;

        meleeRadius = monsterData.attackRadius;
        meleeAngle = monsterData.attackAngle;
        aoeRange = monsterData.aoeRange;
        aoeDamageMultiplier = monsterData.aoeDamageMultiplier;

        projectileSpeed = monsterData.projectileSpeed;
        projectileCount = monsterData.projectileCount;
        shotInterval = monsterData.shotInterval;
        burstCount = monsterData.burstCount;

        // FX
        attackFX = monsterData.attackFX;
        hitFX = monsterData.hitFX;
        deathFX = monsterData.deathFX;

        // Agent Speed
        agent.speed = speed;

        

    }

    // Update is called once per frame
    void Update()
    {
        if (attRange == monsterData.attackRange)
        {
            print("값 받아옴");

        }
        switch (state)
        {
            case MonsterState.Idle:
                Idle();
                break;
            case MonsterState.Move:
                Move();
                break;
            case MonsterState.Attack:
                Attack();
                break;
            case MonsterState.GetHit:
                GetHit();
                break;
            case MonsterState.Die:
                Die();
                break;
        }

    }

    void Idle()
    {

    }

    void Move()
    {

    }

    void Attack()
    {

    }

    void GetHit()
    {

    }

    void Die()
    {

    }
    private void OnDrawGizmos()
    {
        //공격가능범위
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, monsterData.attackRange);
        //원거리 최소거리
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, monsterData.minAttackRange);
    }
}
