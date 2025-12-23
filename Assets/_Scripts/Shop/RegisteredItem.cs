using NUnit.Framework.Internal.Execution;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

/// <summary>
/// 진열대 관리 시스템
/// - 인벤토리에서 아이템 드래그 -> 진열대 슬롯에 등록
/// - 슬롯별 아이템 + 수량 + 판매가 관리
/// - 손님 AI에게 현재 판매가 제공
/// </summary>
[System.Serializable]
[RequireComponent(typeof(RegisteredItem))]
public class RegisteredItem : MonoBehaviour, IDropHandler
{
    public static RegisteredItem RegiItem;

    public ItemSettingPopup registerPopup;      // 팝업창 (아이템 갯수, 판매가격 설정창)

    Table table;                                //테이블에도 아이템 이미지 업데이트
    public TextMeshProUGUI[] countTxt;          //등록된 아이템 갯수 보여주기
    public TextMeshProUGUI[] priceTxt;          //등록된 아이템 가격 보여주기

    int dragIndex = 0;

    [System.Serializable]
    [RequireComponent(typeof(RegisteredItemData))]
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

    private void Awake()
    {
        if (RegiItem == null) RegiItem = this;
        else Destroy(gameObject);

        gameObject.SetActive(true);
    }

    private void Start()
    {
        inventory = InventoryManager.Instance;
        table = FindAnyObjectByType<Table>();

        if (inventory == null)
        {
            Debug.LogError("[RegisteredItem] InventoryManager.Instance를 찾을 수 없습니다");
            return;
        }

        // 초기화: DataManager에서 진열대 데이터 로드
        LoadDataFromDataManager();

        Debug.Log("[RegisteredItem] 초기화 완료");
    }

    /// <summary>
    /// DataManager에서 진열대 데이터를 받아와서 UI 업데이트
    /// </summary>
    private void LoadDataFromDataManager()
    {
        var dataManager = _MasterManager.Instance.DataManager;

        if (dataManager == null)
        {
            Debug.LogError("[RegisteredItem] DataManager를 찾을 수 없습니다");
            return;
        }

        var loadedData = dataManager.GetRegisteredItems();

        // DataManager에 저장된 데이터가 있으면 사용
        if (loadedData != null && loadedData.Length > 0)
        {
            registeredItemsData = loadedData;
            itemList = new Item[registeredItemsData.Length];

            // 각 슬롯의 UI 업데이트
            for (int i = 0; i < registeredItemsData.Length; i++)
            {
                if (registeredItemsData[i] != null && registeredItemsData[i].item != null)
                {
                    itemList[i] = registeredItemsData[i].item;

                    // UI 텍스트 업데이트
                    countTxt[i].text = registeredItemsData[i].count.ToString();
                    priceTxt[i].text = $"판매가 :{(registeredItemsData[i].price * registeredItemsData[i].count).ToString()}";

                    // 아이콘 업데이트
                    SetupSlotImage(i, registeredItemsData[i].item);
                }
                else
                {
                    // 빈 슬롯 초기화
                    itemList[i] = null;
                    countTxt[i].text = "";
                    priceTxt[i].text = "";

                    if (itemImages != null && i < itemImages.Length && itemImages[i] != null)
                    {
                        itemImages[i].sprite = null;
                        itemImages[i].enabled = false;
                    }
                }
            }

            Debug.Log("[RegisteredItem] DataManager에서 진열대 데이터 로드 완료");
        }
        else
        {
            // DataManager에 데이터가 없으면 초기화
            registeredItemsData = new RegisteredItemData[capacity];
            itemList = new Item[capacity];
            Debug.Log("[RegisteredItem] 진열대 데이터 새로 초기화됨");
        }

        table.UpdateTable();
    }

    void SetupSlotImage(int index, Item item)
    {
        //if (itemImages == null || index < 0 || index >= itemImages.Length)
        //    return;

        //if (item == null)
        //    return;

        ////기존 이미지 제거
        //if (itemImages[index] != null && itemImages[index].transform.childCount > 0)
        //{
        //    foreach (Transform child in itemImages[index].transform)
        //    {
        //        Destroy(child.gameObject);
        //    }
        //}

        ////새로운 이미지 생성
        //GameObject images = new GameObject("Image");
        //images.transform.SetParent(itemImages[index].transform, false);

        //var img = images.AddComponent<Image>();
        //img.sprite = item.icon;
        //img.enabled = true;
        //img.preserveAspect = true;

        //itemImages[index] = img;

        //위의 경우 text도 함께 파괴되어 missing상태 발생하여 수정
        if (item == null) return;
        if (itemImages == null || index < 0 || index >= itemImages.Length) return;

        Image iconImage = itemImages[index]; // 슬롯에 고정된 Image

        iconImage.sprite = item.icon;
        iconImage.enabled = true;
        iconImage.preserveAspect = true;
        table.UpdateTable();

    }
    ////인벤토리에서 드래그 시작
    //public void OnDragStart(int index)
    //{
    //    dragStartIndex = index;
    //    if (inventory != null)
    //        inventory.OnDragStart(index);
    //}

    ////진열대 슬롯에 드롭
    //public void OnDragEnd(int dropIndex)
    //{
    //    // inventory 상태 확인
    //    if (inventory == null)
    //    {
    //        Debug.LogError("[RegisteredItem] inventory가 null입니다");
    //        CancelDrag();
    //        return;
    //    }

    //    int invDragStartIndex = inventory.GetDragStartIndex();

    //    // 인벤토리에서 진열대로 드롭하는 경우만 처리
    //    if (invDragStartIndex == -1)
    //    {
    //        Debug.Log("[RegisteredItem] 인벤토리 드래그가 활성화되지 않음");
    //        return;
    //    }

