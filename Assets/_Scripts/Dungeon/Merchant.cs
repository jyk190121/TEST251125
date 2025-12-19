using UnityEngine;

public class Merchant : MonoBehaviour
{
    // 플레이어가 판매 가능 범위 내에 있는지 확인하는 변수입니다.
    public bool isPlayerNearby = false;

    // 플레이어가 상인의 트리거 범위에 들어왔을 때 호출됩니다.
    private void OnTriggerEnter(Collider other)
    {
        // 팩트체크: 태그가 "Player"인 오브젝트만 인식합니다.
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            // 인벤토리 매니저에게 현재 상호작용 가능한 상인이 '나'임을 알립니다.
            InventoryManager.Instance.currentMerchant = this;
        }
    }

    // 플레이어가 범위를 벗어났을 때 호출됩니다.
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            // 상인 참조를 비우고, 열려있던 인벤토리(상점)창을 닫습니다.
            InventoryManager.Instance.currentMerchant = null;
            InventoryManager.Instance.inventory.gameObject.SetActive(false);
        }
    }
}