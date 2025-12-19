using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem.Android;


/// <summary>
/// 코뿔소 보스 전용FSM
/// 근접일반 공격
/// 특수 : 포효(전방위 범위 공격), 돌진(보스기준 정면으로 이정거리 빠르게 이동하며 히트 판정)
/// 위 패턴들은 데이터기반 + FSM에서 필터링(해당 패턴만을 받아오게 할지는 고민중(데이터에서 입력을 잘해 놓는다면 어떤 방식이든 상관없을듯)
/// 데이터 상엔 공간이 있더라도 해당 몹의 FSM에서 사용하지 않는다면 참조 및 변수 사용 안함
/// </summary>
public class RihinoFSM : MonoBehaviour
{
    //상태정의(기본적으로 상태의 경우 비슷할 듯함)
    enum RihinoState
    {
        Idle,       //정지상태(공격 후 잠시 유지 등)
        Move,       //이동상태(지금 만드는 프로젝트 특성상 바로 플레이어 추적(후퇴는 최소거리 미만으로 들어왔을 시)
        Attack,     //공격 / 다양한 패턴이 존재하나 상태는 하나로 통일 내부에서 패턴 세부화
        GetHit,     //공격 받는것 / 애니메이션이 존재하기도 하고 넣으면 좋긴하나 매번 맞을때마다 해당 애니메이션이 나오면 보스의 위엄등이 깍임(일정 횟수 맞
                    //거나 일정 체력 이하가 될때마다 애니메이션 출력이 좋을 듯)
        Die         //상태 그대로 죽음, 죽을 시 던전쪽에 죽었다는 정보 전달 및 아이템 드랍, 코뿔소 보스 일정 시간 후 삭제 등 수행)
    }

    RihinoState state;

    //참조
    Transform target;           //목표물(플레이어 캐릭터)
    public MonsterData rihinoData;  //데이터에서 값을 받아와야 하기에
    NavMeshAgent agent;         //이동은 에이전트로 처리
    public Animator anim;       //애니메이션은 루트 안의 오브젝트에서
    RoomController roomController;//보스 사망시 사망했다는 정보를 던전 방에 전달

    //기본적으로 데이터에서 계승 + 데이터엔 존재하지 않으나 해당 FSM에만 필요한 변수의 경우 FSM자체적으로 추가

    //기본스탯
    float currentHP;   
    float speed;    
    float def;

    //공격의 쿨타임
    //일반 공격
    float coolTime;
    float timer;
    //특수
    float specialCoolTime;
    float specialTimer;

    //거리
    float attRange;     //공격 가능 최대거리
    float detRange;     //인식범위(현 게임 특성상 무조건 플레이어 추적하기에 데이터에서 큰값으로 적용됨
    float minRange;     //공격 가능 최소범위(공격 방식이 히트박스에 들어서는 방식이기에 최소 최대값 필요

    //패턴 데이터
    NormalPattern[] normalPatterns;  //일반공격의 패턴 배열의 이유는 나중에 일반공격으로 분류되는 패턴의 추가 고려(일반몹FSM에서는 사용하지 않음 이유는 일반공격의 아이디로 사용했기에
    SpecialPattern[] specialPatterns;//특수공격의 패턴 배열/ 특수공격의 경우 현재로는 돌진, 포효가 있기에 배열로 현재로는 배열에서 랜덤으로 뽑지는 않을듯함
    int[] normalPatternIDs; //일반공격 패턴의 아이디 배열(직접적으로 패턴과 열결 되어있진 않으나
                            //애니메이터에서 인스펙터에 입력한 것에 맞춰 설정 이후 해당 배열에서 랜덤 선택후 해당 애니메이션 출력

    //보스 패턴 변수 (돌진거리, 후딜레이 / 포효 범위등 현재 해당 보스의 패턴에 맞는 변수만 사용)
    float chargeDistance;       //돌진거리
    float chargeStop;           //돌진 후딜
    float roarRange;            //포효 범위

    //FX
    GameObject attackFX;
    GameObject hitFX;
    GameObject deathFX;

