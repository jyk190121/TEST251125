using UnityEngine;

public class QuickSlotPresenter : MonoBehaviour
{
    public static QuickSlotPresenter Instance;

    //퀵슬롯 1개만 사용하므로 0번 인덱스 고정
    private const int SLOT_INDEX = 0;
    private const int CAPACITY = 1;

    [Header("View 연결")]
    public QuickSlotView quickSlotView; //단일 슬롯 연결

    public QuickSlotModel model;
    private InventoryModel inventoryModel;
    private SoundManager soundManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        model = new QuickSlotModel(CAPACITY);

        soundManager = FindAnyObjectByType<SoundManager>();
    }

    private void Start()
    {
        //인벤토리 변경 감지 구독 (포션 쓰면 퀵슬롯 숫자도 줄어들게)
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryUpdated += RefreshUI;
        }

        //View 초기화
        if (quickSlotView != null)
        {
            quickSlotView.Initialize(SLOT_INDEX);
        }

        LoadQuickSlotFromDataManager();

        RefreshUI();
    }

    private void Update()
    {
        //숫자 1번 키 입력 시 아이템 사용
        if (Input.GetKeyDown(KeySetting.keys[KeyInput.QUICKSLOT]))
        {
            UseQuickSlotItem(SLOT_INDEX);
        }
    }

    //아이템 등록
    public void RegisterItem(int index, Item newItem)
    {
        //포션이 아니면 등록 거절
        if (newItem.type != ItemType.Potion)
        {
            Debug.Log("퀵슬롯에는 포션만 등록할 수 있습니다.");
            return;
        }

        //현재 드래그 중인 아이템의 인덱스 확인 (인벤토리에서 가져옴)
        int inventroyIndex = InventoryManager.Instance.GetDragStartIndex();

        //드래그 중이 아니면 취소
        if (inventroyIndex == -1) return;

        //퀵슬롯에 아이템이 있다면
        Item oldItem = model.GetItem(index);
        if (oldItem != null)
        {
            //기존 아이템이 있으면 인벤토리로 돌려보냄
            InventoryManager.Instance.AddItem(oldItem, 0);
        }

        //인벤토리에 새로운 아이템 등록
        model.SetQickSlot(index, newItem);

        //인벤토리에서 아이템 제거
        //inventoryModel.DecreaseItemAmount(index,1);

        //드래그 상태 종료
        InventoryManager.Instance.CancelDrag();

        //장착 사운드 재생
        soundManager.PlaySFX("시우", 4);

        //UI 갱신
        RefreshUI();
    }

    //아이템 해제
    //public void UnregisterItem(int index)
    //{
    //    model.ClearQickSlot(index);
    //    RefreshUI();
    //}

    public void UnregisterItem(int index)
    {
        Item item = model.GetItem(index);
        if (item == null) return;

        //인벤토리로 복귀 시도
        //bool added = InventoryManager.Instance.AddItem(item);
        bool added = true;

        //인벤토리에 자리가 있어서 잘 들어갔다면 퀵슬롯 비우기
        if (added)
        {
            model.ClearQickSlot(index);            

            //장착 해제 사운드 재생
            int soundIndex = Random.Range(5, 7);
            soundManager.PlaySFX("시우", soundIndex);

            RefreshUI();
        }
        else
        {
            Debug.Log("인벤토리가 꽉 차서 해제할 수 없습니다.");
        }
    }


    //아이템 사용 (소모 + 효과)    
    public void UseQuickSlotItem(int slotIndex)
    {
        Item item = model.GetItem(slotIndex);

        //아이템이 있고, 인벤토리에 수량이 있다면
        if (item != null && InventoryManager.Instance.GetItemCount(item) > 0)
        {
            Debug.Log($"퀵슬롯 사용: {item.itemName}");
            //인벤토리에서 이 아이템이 "몇 번째 칸"에 있는지 찾기
            int realInventoryIndex = InventoryManager.Instance.FindInventoryIndex(item);

            //찾은 인덱스가 유효하다면(-1이 아니라면) 사용
            if (realInventoryIndex != -1)
            {
                Debug.Log($"퀵슬롯 사용: {item.itemName} (인벤토리 {realInventoryIndex}번 슬롯 사용)");

                //퀵슬롯 번호(slotIndex)가 아니라, 진짜 인벤토리 번호(realInventoryIndex)를 넘겨줌
                InventoryManager.Instance.UseItem(realInventoryIndex);
            }
            else
            {

                Debug.Log("오류: 아이템 수량은 있는데 인벤토리에서 찾을 수 없습니다.");
            }
        }
    }

    //화면 갱신 (인벤토리 수량 확인)
    public void RefreshUI()
    {
        Item item = model.GetItem(SLOT_INDEX);

        if (item != null)
        {
            //인벤토리에 몇 개 있는지 실시간 확인
            int count = InventoryManager.Instance.GetItemCount(item);
            quickSlotView.UpdateSlotView(item, count);

            // DataManager 동기화
            _MasterManager.Instance.DataManager.QuickSlotItem = item;
        }
        else
        {
            quickSlotView.UpdateSlotView(null, 0);
            _MasterManager.Instance.DataManager.QuickSlotItem = null;
        }
    }

    //데이터 로드
    private void LoadQuickSlotFromDataManager()
    {
        DataManager dm = _MasterManager.Instance.DataManager;

        if (dm.QuickSlotItem != null)
        {
            model.SetQickSlot(SLOT_INDEX, dm.QuickSlotItem);
        }

        RefreshUI();
    }
}
