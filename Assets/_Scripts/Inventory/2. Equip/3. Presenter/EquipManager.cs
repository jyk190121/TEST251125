using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class EquipManager : MonoBehaviour
{
    public static EquipManager Instance;

    [Header("UI 연결")]
    public EquipSlotView[] uiSlots;     //슬롯 UI 4개 연결 (0:무기, 1:머리, 2:몸, 3:발)
    public EquipSlotView[] uiSlots_2;   //Result 장비창
    public GameObject equipPanel;
    public GameObject equipPanel_2;

    //실제 데이터를 관리하는 모델 객체
    public EquipModel model;

    //효과음 재생
    SoundManager soundManager;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        //모델 생성 (데이터 초기화)
        model = new EquipModel();

        //SoundManager 인스턴스 할당
        soundManager = FindAnyObjectByType<SoundManager>();

        DataManager.OnDataLoaded += OnDataLoadedHandler;
    }

    private void Start()
    {
        if (equipPanel_2 == null)
        {
            var allSlots = FindAnyObjectByType<EquipSlotView>();
        }

        // 새 게임이면 빈 슬롯 표시
        if (!SaveManager.HasSaveData())
        {
            RefreshUI();
        }
    }

    // 데이터 로드 완료 시 호출
    private void OnDataLoadedHandler()
    {
        Debug.Log("[EquipManager] 데이터 로드 이벤트 수신!");
        LoadEquipmentFromDataManager();
    }

    //====================================================
    //장착 로직 (인벤토리 -> 장비창)
    //====================================================

    //인벤토리에서 더블클릭 등으로 장착을 시도할 때 호출
    public (bool success, Item unequippedItem) TryEquipItem(Item newItem)
    {
        if (newItem.type != ItemType.Equipment) return (false, null);

        int targetIndex = GetSlotIndexByEnum(newItem.equipmentSlot);

        EquipSound(newItem);

        if (targetIndex != -1)
        {
            // 장착 실행하고, 벗은 아이템을 받아옴
            Item oldItem = EquipItemToSlot(targetIndex, newItem);

            // 성공했음과 벗은 아이템을 같이 보고
            return (true, oldItem);
        }
        return (false, null);
    }

    //드래그 앤 드롭으로 장착할 때 호출 (View에서 호출됨)
    public void OnDropToEquipSlot(int slotIndex, EquipmentSlot requiredType)
    {
        //인벤토리 매니저에게 "지금 드래그 중인 아이템 내놔" 라고 요청
        Item draggedItem = InventoryManager.Instance.GetDraggedItem();
        if (draggedItem == null) return;

        //아이템 타입 검사 (예: 투구 슬롯에 무기를 넣으려고 하면 거절)
        if (draggedItem.type != ItemType.Equipment) return;
        if (draggedItem.equipmentSlot != requiredType)
        {
            Debug.Log("장비 타입이 맞지 않습니다.");
            return;
        }

        Item oldItem = EquipItemToSlot(slotIndex, draggedItem);

        //장착 실행
        EquipItemToSlot(slotIndex, draggedItem);

        //인벤토리에서 해당 아이템 소모(삭제) 처리
        //(단순 삭제가 아니라 장착 처리를 위해 인벤토리에서 빼는 함수)
        InventoryManager.Instance.UseItemForEquip();

        if (oldItem != null)
        {
            InventoryManager.Instance.AddItem(oldItem);
        }
    }

    private Item EquipItemToSlot(int index, Item newItem)
    {
        Item oldItem = model.GetEquip(index);
        model.SetEquip(index, newItem);        

        RefreshUI();
        //UpdateStatToPlayer();

        // 벗은 아이템을 반환 (없으면 null)
        return oldItem;
    }

    void EquipSound(Item item)
    {
        if (item.equipmentType == EquipmentType.Weapon)
        {
            int soundIndex = Random.Range(1, 3);
            soundManager.PlaySFX("시우", soundIndex);
        }
        else
        {
            soundManager.PlaySFX("시우", 3);
        }
    }

    //====================================================
    //해제 로직 (장비창 -> 인벤토리)
    //====================================================

    public void UnEquipItem(int slotIndex)
    {
        //해당 슬롯에 아이템이 있는지 확인
        Item item = model.GetEquip(slotIndex);
        if (item == null) return;

        //인벤토리로 복귀 시도
        //(인벤토리가 꽉 찼으면 해제 불가능하게 처리)
        //AddItem은 성공 여부(bool)를 반환한다고 가정
        bool addedToInventory = InventoryManager.Instance.AddItem(item);

        if (addedToInventory) //인벤토리에 잘 들어갔다면
        {
            //모델에서 장비 제거
            model.Unequip(slotIndex);

            // 갱신 (RefreshUI가 모든 업데이트를 처리합니다)
            RefreshUI();
        }
        else
        {
            Debug.Log("인벤토리가 꽉 차서 장비를 해제할 수 없습니다.");
        }
    }

    //====================================================
    //보조 기능
    //====================================================

    // Enum 타입을 배열 인덱스로 변환하는 함수
    private int GetSlotIndexByEnum(EquipmentSlot type)
    {
        switch (type)
        {
            case EquipmentSlot.Weapon: return EquipModel.SLOT_WEAPON;   //0
            case EquipmentSlot.Head: return EquipModel.SLOT_HEAD;       //1
            case EquipmentSlot.Body: return EquipModel.SLOT_BODY;       //2
            case EquipmentSlot.Foot: return EquipModel.SLOT_FOOT;       //3             
            default: return -1;
        }
    }

    //모든 슬롯 UI를 모델 데이터에 맞춰 다시 그림
    public void RefreshUI()
    {
        //UI 슬롯 업데이트
        Item[] currentEquips = model.GetAllEquips();
        for (int i = 0; i < uiSlots.Length; i++)
        {
            if (i < currentEquips.Length)
            {
                uiSlots[i].UpdateSlot(currentEquips[i]);

                if (equipPanel_2 == null) continue;
                if (uiSlots_2 == null) continue;
                uiSlots_2[i].UpdateSlot(currentEquips[i]);
            }
        }

        //DataManager에 모든 장비 정보 업데이트
        DataManager dm = _MasterManager.Instance.DataManager;
        Item currentWeapon = model.GetEquip(EquipModel.SLOT_WEAPON);

        //DataManager의 필드를 업데이트
        dm.EquipWeapon = currentWeapon;
        dm.ChangeWeapon(currentWeapon); //무기 변경 이벤트 호출

        dm.EquipHead = model.GetEquip(EquipModel.SLOT_HEAD);
        dm.EquipBody = model.GetEquip(EquipModel.SLOT_BODY);
        dm.EquipFoot = model.GetEquip(EquipModel.SLOT_FOOT);
    }

    //데이터 매니저에서 데이터 가져오기
    public void LoadEquipmentFromDataManager()
    {
        DataManager dm = _MasterManager.Instance.DataManager;

        // 장비 데이터가 비어있으면 아무것도 하지 않음
        if (dm.EquipWeapon == null && dm.EquipHead == null &&
            dm.EquipBody == null && dm.EquipFoot == null)
        {
            // 새 게임이거나 아직 로드 안 됨
            return;
        }

        if (dm.EquipWeapon != null)
        {
            model.SetEquip(EquipModel.SLOT_WEAPON, dm.EquipWeapon);
        }

        if (dm.EquipHead != null)
        {
            model.SetEquip(EquipModel.SLOT_HEAD, dm.EquipHead);
        }

        if (dm.EquipBody != null)
        {
            model.SetEquip(EquipModel.SLOT_BODY, dm.EquipBody);
        }

        if (dm.EquipFoot != null)
        {
            model.SetEquip(EquipModel.SLOT_FOOT, dm.EquipFoot);
        }

        RefreshUI();
    }

    //플레이어 스탯 매니저에게 변경된 수치 전달
    //private void UpdateStatToPlayer()
    //{
    //    int currentWeaponID = model.GetEquip(EquipModel.SLOT_WEAPON) != null ?
    //    model.GetEquip(EquipModel.SLOT_WEAPON).itemID : -1;

    //    //DataManager에 전달
    //    if(_MasterManager.Instance != null)
    //    {
    //        _MasterManager.Instance.DataManager.SetCurrentWeapon(currentWeaponID);
    //    }
    //}

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        DataManager.OnDataLoaded -= OnDataLoadedHandler;
    }
}