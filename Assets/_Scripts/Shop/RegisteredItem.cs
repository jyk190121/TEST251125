using NUnit.Framework.Internal.Execution;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 진열대 관리 시스템
/// - 인벤토리에서 아이템 드래그 -> 진열대 슬롯에 등록
/// - 슬롯별 아이템 + 수량 + 판매가 관리
/// - 손님 AI에게 현재 판매가 제공
/// </summary>
[System.Serializable]
public class RegisteredItem : MonoBehaviour
{
    public static RegisteredItem RegiItem;

    public ItemSettingPopup dropPopup;          // 팝업창 (아이템 갯수, 판매가격 설정창)

    [System.Serializable]
    public class RegisteredItemData
    {
        public Item item;
        public int count;
        public int price;

        public RegisteredItemData(Item item, int count, int price)
        {
            this.item = item;
            this.count = count;
            this.price = price;
        }
    }

    [Header("진열대 슬롯 데이터")]
    public RegisteredItemData[] registeredItemsData;
    public Item[] itemList;
    public Image[] itemImages;
    int capacity = 4;

    //드래그 시작한 슬롯 번호 (-1: 아무것도 안 잡음)
    private int dragStartIndex = -1;

    [Header("UI 참조")]
    public Transform itemParent;
    public Image currentImage;
    public TMP_InputField currentCount;
    public TMP_InputField currentPrice;

    InventoryManager inventory;
    Item dropItem;                         // 현재 드롭중인 아이템


    private void Awake()
    {
        if (RegiItem == null) RegiItem = this;
        else Destroy(gameObject);

        gameObject.SetActive(true);
    }

    private void Start()
    {
        // inventory 참조 초기화
        inventory = InventoryManager.Instance;
        if (inventory == null)
        {
            Debug.LogError("[RegisteredItem] InventoryManager.Instance를 찾을 수 없습니다");
            return;
        }

        // itemList 배열 검증
        if (itemList == null || itemList.Length == 0)
        {
            Debug.LogWarning("[RegisteredItem] itemList가 비어있습니다. 새로 초기화합니다");
            itemList = new Item[capacity];
            registeredItemsData = new RegisteredItemData[capacity];
            return;
        }

        // registeredItemsData 초기화
        registeredItemsData = new RegisteredItemData[capacity];


        // 기존 itemList를 registeredItemsData로 마이그레이션
        for (int i = 0; i < capacity && i < itemList.Length; i++)
        {
            if (itemList[i] != null)
            {
                registeredItemsData[i] = new RegisteredItemData(
                    itemList[i],
                    1,
                    itemList[i].sellPrice
                );

                SetupSlotImage(i, itemList[i]);
            }
        }

        Debug.Log("[RegisteredItem] 초기화 완료");
    }

    void SetupSlotImage(int index, Item item)
    {
        if (itemImages == null || index < 0 || index >= itemImages.Length)
            return;

        if (item == null)
            return;

        GameObject images = new GameObject("Image");
        images.transform.SetParent(itemImages[index].transform, false);

        var img = images.AddComponent<Image>();
        img.sprite = item.icon;
        img.enabled = true;
        img.preserveAspect = true;

        itemImages[index] = img;
    }

    //인벤토리에서 드래그 시작
    public void OnDragStart(int index)
    {
        dragStartIndex = index;
        if (inventory != null)
            inventory.OnDragStart(index);
    }

    //진열대 슬롯에 드롭
    public void OnDragEnd(int dropIndex)
    {
        // inventory 상태 확인
        if (inventory == null)
        {
            Debug.LogError("[RegisteredItem] inventory가 null입니다");
            CancelDrag();
            return;
        }

        int invDragStartIndex = inventory.GetDragStartIndex();

        // 인벤토리에서 진열대로 드롭하는 경우만 처리
        if (invDragStartIndex == -1)
        {
            Debug.Log("[RegisteredItem] 인벤토리 드래그가 활성화되지 않음");
            return;
        }

        // dropIndex 범위 확인
        if (dropIndex < -1 || dropIndex >= capacity)
        {
            Debug.LogWarning("[RegisteredItem] 진열대 범위 밖에 드롭됨");
            return;
        }

        if (dropIndex == -1)
        {
            Debug.Log("[RegisteredItem] 진열대 범위 밖 드롭 - 취소");
            return;
        }

        Item inventoryItem = inventory.GetDraggedItem();
        int inventoryCount = inventory.GetDraggedItemCount();

        // 아이템 검증
        if (inventoryItem == null)
        {
            Debug.LogError("[RegisteredItem] 드래그 아이템이 null입니다");
            return;
        }

        if (inventoryCount <= 0)
        {
            Debug.LogError("[RegisteredItem] 드래그 아이템 수량이 0 이하입니다");
            return;
        }

        Debug.Log($"[RegisteredItem] 드래그 시작: {inventoryItem.itemName} x{inventoryCount}");

        // 수량이 여러 개면 팝업으로 수량/가격 입력
        if (inventoryCount > 1)
        {
            // 팝업에 기본값 세팅
            currentImage.sprite = inventoryItem.icon;
            currentCount.text = inventoryCount.ToString();
            currentPrice.text = inventoryItem.sellPrice.ToString();

            dropPopup.OpenPopup(
                inventoryItem.itemName,
                onYes: () =>
                {
                    OnPopupYes(dropIndex, invDragStartIndex, inventoryItem, inventoryCount);
                },
                onNo: () =>
                {
                    OnPopupNo();
                }
            );
        }
        else
        {
            // 1개면 바로 등록 (기본 가격 = sellPrice)
            RegisterItemToSlot(dropIndex, inventoryItem, 1, inventoryItem.sellPrice);
            inventory.DecreaseItemAtIndex(invDragStartIndex, 1);
            dragStartIndex = -1;
        }
    }

