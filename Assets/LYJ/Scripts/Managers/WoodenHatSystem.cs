using UnityEngine;

/// <summary>
/// 나무 모자 전용 시스템 (물약 제작 + 무기 강화)
/// 부모: CraftingSystemBase
/// </summary>
public class WoodenHatSystem : CraftingSystemBase
{
    public static WoodenHatSystem Instance { get; private set; }

    [Header("포션 제작 레시피")]
    [Tooltip("에디터에서 AlchemyRecipe SO들을 할당")]
    public AlchemyRecipe[] AlchemyRecipes;

    [Header("인챈트 강화 레시피")]
    [Tooltip("에디터에서 EnchantRecipe SO들을 할당")]
    public EnchantRecipe[] EnchantRecipes;

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
    /// 나무 모자 레시피 초기화 (포션 제작 + 인챈트 강화)
    /// </summary>
    public override void InitializeRecipes()
    {
        recipes.Clear();

        // ========== 포션 제작 레시피 ==========
        if (AlchemyRecipes != null && AlchemyRecipes.Length > 0)
        {
            foreach (var recipe in AlchemyRecipes)
            {
                if (recipe != null)
                {
                    recipes[recipe.recipeID] = recipe;
                    Debug.Log($"[WoodenHatSystem] 포션 레시피 로드: {recipe.recipeName}");
                }
            }
        }
        else
        {
            Debug.LogWarning("[WoodenHatSystem] 할당된 AlchemyRecipe가 없습니다");
        }

        // ========== 인챈트 강화 레시피 ==========
        if (EnchantRecipes != null && EnchantRecipes.Length > 0)
        {
            foreach (var recipe in EnchantRecipes)
            {
                if (recipe != null)
                {
                    recipes[recipe.recipeID] = recipe;
                    Debug.Log($"[WoodenHatSystem] 인챈트 레시피 로드: {recipe.recipeName}");
                }
            }
        }
        else
        {
            Debug.LogWarning("[WoodenHatSystem] 할당된 EnchantRecipe가 없습니다");
        }

        Debug.Log($"[WoodenHatSystem] 총 {recipes.Count}개의 레시피 로드됨");
    }

    /// <summary>
    /// 나무 모자 특수 조건 체크
    /// AlchemyRecipe: 기본 조건만 확인
    /// EnchantRecipe: 대상 장비 관련 추가 조건 확인
    /// </summary>
    public override bool CanCraft(int recipeID)
    {
        if (!recipes.TryGetValue(recipeID, out var recipe))
        {
            Debug.LogError($"[WoodenHatSystem] 존재하지 않는 레시피: {recipeID}");
            return false;
        }

        // 기본 조건 (부모 클래스)
        bool baseConditions = base.CanCraft(recipeID);
        if (!baseConditions)
            return false;

        // 포션 제작 레시피 - 추가 조건 없음
        if (recipe is AlchemyRecipe alchemyRecipe)
        {
            return true;
        }

        // 인챈트 강화 레시피 - 추가 조건 확인
        if (recipe is EnchantRecipe enchantRecipe)
        {
            // TODO: EnchantRecipe 특수 조건 확인
            // (targetEquipmentType, requiredCurrentEnchantLevel 등)
            // if (enchantRecipe.requiredCurrentEnchantLevel > 0)
            // {
            //     // 플레이어가 보유한 장비의 현재 인챈트 레벨 확인
            // }

            return true;
        }

        return true;
    }
}
