using UnityEngine;
using System.IO;

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

        if (saveData.equippedWeapon != null)
        {
            Item weapon = saveData.equippedWeapon;
            if (weapon != null)
            {
                dm.EquipWeapon = weapon;
            }
        }

        // 시간 정보 복구
        dm.currentTime = (DayManager.TimeOfDay)saveData.currentTimeOfDay;
        dm.currentDay = saveData.currentDay;

        DataManager.OnDataLoaded?.Invoke();

        // 던전 정보 복구
        dm.dungeonCleared = saveData.dungeonCleared;

        // 시설 정보 복구
        dm.facilities = saveData.facilities;

        // 인벤토리 정보 복구
        if (saveData.inventorySlots != null)
        {
            dm.inventoryData.slots = saveData.inventorySlots;
        }

        Debug.Log($"[SaveManager] 데이터 로드 완료");
    }

    // DataManager → GameSaveData 변환
    private GameSaveData ConvertDataManagerToSaveData(DataManager dm)
    {
        var playerStat = dm.GetStat();
        var weapon = dm.GetWeapon();

        return new GameSaveData
        {
            playerMoney = playerStat.Money,
            playerHP = playerStat.HP,
            playerMaxHP = playerStat.MaxHP,
            playerATT = playerStat.ATT,
            playerDefend = playerStat.Defend,
            playerMoveSpeed = playerStat.moveSpeed,
            playerAttackSpeed = playerStat.attackSpeed,
            equippedWeapon = weapon,

            currentTimeOfDay = (int)dm.currentTime,
            currentDay = dm.currentDay,

            dungeonCleared = dm.dungeonCleared,

            facilities = dm.facilities,
            inventorySlots = dm.inventoryData.slots,
        };
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
