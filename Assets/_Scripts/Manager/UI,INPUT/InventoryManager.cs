using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[RequireComponent(typeof(InventoryManager))]
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("설정")]
    public int capacity = 20;           //인벤토리 크기

    [Header("View 연결")]
    public GameObject equipView;
    public QuickSlotView quickSlotView;
    public WarehouseView warehouseView;
    public InventoryView inventoryView;
    public RegisteredItem itemView;
    public Inventory inventory;         //인벤토리창 On/Off

    [Header("테스트용 아이템 연결")]
    public Item testItemA;              //인스펙터에서 아이템(임시) 연결
    public Item testItemB;              //인스펙터에서 아이템(임시) 연결
    public Item testItemC;              //인스펙터에서 아이템(임시) 연결
    public Item testItemD;              //인스펙터에서 아이템(임시) 연결
    public Item testItemE;              //인스펙터에서 아이템(임시) 연결

    //Model (Inspector에 안 보임)
    private InventoryModel model;

    [Header("드래그 상태")]
    //드래그 시작한 슬롯 번호 (-1: 아무것도 안 잡음)
    private int dragStartIndex = -1;
    
    //아이템 정렬 순서 변수
    private int currentSortIndex = 0;

    //클릭할 때마다 바뀔 정렬 타입 순서
    //0: Material, 1: Weapon, 2: Potion
    private readonly ItemType[] sortOrder = new ItemType[]
    {
        ItemType.Material,
        ItemType.Equipment,
        ItemType.Potion
    };

    [Header("UI 영역 설정")]
    public RectTransform inventoryPanelRect;    //인벤토리 배경 (이 밖으로 나가면 팝업)
    public ItemDropPopup dropPopup;             //팝업창 스크립트
    public ItemSplitPopup splitPopup;           //아이템 소분팝업    

    private void Awake()
    {
        if (Instance == null) Instance = this;
 
        else Destroy(gameObject);

        //Model 생성
        model = new InventoryModel(capacity);

        //View 초기화
        inventoryView.CreateSlots(capacity);

        //이벤트 연결 (Model -> View)
        //모델 데이터가 변하면 -> HandleInventoryUpdate 실행
        model.OnInventoryUpdated += HandleInventoryUpdate;

        //이벤트 연결 (View -> Logic)
        //슬롯이 클릭되면 -> HandleSlotClick 실행
        inventoryView.OnSlotClicked += HandleSlotClick;

        //정렬 버튼이 클릭되면 -> HandleSortSequence 실행
        inventoryView.OnSortRequest += HandleSortSequence;

    }

    private void Start()
    {
        //시작 시 초기화
        HandleInventoryUpdate();
        dropPopup.ClosePopup();
        //inventory = transform.GetChild(0).gameObject.GetComponent<Inventory>();i
        model.InitSlots(capacity);
    }

    //임시 아이템 업로드 코드
    private void Update()
    {
        //A키를 누르면 테스트 아이템 A 획득
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            if (testItemA != null)
            {
                AddItem(testItemA);
                Debug.Log("아이템 획득: " + testItemA.itemName);
            }
        }

        //B키를 누르면 테스트 아이템 B 획득
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            if (testItemB != null)
            {
                AddItem(testItemB);
                Debug.Log("아이템 획득: " + testItemB.itemName);
            }
        }

        //C키를 누르면 테스트 아이템 C 획득
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            if (testItemB != null)
            {
                AddItem(testItemC);
                Debug.Log("아이템 획득: " + testItemC.itemName);
            }
        }

        //D키를 누르면 테스트 아이템 D 획득
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            if (testItemB != null)
            {
                AddItem(testItemD);
                Debug.Log("아이템 획득: " + testItemD.itemName);
            }
        }

        //C키를 누르면 테스트 아이템 E 획득
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            if (testItemB != null)
            {
                AddItem(testItemE);
                Debug.Log("아이템 획득: " + testItemE.itemName);
            }
        }

        //인벤토리창 열고 닫기
        if (Input.GetKeyDown(KeySetting.keys[KeyInput.INVENTORY]))
        {
            inventory.gameObject.SetActive(!inventory.gameObject.activeSelf);
        }

        //창고, 아이템 등록UI 상태에 따라 다른 UI 패널(장비, 퀵슬롯) 활성화 상태 관리
        if (warehouseView != null && itemView != null && equipView != null && quickSlotView != null)
        {
            bool isWarehouseActive = warehouseView.gameObject.activeSelf;
            bool isItemRegiActive = itemView.gameObject.activeSelf;

            //창고나 아이템 등록UI가 활성화 상태면 장비창과 퀵슬롯을 비활성화
            //창고나 아이템 등록UI가 비활성화 상태면, 퀵슬롯은 활성화하고 장비창은 인벤토리의 활성화 상태에 따름
            equipView.SetActive(!isItemRegiActive && !isWarehouseActive && inventory.gameObject.activeSelf);
            quickSlotView.gameObject.SetActive(!isItemRegiActive && !isWarehouseActive);
        }
        
        //마우스 버튼을 뗐는데(Up) && 드래그 중이라면(dragStartIndex != -1)
        if (Input.GetMouseButtonUp(0) && dragStartIndex != -1)
        {
            //팝업창이 꺼져있을 때만 강제로 종료 처리
            //팝업이 켜져 있다면, 유저의 응답을 기다려야 하므로 건드리지 않음
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

    //드래그 아이템을 쓰레기통으로
    public void OnDropToTrash()
    {
        //드래그 중인 아이템이 없으면 취소
        if (dragStartIndex == -1) return;

        //쓰레기통에 아이템을 드래그 앤 드롭하면 바로 삭제
        model.RemoveItem(dragStartIndex);
        Debug.Log("쓰레기통에 버려 삭제되었습니다.");

        //팝업
        //ShowDropPopup();
        //Debug.Log("쓰레기통에 버려 삭제되었습니다.");


        //처리가 끝났으니 드래그 상태 초기화        
        dragStartIndex = -1;

        //화면 갱신
        model.NotifyUpdate();
    }

    //드래그 끝(드롭) 시 호출
    public void OnDragEnd(int dropIndex)
    {
        if (dragStartIndex != -1)
        {
            //인벤토리 슬롯이 아닌 곳(-1)에 Drop 했을 때
            if (dropIndex == -1)
            {
                //마우스가 인벤토리 패널 안에 있는지 확인
                if (IsMouseOverInventoryPanel())
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

        //창고에서 인벤토리로 드롭했을 때
        if (WarehousePresenter.Instance.GetDragStartIndex() != -1)
        {
            if (dropIndex == -1) return;

            //Warehouse에서 드래그한 아이템을 인벤토리에 넣기
            Item warehouseItem = WarehousePresenter.Instance.GetDraggedItem();
            int warehouseCount = WarehousePresenter.Instance.GetDraggedItemCount();
            int warehouseIndex = WarehousePresenter.Instance.GetDragStartIndex();

            //소분 수량이 1보다 많으면 실행
            if (warehouseCount > 1)
            {
                splitPopup.OpenPopup(warehouseItem, warehouseItem.itemName, warehouseCount, (amount) =>
                {
                    ProcessMoveFromWarehouse(dropIndex, warehouseIndex, warehouseItem, amount);
                });
            }
            else
            {
                //1개면 바로 이동
                ProcessMoveFromWarehouse(dropIndex, warehouseIndex, warehouseItem, 1);
            }
        }

    }


    //창고로 아이템 이동
    private void ProcessMoveFromWarehouse(int dropIndex, int sourceWarehouseIndex, Item warehouseItem, int warehouseCount)
    {
        if (warehouseItem == null) return;

        //인벤토리의 해당 슬롯에 있던 아이템 (교체용)
        var slots = model.GetSlotsForView();
        InventorySlotModel targetSlot = model.GetSlotsForView()[dropIndex];
        Item inventoryItem = targetSlot.IsEmpty ? null : targetSlot.itemData;
        int inventoryItemCount = targetSlot.quantity;

        int totalMovedAmount = 0;   //이동한 총 수량

        //인벤토리에 같은 아이템이 있으면 수량만 증가 (창고에서 가져오는 아이템이 스택형일 때)
        if (inventoryItem != null && inventoryItem.itemID == warehouseItem.itemID)
        {
            //남은 공간 계산 (최대 수량 - 현재 수량)
            int spceLeft = inventoryItem.maxStack - inventoryItemCount;

            //첫 번째 슬롯에 넣을 양
            int amountToFirsSlot = Mathf.Min(spceLeft, warehouseCount);

            if (amountToFirsSlot > 0)
            {
                //인벤토리에서 옮길 수 있는 수량 계산
                //int amountToMove = Mathf.Min(spceLeft, warehouseCount);

                //인벤토리 슬롯 수량 증가
                model.AddItemToSlot(dropIndex, new InventorySlotModel
                {
                    itemData = warehouseItem,
                    quantity = inventoryItemCount + amountToFirsSlot
                });

                totalMovedAmount += amountToFirsSlot;
            }

            //남은 수량 빈 칸 찾아 넣기
            int remaining = warehouseCount - amountToFirsSlot;

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
                    model.AddItemToSlot(emptySlotIndex, new InventorySlotModel
                    {
                        itemData = warehouseItem,
                        quantity = remaining
                    });

                    //이동 성공 기록 추가
                    totalMovedAmount += remaining;
                }
                else
                {
                    Debug.Log("인벤토리 공간이 부족하여 일부만 이동했습니다.");
                }
            }
        }

        //빈 슬롯일 경우
        else if (inventoryItem == null)
        {
            model.AddItemToSlot(dropIndex, new InventorySlotModel
            {
                itemData = warehouseItem,
                quantity = warehouseCount
            });
            totalMovedAmount = warehouseCount;
        }

        //다른 아이템일 경우
        else
        {
            //창고 자리에 아이템이 있으면 인벤토리로 보내기
            WarehousePresenter.Instance.AddItem(inventoryItem, inventoryItemCount);

            //창고에 아이템 추가
            model.AddItemToSlot(dropIndex, new InventorySlotModel
            {
                itemData = warehouseItem,
                quantity = warehouseCount
            });

            totalMovedAmount = warehouseCount;
        }

        if (totalMovedAmount > 0)
        {
            //인벤토리 아이템 감소            
            WarehousePresenter.Instance.DecreaseItemAtIndex(sourceWarehouseIndex, warehouseCount);
        }

        // 화면 갱신 및 드래그 종료
        if (WarehousePresenter.Instance.GetDragStartIndex() != -1)
            WarehousePresenter.Instance.CancelDrag();

        //창고 화면 갱신
        model.NotifyUpdate();
    }


    //인벤토리에서 특정 아이템의 인덱스 찾기 
    public int FindInventoryIndex(Item item)
    {
        if (model == null) return -1;
        return model.FindItemIndex(item);
    }

    //아이템 사용/장착 시 호출
    public void UseItem(int index)
    {        
        //모델에서 해당 인덱스 아이템 데이터 가져오기
        var slots = model.GetSlotsForView();
        Debug.Log($"인벤토리 아이템 사용 시도: 인덱스 {index}");

        //인덱스 범위 체크
        if (index < 0 || index >= slots.Length) return;

        var targetSlot = slots[index];

        //빈 슬롯이면 취소        
        if (targetSlot.IsEmpty)
        {
            Debug.Log($"빈슬롯");
            return;
        }

        Item item = targetSlot.itemData;        

        //창고 뷰가 활성화되어 있으면 리턴
        if (warehouseView != null && warehouseView.gameObject.activeSelf)
        {

            return;
        }

        //아이템 타입에 따라 분기 처리        

        //장비 아이템
        if (item.type == ItemType.Equipment)
        {
            //장착 시도하고 결과 받아오기 (성공여부, 벗은아이템)
            var result = EquipManager.Instance.TryEquipItem(item);

            if (result.success)
            {
                //벗은 아이템이 있다면 "방금 사용한 그 자리(index)"에 넣기 (Swap)
                if (result.unequippedItem != null)
                {
                    //새 슬롯 데이터 생성
                    InventorySlotModel swapSlot = new InventorySlotModel();
                    swapSlot.Set(result.unequippedItem, 1);

                    //AddItem(검색) 대신 AddItemToSlot(강제 주입) 사용
                    model.AddItemToSlot(index, swapSlot);

                    //화면 갱신
                    model.NotifyUpdate();
                }
                //빈 슬롯이었다면 그냥 삭제
                else
                {
                    model.RemoveItem(index);
                }
            }
        }

        //소비 아이템 (포션 등)
        else if (item.type == ItemType.Potion)
        {
            //체력이 MAX상태인지 확인
            //MatserManager를 통해 플레이어 정보 접근
            if (_MasterManager.Instance == null) return;

            PlayerModel playerStat = _MasterManager.Instance.DataManager.GetStat();
            if (playerStat.HP >= playerStat.MaxHP)
            {
                Debug.Log("채력이 이미 가득 찼습니다");
                return;
            }

            //체력 회복
            //item.healAmount 만큼 회복
            _MasterManager.Instance.DataManager.AddHP(item.healAmount);
            Debug.Log($"{item.itemName}을(를) 사용하여 체력을 {item.healAmount}만큼 회복했습니다.");

            //아이템 수량 감소
            //InventoryModel의 수량 감소 함수 호출
            model.DecreaseItemAmount(index, 1);
        }
    }

    //외부(퀵슬롯)에서 현재 드래그 시작 인덱스 조회 함수
    public int GetDragStartIndex()
    {
        return dragStartIndex;
    }

    //현재 드래그 중인 아이템 데이터를 반환하는 함수 (장비창에서 쓰기 위함)
    public Item GetDraggedItem()
    {
        if (dragStartIndex == -1) return null;
        var slots = model.GetSlotsForView();
        return slots[dragStartIndex].itemData;
    }

    //현재 드래그 중인 아이템 수량 반환 함수 (창고에서 쓰기 위함)
    public int GetDraggedItemCount()
    {
        if (dragStartIndex == -1) return 0;
        return model.GetSlotsForView()[dragStartIndex].quantity;
    }

    //드래그 중인 아이템 수량 감소 함수 (창고에서 쓰기 위함)
    public void DecreaseDraggedItemAmount(int amount)
    {
        if (dragStartIndex == -1) return;
        //모델에게 해당 슬롯에서 amount만큼 감소시키라고
        model.DecreaseItemAmount(dragStartIndex, amount);        
    }

    //인덱스 기반 수량 감소 (창고에서 쓰기 위함)
    public void DecreaseItemAtIndex(int index, int amount)
    {
        model.DecreaseItemAmount(index, amount);
    }

    //특정 인덱스의 아이템을 장착 및 창고 저장 때문에 삭제하는 함수 (단순 삭제와 다름)
    public void UseItemForEquip()
    {
        if (dragStartIndex != -1)
        {
            model.RemoveItem(dragStartIndex);
            dragStartIndex = -1;
            model.NotifyUpdate();
        }
    }


    //마우스가 인벤토리 패널 위에 있는지 판별하는 함수
    private bool IsMouseOverInventoryPanel()
    {
        //RectTransformUtility가 마우스 좌표가 네모 칸 안에 있는지 검사해줌
        return RectTransformUtility.RectangleContainsScreenPoint(
            inventoryPanelRect,
            Input.mousePosition
        );
    }

    private void ShowDropPopup()
    {
        //현재 잡고 있는 아이템 데이터 가져오기
        var slots = model.GetSlotsForView();

        //안전장치: 인덱스가 이상하면 취소
        if (dragStartIndex < 0 || dragStartIndex >= slots.Length) return;

        var itemToDrop = slots[dragStartIndex].itemData;

        //팝업 열기 (아이템 이름, YES 행동, NO 행동 전달)
        dropPopup.OpenPopup(
            itemToDrop.itemName,
            //YES 눌렀을 때: 아이템 삭제
            onYes: () => {
                model.RemoveItem(dragStartIndex); // 모델에서 삭제
                dragStartIndex = -1;              // 드래그 상태 초기화
            },
            //NO 눌렀을 때: 드래그 취소 (제자리 복귀)
            onNo: () => {
                CancelDrag();
            }
        );
    }

    //인벤토리 슬롯이 아닌 곳에 Drop했을 때
    public void CancelDrag()
    {
        dragStartIndex = -1;    //드래그 상태 초기화
        model.NotifyUpdate();   //화면을 원래대로 복구
    }

    //인덱스 두 개를 받아서 데이터를 교환
    private void SwapItems(int indexA, int indexB)
    {
        var slots = model.GetSlotsForView();

        //임시 변수(temp)를 이용해 데이터 교환
        InventorySlotModel temp = slots[indexA];
        slots[indexA] = slots[indexB];
        slots[indexB] = temp;

        //바뀐 내용을 모델(딕셔너리)에 저장
        model.AddItemToSlot(indexA, slots[indexA]);
        model.AddItemToSlot(indexB, slots[indexB]);

        //화면 갱신
        model.NotifyUpdate();
    }

    //데이터 변경 시 호출되는 콜백 함수
    private void HandleInventoryUpdate()
    {
        //Model의 딕셔너리를 배열로 변환해서 View에 전달
        inventoryView.RefreshAll(model.GetSlotsForView());
    }

    //슬롯 클릭 시 호출 (우클릭 등 나중에 사용)
    private void HandleSlotClick(int index)
    {
        var slots = model.GetSlotsForView();
        var clickedSlot = slots[index];

        if (!clickedSlot.IsEmpty)
        {
            Debug.Log($"[{index}] 아이템 선택: {clickedSlot.itemData.itemName}");
        }
    }

    //아이템 정렬 아이콘 클릭될 때마다 실행
    private void HandleSortSequence()
    {
        //현재 순서에 맞는 타입 호출
        ItemType targetType = sortOrder[currentSortIndex];

        Debug.Log($"[{currentSortIndex + 1}번째 클릭] {targetType} 위주로 정렬합니다.");

        //모델에게 정렬
        model.SortInventory(targetType);

        //화면 갱신
        model.NotifyUpdate();

        //다음 순서
        currentSortIndex = (currentSortIndex + 1) % sortOrder.Length;
    }

    //외부에서 아이템 획득 시 호출
    public bool AddItem(Item item, int count = 1)
    {
        return model.AddItem(item, count);
        //return true;
    }

    //플레이어가 죽었을 때 호출
    public void OnPlayerDeath()
    {
        //가장 위 5칸을 제외한 모든 인벤토리 아이템 제거
        int safeSlotCount = 5;
        model.RemoveSomeItemsOnDeath(safeSlotCount);
    }

    //외부에서 아이템 사용 시 호출 (장비 강화, 소모품 사용 등)
    public void ConsumeItem(int itemID, int count)
    {
        model.RemoveItemByCount(itemID, count);
    }

    //외부에서 아이템 ID 값 안내 시 호출
    public int GetItemCount(Item item)
    {
        //InventoryModel null 체크
        if (model == null) return 0;

        //Model을 호출하여 아이템 아이디 및 수량 확인
        return model.GetItemCount(item.itemID);
    }

    public event Action OnInventoryUpdated
    {
        add { model.OnInventoryUpdated += value; }
        remove { model.OnInventoryUpdated -= value; }
    }

    // SaveManager에서 호출할 수 있도록
    public InventorySlotModel[] GetSlotsForView()
    {
        return model.GetSlotsForView();
    }

    public void Initialize()
    {
        Debug.Log("[InventoryManager] 초기화 시작");

        // Model 생성 (Awake에서 이미 생성되지만, 안전을 위해)
        if (model == null)
        {
            model = new InventoryModel(capacity);
            Debug.LogWarning("[InventoryManager] Model이 null이었습니다. 새로 생성합니다.");
        }

        // View 초기화
        if (inventoryView != null)
        {
            inventoryView.CreateSlots(capacity);
            Debug.Log("[InventoryManager] InventoryView 초기화됨");
        }
        else
        {
            Debug.LogError("[InventoryManager] InventoryView가 연결되지 않았습니다!");
        }

        // 이벤트 연결 (이미 Awake에서 했지만, 안전을 위해 다시)
        model.OnInventoryUpdated += HandleInventoryUpdate;
        if (inventoryView != null)
        {
            inventoryView.OnSlotClicked += HandleSlotClick;
            inventoryView.OnSortRequest += HandleSortSequence;
        }

        // 슬롯 초기화
        model.InitSlots(capacity);

        // 화면 갱신
        HandleInventoryUpdate();

        // 팝업 닫기
        if (dropPopup != null) dropPopup.ClosePopup();
        if (splitPopup != null) splitPopup.gameObject.SetActive(false);

        Debug.Log("[InventoryManager] 초기화 완료");
    }

}