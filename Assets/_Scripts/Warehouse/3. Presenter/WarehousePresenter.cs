using System;
using UnityEngine;

public class WarehousePresenter : MonoBehaviour
{
    public static WarehousePresenter Instance;

    [Header("설정")]
    public int capacity = 30;   //창고 크기

    [Header("View Panel 연결")]
    public WarehouseView warehouseView;

    //Model 데이터 
    private WarehouseModel model;

    [Header("드래그 상태")]
    //드래그 시작한 슬롯 번호 (-1: 아무것도 안 잡음)
    private int dragStartIndex = -1;

    [Header("UI 영역 설정")]
    public RectTransform WarehousePanelRect;    //창고 배경
    //public ItemDropPopup dropPopup;             //팝업창

    private void Awake()
    {
        //Model 생성
        model = new WarehouseModel(capacity);

        //View 초기화
        warehouseView.CreateSlots(capacity);

        //이벤트 연결 (Model -> View)
        //모델 데이터가 변하면 -> HandleWarehouseUpdate 실행
        model.OnWarehouseUpdated += HandleWarehouseUpdate;

        //이벤트 연결 (Model -> Logic)
        //슬롯이 클릭되면 -> HandleSlotClick 실행
        warehouseView.OnSlotClicked += HandleSlotClick;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        //시작 시 초기화
        HandleWarehouseUpdate();        
    }

    // Update is called once per frame
    private void Update()
    {
        //마우스 버튼을 땠는데 드래그 중이라면
        if (Input.GetMouseButtonUp(0) && dragStartIndex != -1)
        {            
            //드래그 종료 처리
            OnDragEnd(-1);
        }
    }

    //드래그 시작 시 호출
    public void OnDragStart(int index)
    {
        dragStartIndex = index;    
    }

    //드래그 종료 시 호출
    public void OnDragEnd(int dropIndex)
    {
        //인벤토리 슬롯이 아닌 곳(-1)에 Drop 했을 때
        if (dropIndex == -1)
        {
            ////마우스가 인벤토리 패널 안에 있는지 확인
            //if (IsMouseOverInventoryPanel())
            //{
            //    //안쪽이면 -> 그냥 취소 (원래대로 돌아감)
            //    CancelDrag();
            //}
            //else
            //{
            //    //바깥쪽이면 -> "버리시겠습니까?" 팝업 띄우기
            //    ShowDropPopup();
            //}
            //return;
            
            //출발한 적이 없거나(-1), 제자리에 놨으면 취소
            if (dragStartIndex == -1 || dragStartIndex == dropIndex)
            {
                dragStartIndex = -1;
                return;
            }

            //교환 실행
            SwapItems(dragStartIndex, dropIndex);

            //기록 초기화
            dragStartIndex = -1;            
        }        

        else if (InventoryManager.Instance.GetDragStartIndex() != -1)
        {
            if (dropIndex == -1) return;
            
            //인벤토리 매니저에서 드래그한 아이템 가져오기
            Item inventoryItem = InventoryManager.Instance.GetDraggedItem();
            if (inventoryItem == null) return;

            //창고의 해당 슬롯에 있던 아이템 (교체용)
            WarehouseSlotModel targetSlot = model.GetSlotsForView()[dropIndex];
            Item warehouseItem = targetSlot.IsEmpty ? null : targetSlot.itemDate;
            int warehouseQuantity = targetSlot.quantity;

            //인벤토리에서 아이템 삭제
            InventoryManager.Instance.UseItemForEquip();
        }
        
    }

    //창고 슬롯이 아닌 곳에 Drop했을 때
    public void CancelDrag()
    {
        //드래그 상태 초기화
        dragStartIndex = -1;
        //화면을 원래대로 복구
        model.NotifyUpdate();
    }

    //인덱스 두 개를 받아서 데이터를 교환
    private void SwapItems(int indexA, int indexB)
    {
        var slots = model.GetSlotsForView();

        //임시 변수(temp)를 이용해 데이터 교환
        WarehouseSlotModel temp = slots[indexA];
        slots[indexA] = slots[indexB];
        slots[indexB] = temp;

        //바뀐 내용을 모델(딕셔너리)에 저장
        model.AddItemToSlot(indexA, slots[indexA]);
        model.AddItemToSlot(indexB, slots[indexB]);

        //화면 갱신
        model.NotifyUpdate();
    }

    //데이터 변경 시 콜백 호출
    private void HandleWarehouseUpdate()
    {
        //Model에서 배열 형태로 변환된 슬롯 데이터를 받아와 View에 전달        
        //WarehouseSlotModel[] dataSlots = model.GetSlotsForView();
        //warehouseView.RefreshAll(dataSlots);
        warehouseView.RefreshAll(model.GetSlotsForView());
    }

    //슬롯 클릭 시 호출 (우클릭 등 나중에 사용)
    private void HandleSlotClick(int index)
    {
        var slots = model.GetSlotsForView();
        var clickedSlot = slots[index];

        if (!clickedSlot.IsEmpty)
        {
            Debug.Log($"[{index}] 아이템 선택: {clickedSlot.itemDate.itemName}");
        }
    }

    //외부에서 아이템 획득 시 호출
    public bool AddItem(Item item, int count = 1)
    {
        model.AddItem(item, count);
        return true;
    }
}
