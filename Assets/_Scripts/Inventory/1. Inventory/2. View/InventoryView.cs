using System;
using UnityEngine;

[RequireComponent(typeof(InventoryView))]
public class InventoryView : MonoBehaviour
{
    [Header("정렬 버튼 아이콘")]
    public SortIconListener sortIcon;

    public GameObject slotPrefab;

    [Header("UI 슬롯")]
    public Transform safeSlotArea;      //세이프티 슬롯
    public Transform deleteSlotArea;    //딜리트 슬롯

    private InventorySlotView[] uiSlots;

    //Presenter에게 클릭 신호 전달
    public event Action<int> OnSlotClicked;
    public event Action OnSortRequest;
    
    private void Start()
    {
        if (sortIcon != null)
        {
            //정렬 아이콘 클릭 시 매니저 호출
            sortIcon.OnClick += () => OnSortRequest?.Invoke();
        }
    }

    //초기화: 슬롯 UI 생성 (Array 기반)
    public void CreateSlots(int capacity)
    {
        foreach (Transform child in safeSlotArea) Destroy(child.gameObject);
        foreach (Transform child in deleteSlotArea) Destroy(child.gameObject);

        uiSlots = new InventorySlotView[capacity];

        for (int i = 0; i < capacity; i++)
        {
            Transform targetParent = (i < 5) ? safeSlotArea : deleteSlotArea;

            GameObject go = Instantiate(slotPrefab, targetParent);
            InventorySlotView slotView = go.GetComponent<InventorySlotView>();

            slotView.Initialize(i);
            slotView.OnSlotClick += (idx) => OnSlotClicked?.Invoke(idx);

            uiSlots[i] = slotView;
        }
    }

    //갱신: Model에서 변환된 배열을 받아 UI 업데이트
    public void RefreshAll(InventorySlotModel[] dataSlots)
    {
        for (int i = 0; i < uiSlots.Length; i++)
        {
            uiSlots[i].UpdateView(dataSlots[i]);            
        }
    }
}