    //    // dropIndex 범위 확인
    //    if (dropIndex < -1 || dropIndex >= capacity)
    //    {
    //        Debug.LogWarning("[RegisteredItem] 진열대 범위 밖에 드롭됨");
    //        return;
    //    }

    //    if (dropIndex == -1)
    //    {
    //        Debug.Log("[RegisteredItem] 진열대 범위 밖 드롭 - 취소");
    //        return;
    //    }

    //    Item inventoryItem = inventory.GetDraggedItem();
    //    int inventoryCount = inventory.GetDraggedItemCount();

    //    // 아이템 검증
    //    if (inventoryItem == null)
    //    {
    //        Debug.LogError("[RegisteredItem] 드래그 아이템이 null입니다");
    //        return;
    //    }

    //    if (inventoryCount <= 0)
    //    {
    //        Debug.LogError("[RegisteredItem] 드래그 아이템 수량이 0 이하입니다");
    //        return;
    //    }

    //    Debug.Log($"[RegisteredItem] 드래그 시작: {inventoryItem.itemName} x{inventoryCount}");

    //    // 수량이 여러 개면 팝업으로 수량/가격 입력
    //    //if (inventoryCount > 0)
    //    //{
    //    // 팝업에 기본값 세팅
    //    currentImage.sprite = inventoryItem.icon;
    //    currentCount.text = inventoryCount.ToString();
    //    currentPrice.text = inventoryItem.sellPrice.ToString();

    //    registerPopup.OpenPopup(
    //        inventoryItem,
    //        onYes: () =>
    //        {
    //            OnPopupYes(dropIndex, invDragStartIndex, inventoryItem, inventoryCount);
    //        },
    //        onNo: () =>
    //        {
    //            OnPopupNo();
    //        }
    //    );
    //    //}
    //    //else
    //    //{
    //    //    // 1개면 바로 등록 (기본 가격 = sellPrice)
    //    //    RegisterItemToSlot(dropIndex, inventoryItem, 1, inventoryItem.sellPrice);
    //    //    inventory.DecreaseItemAtIndex(invDragStartIndex, 1);
    //    //    dragStartIndex = -1;
    //    //}
    //}

    void OnPopupYes(int dropIndex, int inventoryIndex, Item item, int maxCount)
    {
        // 입력값 검증 및 파싱
        int count = ParseCountOrDefault(currentCount.text, 1, maxCount);
        int price = ParsePriceOrDefault(currentPrice.text, item.sellPrice);

        RegisterItemToSlot(dropIndex, item, count, price);
        inventory.DecreaseItemAtIndex(inventoryIndex, count);

        dragStartIndex = -1;
        table.UpdateTable();

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

        countTxt[slotIndex].text = count.ToString();
        priceTxt[slotIndex].text = $"판매가 :{(price*count).ToString()}";

        SetupSlotImage(slotIndex, item);

        UpdateDataManager();

        Debug.Log($"[RegisteredItem] {slotIndex}번 슬롯 등록 완료: {item.itemName} x{count}, 가격 {price}");
    }

    //아이템 등록 판넬 이외의 곳으로 드롭했을 때
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

        if (registeredItemsData == null || registeredItemsData.Length == 0)
        {
            return 0;
        }

        // itemID로 비교
        for (int i = 0; i < registeredItemsData.Length; i++)
        {
            if (registeredItemsData[i] != null && registeredItemsData[i].item != null)
            {
                if (registeredItemsData[i].item.itemID == item.itemID)
                {
                    Debug.Log($"[RegisteredItem] GetCurrentPrice: {item.itemName} = {registeredItemsData[i].price}");
                    return registeredItemsData[i].price;
                }
            }
        }

        Debug.LogError($"[RegisteredItem] GetCurrentPrice: {item.itemName}을 찾을 수 없습니다!");
        return 0;
    }

    /// <summary>
    /// 이미 진열된 아이템의 판매가 변경 (현재 미사용)
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

                UpdateDataManager();

                Debug.Log($"[RegisteredItem] SetCurrentPrice: {item.itemName} = {registeredItemsData[i].price}");
                return;
            }
        }

        Debug.LogWarning($"[RegisteredItem] SetCurrentPrice: {item.itemName}을 찾지 못함");
    }

    public RegisteredItemData[] GetAllRegisteredItemsData()
    {
        return registeredItemsData;
    }
    public void OnDrop(PointerEventData eventData)
    {
        if (dragIndex == -1) return;

        Item item = InventoryManager.Instance.GetDraggedItem();
        int count = InventoryManager.Instance.GetDraggedItemCount();

        if (item == null) return;
        
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
        if (dragIndex < -1 || dragIndex >= capacity)
        {
            Debug.LogWarning("[RegisteredItem] 아이템 추가할 수 없음");
            return;
        }

        if (dragIndex == -1)
        {
            Debug.Log("[RegisteredItem] 진열대 범위 밖 드롭 - 취소");
            return;
        }

        // 팝업에 기본값 세팅
        currentImage.sprite = item.icon;
        currentCount.text = count.ToString();
        currentPrice.text = item.sellPrice.ToString();

        // 팝업 열기
        registerPopup.OpenPopup(
                item,
                onYes: () =>
                {
                    OnPopupYes(dragIndex, invDragStartIndex, item, count);
                    dragIndex++;
                },
                onNo: () =>
                {
                    OnPopupNo();
                }
            );
    }

    // DataManager에 진열대 데이터 전달
    private void UpdateDataManager()
    {
        var dataManager = _MasterManager.Instance.DataManager;
        if (dataManager != null)
        {
            dataManager.SetRegisteredItems(registeredItemsData);
        }
    }

}