    //초기화
    void Start()
    {
        if (rihinoData == null) return;
        
        state = RihinoState.Idle;

        anim = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();

        //기본스탯
        currentHP = rihinoData.HP;
        speed = rihinoData.Speed;
        def = rihinoData.Defense;

        //쿨타임
        coolTime = rihinoData.CoolTime;
        specialCoolTime = rihinoData.specialCoolTime;

        timer = coolTime;
        specialTimer = specialCoolTime;

        //거리
        attRange = rihinoData.attackRange;
        detRange = rihinoData.detectionRange;
        minRange = rihinoData.minAttackRange;

        //패턴
        normalPatterns = rihinoData.NormalPatterns;
        specialPatterns = rihinoData.SpecialPatterns;
        normalPatternIDs = rihinoData.NormalpatternIDs;

        chargeDistance = rihinoData.chargeDistance;
        chargeStop = rihinoData.chargeStoppingTime;
        roarRange = rihinoData.roarRange;

        // FX
        attackFX = rihinoData.attackFX;
        hitFX = rihinoData.hitFX;
        deathFX = rihinoData.deathFX;

        agent.speed = speed;
        agent.isStopped = false;
    }

    void Update()
    {
        if (state == RihinoState.Idle || state == RihinoState.Move)
        {
            if(timer > 0) timer -= Time.deltaTime;
            if(specialTimer > 0) specialTimer -= Time.deltaTime;
        }

        switch (state)
        {
            case RihinoState.Idle: Idle(); break;
            case RihinoState.Move: Move(); break;
            case RihinoState.Attack: Attack(); break;
            case RihinoState.GetHit: break;
            case RihinoState.Die: break;
        }

    }

    //기본상태
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
            state = RihinoState.Move;
        }
    }

    //이동
    void Move()
    {
        anim.applyRootMotion = false;
        if (target == null) return;
        anim.SetBool("isMove", true);
        agent.isStopped = false;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance < minRange)
        {
            //너무 가까이 있어 히트박스에 안맞음 -> 거리 벌리기
            Vector3 awayDir = (transform.position - target.position).normalized;
            Vector3 adjustPos = transform.position + awayDir * 1f;

            agent.SetDestination(adjustPos);
            return;
        }

        //공격 사거리 밖 -> 추적
        if (distance > attRange)
        {
            agent.SetDestination(target.position);
            return;
        }

        //공격 가능거리
        anim.SetBool("isMove", false);
        agent.isStopped = true;
        state = RihinoState.Attack;
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
        state = RihinoState.Idle;
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
            if (sp == SpecialPattern.Charge && chargeDistance > 0) return true;

            if (sp == SpecialPattern.Roar) return true;
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
        float remainTime = total - (hitTime + hitDuration); // == 40
        yield return new WaitForSeconds(hitTime);

        yield return new WaitForSeconds(hitDuration);

        yield return new WaitForSeconds(remainTime);
        
        

        // RootMotion 종료 → NavMesh 복귀
        anim.applyRootMotion = false;

        timer = coolTime;
        isActing = false;
        state = RihinoState.Idle;
    }

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
                Charge();
                break;
            case SpecialPattern.Roar:
                Roar();
                break;
        }

        // 애니메이션 종료까지 대기
        yield return new WaitForSeconds(total - hitTime);

        // 종료 처리
        anim.applyRootMotion = false;

        specialTimer = specialCoolTime;
        isActing = false;
        state = RihinoState.Idle;
    }

    SpecialPattern GetValidSpecialPattern()
    {
        List<SpecialPattern> valid = new List<SpecialPattern>();

        foreach (var sp in specialPatterns)
        {
            if (sp == SpecialPattern.Charge && chargeDistance > 0f)
                valid.Add(sp);

            if (sp == SpecialPattern.Roar)
                valid.Add(sp);
        }

        if (valid.Count == 0) return default;
        return valid[Random.Range(0, valid.Count)];
    }

    void Charge()
    {

    }

    void Roar()
    {

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
            // 1️ 확률 체크
            float roll = Random.value; // 0.0 ~ 1.0
            if (roll > drop.chance)
                continue;

            // 2️ 드랍 개수 결정
            int count = Random.Range(drop.minCount, drop.maxCount + 1);
            if (count <= 0)
                continue;

            // 3️ 아이템 생성
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
        if (state == RihinoState.Attack) return true;
        else return false;
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

        if (rihinoData.roarRange > 0f)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, rihinoData.roarRange);
        }
    }

}
