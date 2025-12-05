using System;
using UnityEngine;

public class InventoryView : MonoBehaviour
{
    public GameObject slotPrefab;
    public Transform contentArea; //Grid Layout Group이 있는 부모

    private InventorySlotView[] uiSlots;

    //Presenter에게 클릭 신호 전달
    public event Action<int> OnSlotClicked;

    //초기화: 슬롯 UI 생성 (Array 기반)
    public void CreateSlots(int capacity)
    {
        foreach (Transform child in contentArea) Destroy(child.gameObject);

        uiSlots = new InventorySlotView[capacity];

        for (int i = 0; i < capacity; i++)
        {
            GameObject go = Instantiate(slotPrefab, contentArea);
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