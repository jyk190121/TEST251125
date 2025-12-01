using UnityEngine;

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

    //Model (Inspector에 안 보임)
    private InventoryModel model;    
    
    [Header("드래그 상태")]
    //드래그 시작한 슬롯 번호 (-1: 아무것도 안 잡음)
    private int dragStartIndex = -1;

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

        if (Input.GetMouseButtonUp(0))
        {
            if (dragStartIndex != -1)
            {
                // 상태 초기화
                dragStartIndex = -1;

                // 화면 강제 갱신 (이때 SlotView의 UpdateView가 호출되면서 고스트가 삭제됨)
                model.NotifyUpdate();
                Debug.Log("드래그 강제 종료 (안전장치 발동)");
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

        //처리가 끝났으니 드래그 상태 초기화        
        dragStartIndex = -1;

        //화면 갱신
        model.NotifyUpdate();
    }

    //드래그 끝(드롭) 시 호출
    public void OnDragEnd(int dropIndex)
    {
        //인벤토리 슬롯 이외에 Drop하면 리셋
        if (dropIndex == -1)
        {
            dragStartIndex = -1;
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

    //인벤토리 슬롯이 아닌 곳에 Drop했을 때
    public void CancelDrag()
    {
        //드래그 상태 초기화
        dragStartIndex = -1;       
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

        if (clickedSlot.IsEmpty)
        {
            Debug.Log($"[{index}] 빈 슬롯입니다.");
        }
        else
        {
            Debug.Log($"[{index}] 아이템 선택: {clickedSlot.itemData.itemName}");
        }
    }

    //외부에서 아이템 획득 시 호출
    public void AddItem(Item item, int count = 1)
    {
        model.AddItem(item, count);
    }

    //외부에서 아이템 사용 시 호출 (장비 강화, 소모품 사용 등)
    public void ConsumeItem(int itemID, int count)
    {
        model.RemoveItemByCount(itemID, count);
    }


    //외부에서 아이템 ID 값 안내 시 호출
    public int GetItemCount(Item itemID)
    {
        //InventoryModel null 체크
        if (model == null) return 0;

        //Model을 호출하여 아이템 아이디 및 수량 확인
        return GetItemCount(itemID);
    }
}