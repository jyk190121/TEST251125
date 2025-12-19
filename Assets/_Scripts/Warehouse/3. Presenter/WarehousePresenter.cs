using System;
using Unity.VisualScripting;
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
    public Transform ghostIconParent;           //고스트 아이콘 부모 패널
    public RectTransform warehousePanelRect;    //창고 배경
    public ItemDropPopup dropPopup;             //팝업창
    public ItemSplitPopup splitPopup;           //아이템 소분팝업

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        //Model 생성 (데이터 로드 실패 시 사용할 기본 모델)
        model = new WarehouseModel(capacity);

        //View 초기화
        warehouseView.CreateSlots(capacity);

        //이벤트 연결은 Start에서 진행

        //이벤트 연결 (Model -> Logic)
        //슬롯이 클릭되면 -> HandleSlotClick 실행
        warehouseView.OnSlotClicked += HandleSlotClick;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        //시작 시 초기화
        dropPopup.ClosePopup();
        splitPopup.ClosePopup();

        //DataManager에서 WarehouseModel 가져오기 (저장 데이터 불러오기)
        var loadedModel = _MasterManager.Instance.DataManager.warehouseModel;

        if (loadedModel != null)
        {
            //불러온 데이터가 있으면 교체
            model = loadedModel;
        }
        else
        {
            //불러온 데이터가 없으면 Awake에서 생성한 모델을 DataManager에 등록
            _MasterManager.Instance.DataManager.warehouseModel = model;
        }

        //최종적으로 결정된 model 객체의 이벤트에 구독
        model.OnWarehouseUpdated += HandleWarehouseUpdate;

        //최종 model 데이터로 UI 갱신
        HandleWarehouseUpdate();
    }

    // Update is called once per frame
    private void Update()
    {
        //마우스 버튼을 땠는데 드래그 중이라면
        if (Input.GetMouseButtonUp(0) && dragStartIndex != -1)
        {
            //드랍 팝업 및 소분팝업이 비활성화 상태라면
            if (dropPopup.gameObject.activeSelf == false && splitPopup.gameObject.activeSelf == false)
            {
                //강제로 드래그 종료 함수 호출 (-1: 인벤토리 밖으로 간주)
                OnDragEnd(-1);
            }
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
        if (dragStartIndex != -1)
        {
            //인벤토리 슬롯이 아닌 곳(-1)에 Drop 했을 때
            if (dropIndex == -1)
            {
                //마우스가 인벤토리 패널 안에 있는지 확인
                if (IsMouseOverWarehousePanel())
                {
                    //안쪽이면 -> 그냥 취소 (원래대로 돌아감)
                    CancelDrag();
                }
                else
                {
                    //바깥쪽이면 -> "버리시겠습니까?" 팝업 띄우기
                    ShowDropPopup();
                }
                return;
            }

            //제자리에 놨으면 취소
            if (dragStartIndex == dropIndex)
            {
                CancelDrag();
                return;
            }

            //교환 실행
            SwapItems(dragStartIndex, dropIndex);

            //기록 초기화
            dragStartIndex = -1;
        }

        //인벤토리에서 창고로 드롭했을 때
        if (InventoryManager.Instance.GetDragStartIndex() != -1)
        {
            if (dropIndex == -1) return;

            //인벤토리 매니저에서 드래그한 아이템 가져오기
            Item inventoryItem = InventoryManager.Instance.GetDraggedItem();
            int inventoryCount = InventoryManager.Instance.GetDraggedItemCount();
            int inventoryIndex = InventoryManager.Instance.GetDragStartIndex();

            if (inventoryItem == null) return;

            //소분 수량이 1보다 많으면 실행
            if (inventoryCount > 1)
            {
                splitPopup.OpenPopup(inventoryItem, inventoryItem.itemName, inventoryCount, (amount) =>
                {
                    ProcessMoveFromInventory(dropIndex, inventoryIndex, inventoryItem, amount);
                });
            }
            else
            {
                //1개면 바로 이동
                ProcessMoveFromInventory(dropIndex, inventoryIndex, inventoryItem, 1);
            }
        }
    }

    private void ProcessMoveFromInventory(int dropIndex, int sourceInventoryIndex, Item inventoryItem, int inventoryCount)
    {
        if (inventoryItem == null) return;

        //창고의 해당 슬롯에 있던 아이템 (교체용)
        var slots = model.GetSlotsForView();
        WarehouseSlotModel targetSlot = model.GetSlotsForView()[dropIndex];
        Item warehouseItem = targetSlot.IsEmpty ? null : targetSlot.itemDate;
        int warehouseItemCount = targetSlot.quantity;

        int totalMovedAmount = 0; //이동한 총 수량

        //창고에 같은 아이템이 있으면 수량만 증가 (스택형 아이템일 경우)
        if (warehouseItem != null && warehouseItem.itemID == inventoryItem.itemID)
        {
            //남은 공간 계산 (최대 수량 - 현재 수량)
            int spaceLeft = warehouseItem.maxStack - warehouseItemCount;

            //첫 번째 슬롯에 넣을 양
            int amountToFirstSlot = Mathf.Min(spaceLeft, inventoryCount);

            if (amountToFirstSlot > 0)
            {
                ////인벤토리에서 옮길 수 있는 수량 계산
                //int amountToMove = Mathf.Min(spaceLeft, inventoryCount);

                //창고 슬롯 수량 증가
                model.AddItemToSlot(dropIndex, new WarehouseSlotModel
                {
                    itemDate = inventoryItem,
                    quantity = warehouseItemCount + amountToFirstSlot
                });

                totalMovedAmount += amountToFirstSlot;
            }

            //남은 수량 빈 칸 찾아 넣기
            int remaining = inventoryCount - amountToFirstSlot;

            if (remaining > 0)
            {
                //빈 슬롯 찾기
                int emptySlotIndex = -1;
                for (int i = 0; i < slots.Length; i++)
                {
                    if (slots[i].IsEmpty)
                    {
                        emptySlotIndex = i;
                        break;
                    }
                }

                if (emptySlotIndex != -1)
                {
                    model.AddItemToSlot(emptySlotIndex, new WarehouseSlotModel
                    {
                        itemDate = inventoryItem,
                        quantity = remaining
                    });
                }
                else
                {
                    Debug.Log("창고 공간이 부족하여 일부만 이동했습니다.");
                }
            }
        }

        //빈 슬롯일 경우
        else if (warehouseItem == null)
        {
            model.AddItemToSlot(dropIndex, new WarehouseSlotModel
            {
                itemDate = inventoryItem,
                quantity = inventoryCount
            });
            totalMovedAmount = inventoryCount;
        }

        //다른 아이템일 경우
        else
        {
            //창고 자리에 아이템이 있으면 인벤토리로 보내기
            InventoryManager.Instance.AddItem(warehouseItem, warehouseItemCount);

            //창고에 아이템 추가
            model.AddItemToSlot(dropIndex, new WarehouseSlotModel
            {
                itemDate = inventoryItem,
                quantity = inventoryCount
            });

            totalMovedAmount = inventoryCount;
        }

        if (totalMovedAmount > 0)
        {
            //인벤토리 아이템 감소            
            InventoryManager.Instance.DecreaseItemAtIndex(sourceInventoryIndex, inventoryCount);
        }

        //인벤토리에서 아이템 삭제
        //InventoryManager.Instance.UseItemForEquip();

        //창고에 아이템 추가
        //int count = InventoryManager.Instance.GetDraggedItemCount();
        //model.AddItemToSlot(dropIndex, new WarehouseSlotModel { itemDate = inventoryItem, quantity = inventoryCount });

        //창고 자리에 아이템이 있으면 인벤토리로 보내기
        //if (warehouseItem != null)
        //{
        //    InventoryManager.Instance.AddItem(warehouseItem, warehouseItemCount);
        //}

        // 화면 갱신 및 드래그 종료
        if (InventoryManager.Instance.GetDragStartIndex() != -1)
            InventoryManager.Instance.CancelDrag();

        //창고 화면 갱신
        model.NotifyUpdate();
    }

    //외부(퀵슬롯)에서 현재 드래그 시작 인덱스 조회 함수
    public int GetDragStartIndex()
    {
        return dragStartIndex;
    }

    //현재 드래그 중인 아이템 데이터를 반환하는 함수 (인벤토리에서 쓰기 위함)
    public Item GetDraggedItem()
    {
        if (dragStartIndex == -1) return null;
        var slots = model.GetSlotsForView();
        return slots[dragStartIndex].itemDate;
    }

    //현재 드래그 중인 아이템 수량 반환 함수 (창고에서 꺼내기 위함)
    public int GetDraggedItemCount()
    {
        if (dragStartIndex == -1) return 0;
        return model.GetSlotsForView()[dragStartIndex].quantity;
    }

    //드래그 중인 아이템 수량 감소 함수 (인벤토리에서 쓰기 위함)
    public void DecreaseDraggedItemAmount(int amount)
    {
        if (dragStartIndex == -1) return;
        //모델에게 해당 슬롯에서 amount만큼 감소시키라고
        model.DecreaseItemAmount(dragStartIndex, amount);
    }

    //인덱스 기반 수량 감소 (인벤토리에서 쓰기 위함)
    public void DecreaseItemAtIndex(int index, int amount)
    {
        model.DecreaseItemAmount(index, amount);
    }

    public void UseItemForMove()
    {
        if (dragStartIndex != -1)
        {
            model.RemoveItem(dragStartIndex);
            dragStartIndex = -1;
            model.NotifyUpdate();
        }
    }

    //마우스가 인벤토리 패널 위에 있는지 판별하는 함수
    private bool IsMouseOverWarehousePanel()
    {
        //RectTransformUtility가 마우스 좌표가 네모 칸 안에 있는지 검사해줌
        return RectTransformUtility.RectangleContainsScreenPoint(
            warehousePanelRect,
            Input.mousePosition
        );
    }

    //창고 슬롯이 아닌 곳에 Drop했을 때
    public void CancelDrag()
    {
        dragStartIndex = -1;    //드래그 상태 초기화        
        model.NotifyUpdate();   //화면을 원래대로 복구
    }

    //아이템을 바닥에 드랍 시 팝업
    private void ShowDropPopup()
    {
        //현재 잡고 있는 아이템 데이터 가져오기
        var slots = model.GetSlotsForView();

        //안전장치: 인덱스가 이상하면 취소
        if (dragStartIndex < 0 || dragStartIndex >= slots.Length) return;

        var itemToDrop = slots[dragStartIndex].itemDate;

        //팝업 열기 (아이템 이름, YES 행동, NO 행동 전달)
        dropPopup.OpenPopup(
            itemToDrop.itemName,
            //YES 눌렀을 때: 아이템 삭제
            onYes: () =>
            {
                model.RemoveItem(dragStartIndex); // 모델에서 삭제
                dragStartIndex = -1;              // 드래그 상태 초기화
            },
            //NO 눌렀을 때: 드래그 취소 (제자리 복귀)
            onNo: () =>
            {
                CancelDrag();
            }
        );
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
    public bool AddItem(Item item, int count)
    {
        model.AddItem(item, count);
        return true;
    }
}
