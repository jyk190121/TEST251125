using System.IO;
using UnityEngine;
using static DataManager;

public class SaveManager : MonoBehaviour
{
    private static string _savePath;
    private static readonly string SAVE_FILE = "gamedata.json";

    private static string SavePath // 저장 경로 캐싱
    {
        get
        {
            if (string.IsNullOrEmpty(_savePath))
            {
                _savePath = Application.persistentDataPath + "/saves/";
            }
            return _savePath;
        }
    }

    /// <summary>
    /// 게임 저장
    /// </summary>
    public void SaveGame(DataManager dataManager)
    {
        try
        {
            if (!Directory.Exists(SavePath))
                Directory.CreateDirectory(SavePath);

            // DataManager → GameSaveData로 변환
            GameSaveData saveData = ConvertDataManagerToSaveData(dataManager);

            string json = JsonUtility.ToJson(saveData, true);
            string fullPath = Path.Combine(SavePath, SAVE_FILE);
            File.WriteAllText(fullPath, json);

            Debug.Log("게임 저장 완료: " + fullPath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("저장 오류: " + e.Message);
        }
    }

    /// <summary>
    /// 플레이어 데이터 로드
    /// </summary>
    public void LoadGame(DataManager dataManager)
    {
        try
        {
            string fullPath = Path.Combine(SavePath, SAVE_FILE);

            if (!File.Exists(fullPath))
            {
                Debug.Log("저장 데이터가 없습니다: " + fullPath);
                return;
            }

            string json = File.ReadAllText(fullPath);
            GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);

            // GameSaveData → DataManager로 변환 (직접 수정)
            ConvertSaveDataToDataManager(saveData, dataManager);

            Debug.Log("게임 로드 완료: " + fullPath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("로드 오류: " + e.Message);
        }
    }

    /// <summary>
    /// GameSaveData → DataManager 변환
    /// </summary>
    private void ConvertSaveDataToDataManager(GameSaveData saveData, DataManager dm)
    {
        // 플레이어 정보 복구
        var playerStat = dm.GetStat();
        playerStat.Money = saveData.playerMoney;
        playerStat.HP = saveData.playerHP;
        playerStat.MaxHP = saveData.playerMaxHP;
        playerStat.ATT = saveData.playerATT;
        playerStat.Defend = saveData.playerDefend;
        playerStat.moveSpeed = saveData.playerMoveSpeed;
        playerStat.attackSpeed = saveData.playerAttackSpeed;

        // 장비 정보 복구
        if (saveData.equippedItemIDs != null && saveData.equippedItemIDs.Length == 4)
        {
            EquipManager equipManager = EquipManager.Instance;

            if (equipManager != null && equipManager.model != null)
            {
                // 각 슬롯의 장비를 복구
                for (int i = 0; i < saveData.equippedItemIDs.Length; i++)
                {
                    if (saveData.equippedItemIDs[i] != -1)
                    {
                        Item equipItem = _MasterManager.Instance.ItemManager.GetItemByID(saveData.equippedItemIDs[i]);

                        if (equipItem != null)
                        {
                            equipManager.model.SetEquip(i, equipItem);
                            Debug.Log($"[SaveManager] 로드 장비[{i}]: ItemID={saveData.equippedItemIDs[i]}");
                        }
                        else
                        {
                            Debug.LogWarning($"[SaveManager] 장비 ItemID {saveData.equippedItemIDs[i]}를 찾을 수 없습니다!");
                        }
                    }
                }

                // UI 갱신
                equipManager.RefreshUI();
            }
            else
            {
                Debug.LogWarning("[SaveManager] EquipManager가 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning("[SaveManager] 저장된 장비 데이터가 없습니다");
        }


        // 시간 정보 복구
        dm.currentTime = (DayManager.TimeOfDay)saveData.currentTimeOfDay;
        dm.currentDay = saveData.currentDay;

        // 던전 정보 복구
        dm.dungeonCleared = saveData.dungeonCleared;

        // 시설 정보 복구
        dm.facilities = saveData.facilities;

        // 인벤토리 정보 복구
        if (saveData.inventorySlots != null && saveData.inventorySlots.Length > 0)
        {
            // InventoryManager 초기화 (기존 데이터 초기화)
            var inventoryManager = _MasterManager.Instance.InventoryManager;

            // 모든 저장된 슬롯 데이터를 순회
            for (int i = 0; i < saveData.inventorySlots.Length; i++)
            {
                var slotData = saveData.inventorySlots[i];

                // 유효한 아이템 데이터만 처리
                if (slotData.itemID != -1 && slotData.quantity > 0)
                {
                    // itemID로 Item 객체 다시 찾기
                    Item item = _MasterManager.Instance.ItemManager.GetItemByID(slotData.itemID);

                    if (item != null)
                    {
                        // InventoryManager에 아이템 추가
                        inventoryManager.AddItem(item, slotData.quantity);
                    }
                    else
                    {
                        Debug.LogWarning($"[SaveManager] ItemID {slotData.itemID}를 찾을 수 없습니다!");
                    }
                }
            }

            Debug.Log($"[SaveManager] 인벤토리 로드 완료: {saveData.inventorySlots.Length}개 슬롯");
        }
        else
        {
            Debug.LogWarning("[SaveManager] 저장된 인벤토리 데이터가 없습니다");
            // 빈 인벤토리로 초기화
            dm.inventoryData.slots = new InventorySlotData[20];
        }

        // 퀵슬롯 정보 복구
        if (saveData.quickSlotItemID != -1)
        {
            Item quickSlotItem = _MasterManager.Instance.ItemManager.GetItemByID(saveData.quickSlotItemID);

            if (quickSlotItem != null)
            {
                QuickSlotPresenter quickSlotPresenter = QuickSlotPresenter.Instance;

                if (quickSlotPresenter != null && quickSlotPresenter.model != null)
                {
                    quickSlotPresenter.model.SetQickSlot(0, quickSlotItem);
                    quickSlotPresenter.RefreshUI();

                    Debug.Log($"[SaveManager] 로드 퀵슬롯: ItemID={saveData.quickSlotItemID}");
                }
                else
                {
                    Debug.LogWarning("[SaveManager] QuickSlotPresenter가 없습니다.");
                }
            }
            else
            {
                Debug.LogWarning($"[SaveManager] 퀵슬롯 ItemID {saveData.quickSlotItemID}를 찾을 수 없습니다!");
            }
        }
        else
        {
            Debug.Log("[SaveManager] 퀵슬롯이 비어있습니다.");
        }
    
        // 창고 정보 복구
        if (saveData.warehouseSlots != null && saveData.warehouseSlots.Length > 0)
        {
            WarehouseModel warehouseModel = dm.warehouseModel;

            for (int i = 0; i < saveData.warehouseSlots.Length; i++)
            {
                var slotData = saveData.warehouseSlots[i];

                if (slotData.itemID != -1 && slotData.quantity > 0)
                {
                    Item item = _MasterManager.Instance.ItemManager.GetItemByID(slotData.itemID);

                    if (item != null)
                    {
                        warehouseModel.AddItem(item, slotData.quantity);
                    }
                    else
                    {
                        Debug.LogWarning($"[SaveManager] 창고 ItemID {slotData.itemID}를 찾을 수 없습니다!");
                    }
                }
            }

            Debug.Log($"[SaveManager] 창고 로드 완료: {saveData.warehouseSlots.Length}개 슬롯");
        }
        else
        {
            Debug.LogWarning("[SaveManager] 저장된 창고 데이터가 없습니다");
            dm.warehouseData.slots = new DataManager.WarehouseSlotData[30];
        }

        DataManager.OnDataLoaded?.Invoke();

        Debug.Log($"[SaveManager] 데이터 로드 완료");
    }

    // DataManager → GameSaveData 변환
    private GameSaveData ConvertDataManagerToSaveData(DataManager dm)
    {
        var playerStat = dm.GetStat();

        // 장비 저장
        int[] equippedItemIDs = new int[4]
        {
        dm.EquipWeapon != null ? dm.EquipWeapon.itemID : -1,
        dm.EquipHead != null ? dm.EquipHead.itemID : -1,
        dm.EquipBody != null ? dm.EquipBody.itemID : -1,
        dm.EquipFoot != null ? dm.EquipFoot.itemID : -1
        };

        // InventoryManager에서 현재 슬롯 배열 가져오기
        InventorySlotModel[] currentSlots = _MasterManager.Instance.InventoryManager
            .GetSlotsForView();

        // InventorySlotModel[] → InventorySlotData[] 변환
        InventorySlotData[] inventorySlotsToSave = new InventorySlotData[currentSlots.Length];

        for (int i = 0; i < currentSlots.Length; i++)
        {
            if (currentSlots[i] != null && !currentSlots[i].IsEmpty)
            {
                // Item 객체 대신 itemID만 저장
                inventorySlotsToSave[i] = new InventorySlotData
                {
                    itemID = currentSlots[i].itemData.itemID,      // ID만 저장
                    quantity = currentSlots[i].quantity
                };
            }
            else
            {
                // 빈 슬롯
                inventorySlotsToSave[i] = new InventorySlotData
                {
                    itemID = -1,
                    quantity = 0
                };
            }
        }

        // 퀵슬롯 저장
        int quickSlotItemID = -1;

        QuickSlotPresenter quickSlotPresenter = QuickSlotPresenter.Instance;

        if (quickSlotPresenter == null)
        {
            quickSlotPresenter = FindAnyObjectByType<QuickSlotPresenter>();
        }

        if (quickSlotPresenter != null && quickSlotPresenter.model != null)
        {
            Item quickSlotItem = quickSlotPresenter.model.GetItem(0);

            if (quickSlotItem != null)
            {
                quickSlotItemID = quickSlotItem.itemID;
                Debug.Log($"[SaveManager] 저장 퀵슬롯: ItemID={quickSlotItemID}");
            }
        }
        else
        {
            Debug.LogError("[SaveManager] QuickSlotPresenter를 찾을 수 없습니다!");
        }


        // 창고 저장
        DataManager.WarehouseSlotData[] warehouseSlotsToSave = new DataManager.WarehouseSlotData[30];

        if (dm.warehouseModel != null)
        {
            // DataManager의 warehouseModel에서 직접 GetSlotsForView() 호출
            WarehouseSlotModel[] warehouseSlots = dm.warehouseModel.GetSlotsForView();

            for (int i = 0; i < warehouseSlots.Length; i++)
            {
                if (!warehouseSlots[i].IsEmpty)
                {
                    warehouseSlotsToSave[i] = new WarehouseSlotData
                    {
                        itemID = warehouseSlots[i].itemDate.itemID,
                        quantity = warehouseSlots[i].quantity
                    };
                }
                else
                {
                    warehouseSlotsToSave[i] = new WarehouseSlotData
                    {
                        itemID = -1,
                        quantity = 0
                    };
                }
            }
        }

        // GameSaveData에 모든 데이터 담기
        GameSaveData saveData = new GameSaveData
        {
            playerMoney = playerStat.Money,
            playerHP = playerStat.HP,
            playerMaxHP = playerStat.MaxHP,
            playerATT = playerStat.ATT,
            playerDefend = playerStat.Defend,
            playerMoveSpeed = playerStat.moveSpeed,
            playerAttackSpeed = playerStat.attackSpeed,

            equippedItemIDs = equippedItemIDs,

            currentTimeOfDay = (int)dm.currentTime,
            currentDay = dm.currentDay,

            dungeonCleared = dm.dungeonCleared,

            facilities = dm.facilities,
            inventorySlots = inventorySlotsToSave,
            quickSlotItemID = quickSlotItemID,
            warehouseSlots = warehouseSlotsToSave
        };

        return saveData;
    }

    /// <summary>
    /// 저장 데이터 존재 확인
    /// </summary>
    public static bool HasSaveData()
    {
        string fullPath = Path.Combine(SavePath, SAVE_FILE);
        return File.Exists(fullPath);
    }

    /// <summary>
    /// 저장 데이터 삭제
    /// </summary>
    public static void DeleteSaveData()
    {
        try
        {
            string fullPath = Path.Combine(SavePath, SAVE_FILE);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                Debug.Log("저장 파일 삭제 완료: " + fullPath);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("저장 파일 삭제 오류: " + e.Message);
        }
    }

    public void Initialize()
    {
        // 저장 폴더 존재 확인 및 생성
        if (!Directory.Exists(SavePath))
        {
            Directory.CreateDirectory(SavePath);
        }

        Debug.Log("[SaveManager] 초기화 완료");
    }
}
