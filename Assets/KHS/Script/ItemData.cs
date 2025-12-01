using UnityEngine;


public enum ItemType
{
    Material,           //재료
    Equipment,          //장비
    Potion              //포션
}


[CreateAssetMenu(fileName = "NewItemData", menuName = "Item/Unified Item Data")]
public class ItemData : ScriptableObject
{
    [Header("기본 정보")]
    public int itemID;    //아이템 고유 ID
    public string itemName; //아이템 이름
    public Sprite icon;     //아이템 아이콘(이미지)
    [TextArea(3, 5)]
    public string description;      //아이템 설명

    [Header("아이템 타입")]
    public ItemType type;

    [Header("스택 가능여부 및 수량")]
    public bool isStackable = true;
    public int maxStack = 99;

    [Header("구매가/판매가")]
    public int buyPrice;
    public int sellPrice;

    [Header("재료")]
    [Tooltip("희귀도")]
    public Rarity rarity;
    [Tooltip("해당 아이템 드랍 몬스터")]
    public string[] dropMonsters;

    [Header("장비")]
    [Tooltip("장비 타입(무기/방어구)")]
    public EquipmentType equipmentType;
    [Tooltip("장비 슬롯")]
    public EquipmentSlot equipmentSlot;

    [Header("아이템 티어")]
    public int tier;

    [Header("추가 능력치")]
    public int attack;
    public int defense;
    public int speed;
    public int hpPlus;

    [Header("장비 만드는데 필요한 코스트")]
    public MaterialCost[] craftRecipe;
    public MaterialCost[] upgradeRecipe;

    [Header("포션")]
    public int healAmount;
    public float coolDown;
}

public enum Rarity { Common, Uncommon, Rare, Epic, Legendary }  //희귀도
public enum EquipmentType { Weapon, Armor }         //장비 타입(무기/방어구)
public enum EquipmentSlot { Weapon, Head, Body, Foot }      //장비 슬롯

[System.Serializable]
public struct MaterialCost
{
    public ItemData materialItem;
    public int amount;
}
