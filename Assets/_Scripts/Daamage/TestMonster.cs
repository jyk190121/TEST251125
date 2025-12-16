using UnityEngine;

// IHitResponder 인터페이스를 구현하여 DamageDealer로부터 피해를 받을 수 있게 합니다.
public class TestMonster : MonoBehaviour, IHitResponder
{
    [Header("몬스터 설정")]
    [SerializeField]
    private float currentHealth = 100f;

    // 몬스터의 레이어 설정 (Unity Inspector에서 11번 레이어로 설정해야 합니다.)
    private const int MonsterLayer = 11;

    private const int BossLayer = 10;

    private RoomController roomController;

    private void Awake()
    {
        if (gameObject.layer == BossLayer) return;

        // 몬스터 오브젝트의 레이어를 설정해주는 것이 좋습니다.
        // Hierarchy에서 이 오브젝트를 선택하고 Inspector에서 Layer를 11번(Monster)으로 설정해야 합니다.
        else gameObject.layer = MonsterLayer;
        Debug.Log("몬스터 레이어 지정");

    }

    // IHitResponder 인터페이스 구현
    public void TakeDamage(DamageData data)
    {
        // 1. 데미지 감소 처리
        currentHealth -= data.damageAmount;

        // 2. 피격 확인 로그 출력
        Debug.Log($"몬스터가 피해를 입었습니다! [주체: {data.damageSource}, 피해량: {data.damageAmount}, 남은 체력: {currentHealth}]");

        // 3. 몬스터 사망 처리
        if (currentHealth <= 0)
        {
            Die();
        }

        // (추가 작업 예시) 넉백 처리: data.hitDirection을 사용하여 힘을 가할 수 있습니다.
        // GetComponent<Rigidbody>()?.AddForce(data.hitDirection * 5f, ForceMode.Impulse);
    }

    public void SetupRoom(RoomController room)
    {
        roomController = room;
    }

    public void Die()
    {
        /*
        Debug.Log("몬스터 사망!");
        // 테스트를 위해 오브젝트를 비활성화합니다.
        gameObject.SetActive(false);
        */

        // 실제 게임에서는 파티클, 애니메이션, 드롭 아이템 등의 처리가 필요합니다.

        if (roomController != null)
        {
            // 몬스터 사망 시 RoomController에 자신(gameObject)이 죽었음을 알립니다.
            // [roomController.ClearDungeon 설명]: 이 함수를 통해 RoomController는 리스트에서 몬스터를 제거하고, 
            // 몬스터 수가 0이 되면 UnlockDoors()를 호출하게 됩니다.
            roomController.ClearDungeon(gameObject);
        }

        // 몬스터 오브젝트 파괴 (또는 비활성화)
        Destroy(gameObject);
    }
}