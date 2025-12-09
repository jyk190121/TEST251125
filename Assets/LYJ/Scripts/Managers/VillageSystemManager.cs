using UnityEngine;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;

/// <summary>
/// 마을의 시설 관리와 상태를 담당하는 Manager
/// </summary>
public class VillageSystemManager : MonoBehaviour
{
    public static VillageSystemManager Instance { get; private set; }

    [System.Serializable]
    public class Facility
    {
        public string facilityID;
        public string facilityName;
        public int unlockCost;
        public bool isUnlocked;
    }

    private Dictionary<string, Facility> facilities = new Dictionary<string, Facility>();

    public event Action<string> OnFacilityUnlocked;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        Initialize();
    }

    /// <summary>
    /// 초기 시설 데이터 설정
    /// </summary>
    private void Initialize()
    {
        // 대장간 (무기 제작)
        facilities["smithy"] = new Facility
        {
            facilityID = "smithy",
            facilityName = "벌컨의 대장간",
            unlockCost = 500,
            isUnlocked = false
        };

        // 나무 모자 (물약 제작 + 강화)
        facilities["wooden_hat"] = new Facility
        {
            facilityID = "wooden_hat",
            facilityName = "나무 모자",
            unlockCost = 500,
            isUnlocked = false
        };
    }

    public bool IsFacilityUnlocked(string facilityID)
    {
        if (facilities.TryGetValue(facilityID, out var facility))
        {
            return facility.isUnlocked;
        }
        return false;
    }

    public bool TryUnlockFacility(string facilityID)
    {
        if (!facilities.TryGetValue(facilityID, out var facility))
        {
            Debug.LogError($"[VillageSystemManager] 존재하지 않는 시설: {facilityID}");
            return false;
        }

        if (facility.isUnlocked)
        {
            Debug.LogWarning($"[VillageSystemManager] 이미 해금된 시설: {facilityID}");
            return false;
        }

        int money = _MasterManager.Instance.DataManager.HojuMoney();
        int cost = GetFacilityUnlockCost(facilityID);

        if ( money >= cost )
        {
            facility.isUnlocked = true;
            OnFacilityUnlocked?.Invoke(facilityID);

            Debug.Log($"[VillageSystemManager] 시설 해금: {facility.facilityName}");
            return true;
        }
        else
        {
            Debug.Log($"[VillageSystemManager] 골드 부족");
            return false;
        }
    }

    public int GetFacilityUnlockCost(string facilityID)
    {
        if (facilities.TryGetValue(facilityID, out var facility))
        {
            return facility.unlockCost;
        }
        return -1;
    }

    public string GetFacilityName(string facilityID)
    {
        if (facilities.TryGetValue(facilityID, out var facility))
        {
            return facility.facilityName;
        }
        return "Unknown";
    }

    public Dictionary<string, Facility> GetAllFacilities()
    {
        return new Dictionary<string, Facility>(facilities);
    }

    public void ResetFacilities()
    {
        Initialize();
    }
}
