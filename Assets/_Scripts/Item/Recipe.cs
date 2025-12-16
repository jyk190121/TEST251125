using UnityEngine;

[CreateAssetMenu(fileName = "NewRecipe", menuName = "Recipe/Base Recipe")]
public class Recipe : ScriptableObject
{
    public int recipeID;
    public string recipeName;

    [Header("필요 재료")]
    public MaterialCost[] inputMaterials;

    [Header("비용")]
    public int goldCost;

    [Header("결과")]
    public Item outputItem;
}

// === 대장간 레시피 ===
[CreateAssetMenu(fileName = "NewSmithyRecipe", menuName = "Recipe/Smithy Recipe")]
public class SmithyRecipe : Recipe
{

}

// === 연금술 레시피 ===
[CreateAssetMenu(fileName = "NewAlchemyRecipe", menuName = "Recipe/Alchemy Recipe")]
public class AlchemyRecipe : Recipe
{
    [Header("연금술 특화")]
    [Tooltip("한 번에 만들 포션 개수")]
    public int quantityPerCraft = 1;
}

// === 인챈트 레시피 ===
[CreateAssetMenu(fileName = "NewEnchantRecipe", menuName = "Recipe/Enchant Recipe")]
public class EnchantRecipe : Recipe
{
    [Header("인챈트 특화")]
    [Tooltip("어떤 장비 타입에 적용 가능한가")]
    public EquipmentType targetEquipmentType;

    [Tooltip("인챈트 레벨 (1~5)")]
    public int enchantLevel;

    [Header("인챈트 효과")]
    public int attackBonus;
    public int defenseBonus;
    public int speedBonus;
}

[System.Serializable]
public struct MaterialCost
{
    public Item materialItem;
    public int amount;
}

