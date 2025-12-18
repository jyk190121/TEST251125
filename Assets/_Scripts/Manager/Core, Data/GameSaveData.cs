using UnityEngine;
using System;

/// <summary>
/// 저장/로드용 직렬화 클래스
/// </summary>
[System.Serializable]
public class GameSaveData
{
    // ===== 플레이어 정보 =====
    public int playerMoney;
    public int playerHP;
    public int playerMaxHP;
    public int playerATT;
    public int playerDefend;
    public float playerMoveSpeed;
    public float playerAttackSpeed;

    public int[] equippedItemIDs = new int[4];

    // ===== 시간 정보 =====
    public int currentTimeOfDay;
    public int currentDay;

    // ===== 던전 정보 =====
    public int dungeonCleared;

    // ===== 마을 시설 정보 =====
    public DataManager.FacilityData[] facilities;

    // ===== 인벤토리 정보 =====
    public DataManager.InventorySlotData[] inventorySlots;
    public int quickSlotItemID;

    // ===== 창고 정보 =====
    public DataManager.WarehouseSlotData[] warehouseSlots;
}

