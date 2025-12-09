using UnityEngine;

/// <summary>
/// 대장간 전용 제작 시스템 (무기 제작만)
/// 부모: CraftingSystemBase
/// </summary>
public class SmithySystem : CraftingSystemBase
{
    public static SmithySystem Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeRecipes();
    }

    /// <summary>
    /// 대장간 레시피 초기화 (무기 제작만)
    /// </summary>
    private void InitializeRecipes()
    {
        // 기본 검 제작
        recipes["recipe_basic_sword"] = new CraftRecipe
        {
            recipeID = "recipe_basic_sword",
            recipeName = "기본 검",
            resultItemID = "item_basic_sword",
            resultItemName = "기본 검",
            craftCost = 500,
            recipeType = RecipeType.Craft,
            requiredMaterials = new CraftRequirement[]
            {
                new CraftRequirement { itemID = "material_iron", quantity = 3 },
                new CraftRequirement { itemID = "material_wood", quantity = 2 }
            }
        };

        // 강화된 검 제작
        recipes["recipe_enhanced_sword"] = new CraftRecipe
        {
            recipeID = "recipe_enhanced_sword",
            recipeName = "강화된 검",
            resultItemID = "item_enhanced_sword",
            resultItemName = "강화된 검",
            craftCost = 1000,
            recipeType = RecipeType.Craft,
            requiredMaterials = new CraftRequirement[]
            {
                new CraftRequirement { itemID = "material_iron", quantity = 5 },
                new CraftRequirement { itemID = "material_crystal", quantity = 1 }
            }
        };

        // 창 제작
        recipes["recipe_spear"] = new CraftRecipe
        {
            recipeID = "recipe_spear",
            recipeName = "창",
            resultItemID = "item_spear",
            resultItemName = "창",
            craftCost = 800,
            recipeType = RecipeType.Craft,
            requiredMaterials = new CraftRequirement[]
            {
                new CraftRequirement { itemID = "material_iron", quantity = 4 },
                new CraftRequirement { itemID = "material_wood", quantity = 3 }
            }
        };

        Debug.Log($"[SmithySystem] {recipes.Count}개의 레시피 로드됨");
    }

    public override void ResetRecipes()
    {
        recipes.Clear();
        InitializeRecipes();
    }
}

