using NUnit.Framework.Interfaces;
using UnityEngine;

public enum Class
{
    Normal,     // 일반몹
    Boss        // 보스몹
}

public enum Type
{
    Melee,      // 근접 공격
    Range       // 투사체 / 레이저 등 원거리 공격
}

public enum Race
{
    Slime,
    Golem,
    Drone,
    Beholder,
    Mimic,
    Dragon,
    Rhino
}

public enum NormalPattern
{
    /*───────────────────────────────*
     *  기본 공격 패턴 (공통)
     *───────────────────────────────*/
    MeleeAttack,        // 일반 근접 공격
    RangedAttack,       // 일반 원거리 공격(투사체 기본)
}

public enum SpecialPattern
{
    /*───────────────────────────────*
     *  특수 공격 패턴 (특정 몬스터)
     *───────────────────────────────*/
    AOE = 100,                // 골렘 발구르기 같은 범위 공격
    MimicTrap = 200,          // 미믹 기습
    Laser = 300,              // 드론/비홀더 등 고정 방향 레이저

    /*───────────────────────────────*
     *  보스 전용 패턴
     *───────────────────────────────*/
    Breath = 400,             // 드래곤 브레스
    Charge = 401,             // 코뿔소 돌진 / 드래곤 박치기
    Roar = 402,               // 드래곤 Scream, 코뿔소 Shout (광역 포효)
    JumpSmash = 403,          // 드래곤 점프 후 착지 공격(필요시)

    /*───────────────────────────────*
     *  연출 / 특수 동작
     *───────────────────────────────*/
    PhaseChange = 500        // 보스 페이즈 전환 연출 (필요하면)
}


[System.Serializable]
public struct DropItem
{
    public GameObject itemPrefab;
    public float chance;
    public int minCount;
    public int maxCount;
}

[CreateAssetMenu(fileName = "NewMonsterData", menuName = "Monster/Monster Data")]
public class MonsterData : ScriptableObject
{
    /*───────────────────────────────────────────────*
     *  기본 정보
     *───────────────────────────────────────────────*/
    [Header(" 기본 정보")]
    [Tooltip("몬스터 고유 ID (데이터 관리용)")]
    public int mobID;

    [Tooltip("몬스터 이름")]
    public string mobName;

    [Tooltip("소환할 몬스터 프리팹")]
    public GameObject MobPrefab;


    /*───────────────────────────────────────────────*
     *  공통 스탯
     *───────────────────────────────────────────────*/
    [Header(" 몬스터 기본 스탯")]
    [Tooltip("체력")]
    public float HP;

    [Tooltip("이동 속도")]
    public float Speed;

    [Tooltip("공격력 (기본 공격 데미지)")]
    public float Attack;

    [Tooltip("방어력 (데미지 감소량)")]
    public float Defense;

    [Tooltip("공격 쿨타임")]
    public float CoolTime;

    [Tooltip("근접 또는 투사체 사거리")]
    public float attackRange;

    [Tooltip("플레이어를 탐지하는 거리")]
    public float detectionRange;

    [Tooltip("원거리 몬스터가 최소 거리를 유지할 때 사용")]
    public float minAttackRange;


    /*───────────────────────────────────────────────*
     *  몬스터 유형
     *───────────────────────────────────────────────*/
    [Header(" 몬스터 분류")]
    public Class Class;
    public Type Type;
    public Race Race;


    /*───────────────────────────────────────────────*
     *  패턴 목록
     *───────────────────────────────────────────────*/
    [Header(" 행동 패턴 목록")]
    [Tooltip("몬스터가 사용할 일반공격 패턴들 (FSM이 이 배열 기준으로 공격 선택)")]
    public NormalPattern[] NormalPatterns;
    [Tooltip("몬스터가 사용할 특수공격 패턴들")]
    public SpecialPattern[] SpecialPatterns;


    
    /*───────────────────────────────────────────────*
     *  애니메이터 Attack 패턴 번호
     *───────────────────────────────────────────────*/
    [Header(" 이 몬스터가 사용할 애니메이터 특수, 일반 PatternID 리스트")]
    [Tooltip("Animator의 Pattern(int) 값. 랜덤으로 하나 선택됨.")]
    public int[] NormalpatternIDs;


    /*───────────────────────────────────────────────*
     *  공통 패턴 파라미터
     *───────────────────────────────────────────────*/
    [Header("📌 패턴 공통 파라미터")]
    [Tooltip("공격 준비 시간(바람잡기). 공격 전에 애니나 딜레이가 필요한 몹에게 사용")]
    public float windupTime = 0.1f;

    [Tooltip("공격 후 딜레이. 공격 후 멍때리는 시간")]
    public float recoveryTime = 0.2f;

    [Tooltip("경직 저항 수치. 높을수록 히트 시에도 경직이 덜 걸림")]
    public float poise = 0f;

    /*───────────────────────────────────────────────*
 *  특수 공격 설정 (일반몹/보스 공통)
 *───────────────────────────────────────────────*/
    [Header(" 특수 공격 설정")]
    [Tooltip("특수 공격 쿨타임")]
    public float specialCoolTime = 3f;


    /*───────────────────────────────────────────────*
     *  근접 공격 전용
     *───────────────────────────────────────────────*/
    [Header(" 근접 공격 관련")]

    [Space(5)]
    [Tooltip("AOE(발구르기 등) 범위")]
    public float aoeRange = 2f;


    //원거리 투사체 프리팹
    public GameObject projectilePrefab;

    //원거리 레이저 프리팹
    public GameObject laserPrefab;

    /*───────────────────────────────────────────────*
     *  드론(레이저) 전용
     *───────────────────────────────────────────────*/
    [Header(" 드론(레이저 몹) 전용")]
    [Tooltip("드론이 이동할 수 있는 범위 (XZ 좌표 기준)")]
    public float moveRange = 5f;

    [Tooltip("랜덤 이동 간 대기 시간")]
    public float moveInterval = 1f;


    /*───────────────────────────────────────────────*
     *  보스 몬스터 전용
     *───────────────────────────────────────────────*/
    [Header(" 보스 전용 파라미터")]
    [Tooltip("보스 패턴 간 기본 쿨타임")]
    public float patternCooldown = 3f;

    [Tooltip("페이즈 2 전환 HP 비율 (0.5 = 50%)")]
    public float phaseTwoHpRate = 0.5f;

    [Space(5)]
    [Tooltip("드래곤 점프 착지 AOE 범위")]
    public float jumpAoeRadius = 3f;

    [Tooltip("드래곤의 Scream 범위")]
    public float screamRange = 10f;

    [Space(5)]
    [Tooltip("코뿔소 돌진 거리")]
    public float chargeDistance = 10f;

    [Tooltip("돌진 종료 후 멈춰있는 시간")]
    public float chargeStoppingTime = 1f;

    [Tooltip("Shout 범위")]
    public float shoutRange = 8f;


    /*───────────────────────────────────────────────*
     *  FX (효과)
     *───────────────────────────────────────────────*/
    [Header(" FX / 연출")]
    [Tooltip("공격 시 생성될 FX (없으면 null 허용)")]
    public GameObject attackFX;

    [Tooltip("피격 시 나타날 FX")]
    public GameObject hitFX;

    [Tooltip("죽을 때 생성될 FX")]
    public GameObject deathFX;


    /*───────────────────────────────────────────────*
     *  드랍 정보
     *───────────────────────────────────────────────*/
    [Header(" 드랍 테이블")]
    public DropItem[] DropTable;
}

