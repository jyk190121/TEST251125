using Unity.VisualScripting;
using UnityEngine;
using System;
using UnityEditor.Build;

/// <summary>
/// 게임 전체 데이터를 관리하는 Manager
/// </summary>
public class DataManager : MonoBehaviour
{
    // ===== 플레이어 정보 =====
    PlayerModel modelstat;      //장비를 착용하지 않은 기본 스탯
    PlayerModel player;         //플레이어의 정보를 담을 그릇
    

    public Item EquipWeapon;    //장착한 무기
    public Item EquipHead;      //장착한 투구
    public Item EquipBody;      //장착한 갑옷
    public Item EquipFoot;      //장착한 신발

    // ===== 시간 정보 =====
    public DayManager.TimeOfDay currentTime = DayManager.TimeOfDay.Day;
    public int currentDay = 1;

    // ===== 던전 정보 =====
    public int dungeonCleared = 0;

    //사망,던전 클리어, 펜던트 사용 이후의 복귀인가?
    bool isReturn = false;
    bool isPendant = false;
    bool isClear = false;

    // 던전 내의 전투 데이터 저장
    BattleRecord BR;

    // 정지시켜야 할때 사용
    bool noMove = false;

    // ===== 마을 시설 정보 =====
    [System.Serializable]
    public class FacilityData
    {
        public string facilityID;
        public bool isUnlocked;
    }
    public FacilityData[] facilities = new FacilityData[2]
    {
        new FacilityData { facilityID = "smithy", isUnlocked = false },
        new FacilityData { facilityID = "wooden_hat", isUnlocked = false }
    };

    // ===== 인벤토리 정보 =====
    [System.Serializable]
    public class InventoryData
    {
        public InventorySlotData[] slots;
    }

    [System.Serializable]
    public class InventorySlotData
    {
        public int itemID;
        public int quantity;
    }
    public InventoryData inventoryData;
    public Item QuickSlotItem;

    // ===== 창고 정보 =====
    [System.Serializable]
    public class WarehouseSlotData
    {
        public int itemID;
        public int quantity;
    }

    [System.Serializable]
    public class WarehouseData
    {
        public WarehouseSlotData[] slots;
    }

    public WarehouseData warehouseData;
    public WarehouseModel warehouseModel;

    // 진열대 정보
    [System.NonSerialized]
    public RegisteredItem.RegisteredItemData[] registeredItemsData;

    // ===== 이벤트 =====
    public static Action OnDataLoaded;
    public static Action OnEquipmentChanged;
    public static Action OnStatChanged;

    public void Initialize()
    {
        // 기본값 설정
        modelstat = PlayerModel.SetStat();
        player = PlayerModel.SetStat();
         

        EquipWeapon = _MasterManager.Instance.ItemManager.GetItemByID(2000);
        EquipHead = null;
        EquipBody = null;
        EquipFoot = null;

        EquipManager.Instance.LoadEquipmentFromDataManager();

        // 시설 정보 초기화
        if (facilities == null || facilities.Length == 0)
        {
            facilities = new FacilityData[2]
            {
            new FacilityData { facilityID = "smithy", isUnlocked = false },
            new FacilityData { facilityID = "wooden_hat", isUnlocked = false }
            };
            Debug.Log("[DataManager] 시설 정보 초기화됨");
        }

        // 인벤토리 초기화
        if (inventoryData == null)
        {
            inventoryData = new InventoryData();
            inventoryData.slots = new InventorySlotData[20];

            for (int i = 0; i < inventoryData.slots.Length; i++)
            {
                inventoryData.slots[i] = new InventorySlotData();
            }
            Debug.Log("[DataManager] 인벤토리 초기화됨");
        }

        // 창고 초기화
        if (warehouseData == null)
        {
            warehouseModel = new WarehouseModel(30);
            warehouseData = new WarehouseData();
            warehouseData.slots = new WarehouseSlotData[30]; // 창고 슬롯 개수

            for (int i = 0; i < warehouseData.slots.Length; i++)
            {
                warehouseData.slots[i] = new WarehouseSlotData();
            }
            Debug.Log("[DataManager] 창고 초기화됨");
        }

        // 진열대 초기화
        if (registeredItemsData == null)
        {
            registeredItemsData = new RegisteredItem.RegisteredItemData[4];
            Debug.Log("[DataManager] 진열대 데이터 초기화됨");
        }

        // 기타 데이터 초기화
        currentTime = DayManager.TimeOfDay.Day;
        currentDay = 1;
        dungeonCleared = 0;
        isReturn = false;

        Debug.Log("[DataManager] 모든 데이터 초기화 완료");
    }



//플레이어 아이템 장착시 스탯 변경
public void playerStatChanged(StatStruct stat)
    {
        //이후 장비 관련 변수 추가시 수정 필요
        player.HP = modelstat.HP + stat.hp;
        player.MaxHP = modelstat.HP + stat.hp;
        player.ATT = modelstat.ATT + stat.att;
        player.Defend = modelstat.Defend + stat.def;
        player.moveSpeed = modelstat.moveSpeed + stat.spd;
        player.attackSpeed = modelstat.attackSpeed + stat.spd;

        OnStatChanged?.Invoke();
    }