    void OnPopupYes(int dropIndex, int inventoryIndex, Item item, int maxCount)
    {
        // 입력값 검증 및 파싱
        int count = ParseCountOrDefault(currentCount.text, 1, maxCount);
        int price = ParsePriceOrDefault(currentPrice.text, item.sellPrice);

        RegisterItemToSlot(dropIndex, item, count, price);
        inventory.DecreaseItemAtIndex(inventoryIndex, count);

        dragStartIndex = -1;

        Debug.Log($"[RegisteredItem] {dropIndex}번 슬롯에 등록: {item.itemName} x{count}, 가격 {price}");
    }

    void OnPopupNo()
    {
        CancelDrag();
        Debug.Log("[RegisteredItem] 아이템 등록 취소됨");
    }

    // 수량 파싱 (1 ~ maxCount 범위)
    int ParseCountOrDefault(string text, int minValue, int maxValue)
    {
        if (int.TryParse(text, out int value))
        {
            return Mathf.Clamp(value, minValue, maxValue);
        }
        return minValue;
    }

    // 가격 파싱 (1 ~ MAX_PRICE 범위)
    int ParsePriceOrDefault(string text, int defaultValue)
    {
        const int MAX_PRICE = 999999;

        if (int.TryParse(text, out int value))
        {
            return Mathf.Clamp(value, 1, MAX_PRICE);
        }

        return Mathf.Clamp(defaultValue, 1, MAX_PRICE);
    }

    // 아이템 등록
    void RegisterItemToSlot(int slotIndex, Item item, int count, int price)
    {
        if (slotIndex < 0 || slotIndex >= capacity)
        {
            Debug.LogError($"[RegisteredItem] 잘못된 슬롯 인덱스: {slotIndex}");
            return;
        }

        if (item == null)
        {
            Debug.LogError("[RegisteredItem] 등록할 아이템이 null입니다");
            return;
        }

        if (count <= 0)
        {
            Debug.LogError($"[RegisteredItem] 잘못된 수량: {count}");
            return;
        }

        registeredItemsData[slotIndex] = new RegisteredItemData(item, count, price);
        itemList[slotIndex] = item;

        SetupSlotImage(slotIndex, item);

        Debug.Log($"[RegisteredItem] {slotIndex}번 슬롯 등록 완료: {item.itemName} x{count}, 가격 {price}");
    }

    //창고 슬롯이 아닌 곳에 Drop했을 때
    public void CancelDrag()
    {
        dragStartIndex = -1;    //드래그 상태 초기화        
    }

    /// <summary>
    /// 특정 아이템의 현재 설정된 판매가 반환
    /// 손님 AI가 이 가격으로 평가함
    /// </summary>
    public int GetCurrentPrice(Item item)
    {
        if (item == null)
        {
            Debug.LogWarning("[RegisteredItem] GetCurrentPrice - item이 null입니다");
            return 0;
        }

        // registeredItemsData 검증
        if (registeredItemsData == null || registeredItemsData.Length == 0)
        {
            Debug.LogWarning("[RegisteredItem] registeredItemsData가 비어있습니다");
            return item.sellPrice;
        }

        for (int i = 0; i < registeredItemsData.Length; i++)
        {
            if (registeredItemsData[i] != null &&
                registeredItemsData[i].item != null &&
                registeredItemsData[i].item.itemID == item.itemID)
            {
                Debug.Log($"[RegisteredItem] GetCurrentPrice: {item.itemName} = {registeredItemsData[i].price}");
                return registeredItemsData[i].price;
            }
        }

        Debug.LogWarning($"[RegisteredItem] GetCurrentPrice: {item.itemName}을 찾지 못함. sellPrice 반환");
        return item.sellPrice;
    }

    /// <summary>
    /// 특정 아이템의 판매가 설정 (UI에서 직접 바꾸고 싶을 때)
    /// </summary>
    public void SetCurrentPrice(Item item, int price)
    {
        if (item == null)
        {
            Debug.LogError("[RegisteredItem] SetCurrentPrice - item이 null입니다");
            return;
        }

        if (registeredItemsData == null || registeredItemsData.Length == 0)
        {
            Debug.LogError("[RegisteredItem] registeredItemsData가 비어있습니다");
            return;
        }

        for (int i = 0; i < registeredItemsData.Length; i++)
        {
            if (registeredItemsData[i] != null &&
                registeredItemsData[i].item != null &&
                registeredItemsData[i].item.itemID == item.itemID)
            {
                registeredItemsData[i].price = Mathf.Clamp(price, 1, 999999);
                Debug.Log($"[RegisteredItem] SetCurrentPrice: {item.itemName} = {registeredItemsData[i].price}");
                return;
            }
        }

        Debug.LogWarning($"[RegisteredItem] SetCurrentPrice: {item.itemName}을 찾지 못함");
    }

    public Item[] GetRegisteredItems()
    {
        return itemList;
    }

    public RegisteredItemData[] GetAllRegisteredItemsData()
    {
        return registeredItemsData;
    }
}



