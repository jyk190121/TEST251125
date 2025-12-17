using UnityEngine;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour
{

    [SerializeField] private Item[] allItems;

    // 모든 아이템을 저장할 딕셔너리 (Key: ItemID)
    private Dictionary<int, Item> itemDatabase = new Dictionary<int, Item>();

    // 아이템 ID로 Item 찾기
    public Item GetItemByID(int itemID)
    {
        if (itemDatabase.ContainsKey(itemID))
        {
            return itemDatabase[itemID];
        }
        Debug.LogWarning($"[ItemManager] ItemID {itemID}를 찾을 수 없습니다!");
        return null;
    }

    // 아이템 수동 등록
    public void RegisterItem(Item item)
    {
        if (item == null)
        {
            Debug.LogError("[ItemManager] null 아이템을 등록할 수 없습니다!");
            return;
        }

        if (itemDatabase.ContainsKey(item.itemID))
        {
            Debug.LogWarning($"[ItemManager] ItemID {item.itemID}은 이미 등록되어 있습니다!");
            return;
        }

        itemDatabase.Add(item.itemID, item);
        Debug.Log($"[ItemManager] 아이템 등록: {item.itemName} (ID: {item.itemID})");
    }

    // 모든 아이템 조회 (디버그용)
    public Dictionary<int, Item> GetAllItems()
    {
        return new Dictionary<int, Item>(itemDatabase);
    }

    // 게임 시작 시 모든 아이템 로드
    public void Initialize()
    {
        Debug.Log("[ItemManager] 초기화 시작");

        if (allItems == null || allItems.Length == 0)
        {
            Debug.LogError("[ItemManager] Inspector에 아이템이 연결되지 않았습니다!");
            return;
        }

        // Inspector의 배열을 딕셔너리로 변환
        foreach (Item item in allItems)
        {
            if (item != null)
            {
                itemDatabase.Add(item.itemID, item);
                Debug.Log($"[ItemManager] ✅ 아이템 로드: {item.itemName} (ID: {item.itemID})");
            }
        }

        Debug.Log($"[ItemManager] 초기화 완료: {itemDatabase.Count}개 아이템");
    }
}