    //체력 회복
    public void AddHP(int amount)
    {
        player.HP += amount;
        if(player.HP > player.MaxHP)
        {
            player.HP = player.MaxHP;
        }
    }
    //체력 감소
    public void MinusHP(int amount)
    {
        player.HP -= amount;
        if(player.HP < 0)
        {
            player.HP = 0;
        }
    }
    //돈 벌었을때
    public void EarnMoney(int amount)
    {
        player.Money += amount;
    }
    //돈 썼을 때
    public void SpendMoney(int amount)
    {
        player.Money -= amount;
    }
    //얼마있냐
    public int HojuMoney()
    {
        return player.Money;
    }

    public PlayerModel GetStat()
    {
        return player;
    }
    //무기 변경시
    public void ChangeWeapon(Item newItem)
    {
        EquipWeapon = newItem;

        // 모든 구독자(PlayerControll 등)에게 변경 사항을 알립니다.
        OnEquipmentChanged?.Invoke();
    }

    public Item GetWeapon()
    {
        return EquipWeapon;
    }

    public void ChangeHP(int amount)
    {
        player.HP -= amount;
    }

    // ===== 던전 =====
    //던전 클리어 정보 갱신
    public void DungeonClear(int clearLevel)
    {
        dungeonCleared = clearLevel;

        // 저장 시점: 던전 클리어 후
        AutoSave();
    }

    public int GetDungeonCleared() => dungeonCleared;

    //True = 포탈 타고 복귀 false = 그냥 아무것도 발생하지 않는 복귀
    public void ChangeReturn(bool Return)
    {
        isReturn = Return;

        if (isReturn)
        {
            BR.OpenResultPanel(isClear, isPendant);
        }
    }
    public bool GetReturn()
    {
        return isReturn;
    }
    public void SetisPendant(bool Pendant)
    {
        isPendant = Pendant;
    }
    public void SetisClear(bool Clear)
    {
        isClear = Clear;
    }

    //전투 기록 초기화
    public void RegisterBattleRecord(BattleRecord newBR)
    {
        BR = newBR;
        Debug.Log("BR등록함!");
    }
    //사냥한 몬스터 값 추가
    public void GetMonster(MonsterData MD)
    {
        BR.AddMonster(MD);
    }
    //얻은 아이템 값 추가
    public void GetItem(Item item)
    {
        BR.AddItem(item);
    }

    // ===== 마을 시설 =====
    public bool IsFacilityUnlocked(string facilityID)
    {
        foreach (var facility in facilities)
        {
            if (facility.facilityID == facilityID)
                return facility.isUnlocked;
        }
        return false;
    }

    public void UnlockFacility(string facilityID)
    {
        foreach (var facility in facilities)
        {
            if (facility.facilityID == facilityID)
            {
                facility.isUnlocked = true;
                Debug.Log($"[DataManager] 시설 해금: {facilityID}");

                // 저장 시점: 시설 해금 후
                AutoSave();
                return;
            }
        }
    }

    // ===== 인벤토리 =====
    public void SyncInventoryData(InventorySlotData[] slots)
    {
        inventoryData.slots = slots;
    }

    // ===== 진열대 =====
    public void SetRegisteredItems(RegisteredItem.RegisteredItemData[] items)
    {
        registeredItemsData = items;
        Debug.Log("[DataManager] 진열대 데이터 업데이트됨");
    }

    public RegisteredItem.RegisteredItemData[] GetRegisteredItems()
    {
        return registeredItemsData;
    }

    public InventoryData GetInventoryData() => inventoryData;

    // ===== 시간 =====
    public void SetTimeOfDay(DayManager.TimeOfDay time)
    {
        currentTime = time;
        // 저장 시점: 시간 변경 후
        AutoSave();
    }

    public void SetCurrentDay(int day)
    {
        currentDay = day;
        // 저장 시점: 날짜 변경 후
        AutoSave();
    }

    // ===== 자동 저장 =====
    private void AutoSave()
    {
        _MasterManager.Instance.SaveManager.SaveGame(this);
    }

}
