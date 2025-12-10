using UnityEditor.Analytics;
using UnityEngine;
using UnityEngine.AI;

public class NormalMosterFSM : MonoBehaviour
{
    enum MonsterState
    {
        Idle,
        Move,
        Attack,
        GetHit,  //쓸지는 모르겠다만 애니메이션은 있기에
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
    float timer = 0;
    //몬스터 공격/인식 범위
    float attRange;
    float detRange;

    NavMeshAgent agent;

    public Animator anim;

    void Start()
    {
        state = MonsterState.Idle;

        target = GameObject.FindWithTag("Player").transform;

        anim = GetComponentInChildren<Animator>();

        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;

        currentHP = monsterData.HP;
        att = monsterData.Attack;
        speed = monsterData.Speed;
        def = monsterData.Defense;
        coolTime = monsterData.CoolTime;
        attRange = monsterData.attackRange;
        detRange = monsterData.detectionRange;
    }

    // Update is called once per frame
    void Update()
    {
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

}
