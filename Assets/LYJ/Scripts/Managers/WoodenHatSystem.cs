using UnityEngine;

/// <summary>
/// 나무 모자 전용 시스템 (물약 제작 + 무기 강화)
/// 부모: CraftingSystemBase
/// </summary>
public class WoodenHatSystem : CraftingSystemBase
{
    public static WoodenHatSystem Instance { get; private set; }

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
    /// 나무 모자 레시피 초기화 (물약 제작 + 강화)
    /// </summary>
    private void InitializeRecipes()
    {
        // ========== 제작 레시피 (물약) ==========

        // 체력 포션 제작
        recipes["recipe_health_potion"] = new CraftRecipe
        {
            recipeID = "recipe_health_potion",
            recipeName = "체력 포션",
            resultItemID = "item_health_potion",
            resultItemName = "체력 포션",
            craftCost = 300,
            recipeType = RecipeType.Craft,
            requiredMaterials = new CraftRequirement[]
            {
                new CraftRequirement { itemID = "material_herb", quantity = 2 },
                new CraftRequirement { itemID = "material_water", quantity = 1 }
            }
        };

        // 마나 포션 제작
        recipes["recipe_mana_potion"] = new CraftRecipe
        {
            recipeID = "recipe_mana_potion",
            recipeName = "마나 포션",
            resultItemID = "item_mana_potion",
            resultItemName = "마나 포션",
            craftCost = 400,
            recipeType = RecipeType.Craft,
            requiredMaterials = new CraftRequirement[]
            {
                new CraftRequirement { itemID = "material_flower", quantity = 3 },
                new CraftRequirement { itemID = "material_crystal", quantity = 1 }
            }
        };

        // 영약 제작
        recipes["recipe_elixir"] = new CraftRecipe
        {
            recipeID = "recipe_elixir",
            recipeName = "영약",
            resultItemID = "item_elixir",
            resultItemName = "영약",
            craftCost = 800,
            recipeType = RecipeType.Craft,
            requiredMaterials = new CraftRequirement[]
            {
                new CraftRequirement { itemID = "material_herb", quantity = 5 },
                new CraftRequirement { itemID = "material_crystal", quantity = 2 }
            }
        };

        // ========== 강화 레시피 ==========

        // 기본 검 강화
        recipes["recipe_enhance_basic_sword"] = new CraftRecipe
        {
            recipeID = "recipe_enhance_basic_sword",
            recipeName = "기본 검 강화",
            resultItemID = "item_basic_sword",
            resultItemName = "기본 검 (강화)",
            craftCost = 600,
            recipeType = RecipeType.Enhance,
            requiredMaterials = new CraftRequirement[]
            {
                new CraftRequirement { itemID = "material_crystal", quantity = 2 }
            }
        };

        // 강화된 검 강화
        recipes["recipe_enhance_enhanced_sword"] = new CraftRecipe
        {
            recipeID = "recipe_enhance_enhanced_sword",
            recipeName = "강화된 검 강화",
            resultItemID = "item_enhanced_sword",
            resultItemName = "강화된 검 (강화)",
            craftCost = 1200,
            recipeType = RecipeType.Enhance,
            requiredMaterials = new CraftRequirement[]
            {
                new CraftRequirement { itemID = "material_crystal", quantity = 3 },
                new CraftRequirement { itemID = "material_ore", quantity = 2 }
            }
        };

        // 창 강화
        recipes["recipe_enhance_spear"] = new CraftRecipe
        {
            recipeID = "recipe_enhance_spear",
            recipeName = "창 강화",
            resultItemID = "item_spear",
            resultItemName = "창 (강화)",
            craftCost = 900,
            recipeType = RecipeType.Enhance,
            requiredMaterials = new CraftRequirement[]
            {
                new CraftRequirement { itemID = "material_crystal", quantity = 2 }
            }
        };

        Debug.Log($"[WoodenHatSystem] {recipes.Count}개의 레시피 로드됨 " +
                  $"(제작: 3개, 강화: 3개)");
    }

    public override void ResetRecipes()
    {
        recipes.Clear();
        InitializeRecipes();
    }
}

