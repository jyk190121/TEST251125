using UnityEngine;

public class EquipManager : MonoBehaviour
{
    public static EquipManager Instance;

    [Header("UI 연결")]
    public EquipSlotView[] uiSlots; //슬롯 UI 4개 연결 (0:무기, 1:머리, 2:몸, 3:발)
    public GameObject equipPanel;

    //실제 데이터를 관리하는 모델 객체
    public EquipModel model;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        //모델 생성 (데이터 초기화)
        model = new EquipModel();
    }

    private void Start()
    {
        //게임 시작 시 UI를 한 번 그려줍니다.
        RefreshUI();

        //저장된 장비 복구
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

    //실제 데이터 교체 및 스왑 처리
    //private void EquipItemToSlot(int index, Item newItem)
    //{
    //    //기존에 끼고 있던 아이템이 있는지 확인
    //    Item oldItem = model.GetEquip(index);

    //    //모델 데이터 갱신 (새 아이템 장착)
    //    model.SetEquip(index, newItem);

    //    //기존 아이템이 있었다면 인벤토리로 되돌려줌 (스왑)
    //    if (oldItem != null)
    //    {
    //        InventoryManager.Instance.AddItem(oldItem);            
    //    }

    //    //UI 및 스탯 갱신
    //    RefreshUI();
    //    //UpdateStatToPlayer();
    //}

    private Item EquipItemToSlot(int index, Item newItem)
    {
        Item oldItem = model.GetEquip(index);
        model.SetEquip(index, newItem);

        // [삭제] 여기서 인벤토리로 보내던 코드를 지웁니다! (InventoryManager.Instance.AddItem...)
        // 이유는? 인벤토리 매니저가 직접 제어하게 하기 위해서입니다.

        RefreshUI();
        //UpdateStatToPlayer();

        // 벗은 아이템을 반환 (없으면 null)
        return oldItem;
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
            
            // 갱신
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
        Item[] currentEquips = model.GetAllEquips();
        for (int i = 0; i < uiSlots.Length; i++)
        {
            if (i < currentEquips.Length)
            {
                uiSlots[i].UpdateSlot(currentEquips[i]);
            }
        }
        _MasterManager.Instance.DataManager.ChangeWeapon(model.GetEquip(EquipModel.SLOT_WEAPON));        
    }

    //데이터 매니저에 저장 데이터 전달
    private void LoadEquipmentFromDataManager()
    {
        DataManager dm = _MasterManager.Instance.DataManager;

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
}