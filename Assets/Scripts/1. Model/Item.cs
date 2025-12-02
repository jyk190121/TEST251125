using UnityEngine;

<<<<<<< Updated upstream:Assets/Scripts/1. Model/Item.cs
=======
public enum ItemType
{
    Material,           //재료
    Equipment,          //장비
    Potion              //포션
}

>>>>>>> Stashed changes:Assets/_Scripts/Inventory/1. Inventory/1. Model/Item.cs
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item Data")]
public class Item : ScriptableObject
{
    [Header("기본 정보")]
<<<<<<< Updated upstream:Assets/Scripts/1. Model/Item.cs
    public int ID;                  //아이템 고유 ID
=======
    public int itemID;              //아이템 고유 ID
>>>>>>> Stashed changes:Assets/_Scripts/Inventory/1. Inventory/1. Model/Item.cs
    public string itemName;         //아이템 이름
    public Sprite icon;             //아이콘 이미지
    [TextArea(3, 5)]
    public string description;      //아이템 설명

    [Header("스택(겹치기) 설정")]    
    public bool isStackable;        //스택화 가능 유무 (포션, 재료 등)
<<<<<<< Updated upstream:Assets/Scripts/1. Model/Item.cs
    public int maxStackSize = 99;   //최대 수량
=======
    public int maxStack = 99;       //최대 수량
>>>>>>> Stashed changes:Assets/_Scripts/Inventory/1. Inventory/1. Model/Item.cs

    [Header("데이터")]
    public int buyPrice;            //구매가
    public int sellPrice;           //판매가

    // (필요하다면) 아이템 사용 시 효과 등을 위한 메서드도 추가 가능
<<<<<<< Updated upstream:Assets/Scripts/1. Model/Item.cs
}
=======

    [Header("아이템 타입")]
    public ItemType type;

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
public enum EquipmentType { Weapon, Armor }                     //장비 타입(무기/방어구)
public enum EquipmentSlot { Weapon, Head, Body, Foot }          //장비 슬롯

[System.Serializable]
public struct MaterialCost
{
    public Item materialItem;
    public int amount;
}
>>>>>>> Stashed changes:Assets/_Scripts/Inventory/1. Inventory/1. Model/Item.cs
