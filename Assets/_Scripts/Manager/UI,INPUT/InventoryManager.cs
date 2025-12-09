using System;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using static UnityEditor.Progress;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("설정")]
    public int capacity = 20;           //인벤토리 크기

    [Header("View 연결")]
    public InventoryView inventoryView;

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

    [Header("UI 영역 설정")]
    public RectTransform inventoryPanelRect; //인벤토리 배경 (이 밖으로 나가면 팝업)
    public ItemDropPopup dropPopup;          //팝업창 스크립트

    Inventory inventory;                    //인벤토리창 키고 끄기

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

    }
    
    private void Start()
    {
        //시작 시 초기화
        HandleInventoryUpdate();
        dropPopup.ClosePopup();
        inventory = transform.GetChild(0).gameObject.GetComponent<Inventory>();
    }

    //임시 아이템 업로드 코드
    private void Update()
    {
        //A키를 누르면 테스트 아이템 A 획득
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (testItemA != null)
            {
                AddItem(testItemA);
                Debug.Log("아이템 획득: " + testItemA.itemName);
            }
        }

        //B키를 누르면 테스트 아이템 B 획득
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (testItemB != null)
            {
                AddItem(testItemB);
                Debug.Log("아이템 획득: " + testItemB.itemName);
            }
        }

        //C키를 누르면 테스트 아이템 C 획득
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (testItemB != null)
            {
                AddItem(testItemC);
                Debug.Log("아이템 획득: " + testItemC.itemName);
            }
        }

        //D키를 누르면 테스트 아이템 C 획득
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (testItemB != null)
            {
                AddItem(testItemD);
                Debug.Log("아이템 획득: " + testItemD.itemName);
            }
        }

        //C키를 누르면 테스트 아이템 C 획득
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (testItemB != null)
            {
                AddItem(testItemE);
                Debug.Log("아이템 획득: " + testItemE.itemName);
            }
        }

        if(Input.GetKeyDown(KeyCode.I))
        {
            inventory.gameObject.SetActive(!inventory.gameObject.activeSelf);
        }

        //마우스 버튼을 뗐는데(Up) && 드래그 중이라면(dragStartIndex != -1)
        if (Input.GetMouseButtonUp(0) && dragStartIndex != -1)
        {
            //팝업창이 꺼져있을 때만 강제로 종료 처리
            //팝업이 켜져 있다면, 유저의 응답을 기다려야 하므로 건드리지 않음
            if (dropPopup.gameObject.activeSelf == false)
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
        

        //처리가 끝났으니 드래그 상태 초기화        
        dragStartIndex = -1;       

        //화면 갱신
        model.NotifyUpdate();
    }

    //드래그 끝(드롭) 시 호출
    public void OnDragEnd(int dropIndex)
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

        //아이템 타입에 따라 분기 처리        
        
        //장비 아이템
        if (item.type == ItemType.Equipment)
        {
            //EquipManager의 장착 함수 소출
            bool isEquipped = EquipManager.Instance.TryEquipItem(item);

            if (isEquipped)
            {
                //장착 성공 시 인벤토리에서 해당 슬롯 비우기
                model.RemoveItem(index);
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

    //현재 드래그 중인 아이템 데이터를 반환하는 함수 (장비창 & 창고에서 쓰기 위함)
    public Item GetDraggedItem()
    {
        if (dragStartIndex == -1) return null;
        var slots = model.GetSlotsForView();
        return slots[dragStartIndex].itemData;
    }

    //특정 인덱스의 아이템을 장착 때문에 삭제하는 함수 (단순 삭제와 다름)
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

    //외부에서 아이템 획득 시 호출
    public bool AddItem(Item item, int count = 1)
    {
        model.AddItem(item, count);
        return true;
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

    public void Initialize()
    {

    }
}