using UnityEngine;

/// <summary>
/// 대장간 전용 제작 시스템 (무기 제작)
/// 부모: CraftingSystemBase
/// </summary>
public class SmithySystem : CraftingSystemBase
{
    public static SmithySystem Instance { get; private set; }

    [Header("대장간 레시피")]
    [Tooltip("에디터에서 Recipe SO들을 할당")]
    public SmithyRecipe[] SmithyRecipes;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        InitializeRecipes();
    }

    /// <summary>
    /// 대장간 레시피 초기화
    /// </summary>
    public override void InitializeRecipes()
    {
        recipes.Clear();

        if (SmithyRecipes == null || SmithyRecipes.Length == 0)
        {
            Debug.LogWarning("[SmithySystem] 할당된 SmithyRecipe가 없습니다");
            return;
        }

        foreach (var recipe in SmithyRecipes)
        {
            if (recipe != null)
            {
                recipes[recipe.recipeID] = recipe;
            }
        }

        Debug.Log($"[SmithySystem] 총 {recipes.Count}개의 레시피 로드됨");
    }

    /// <summary>
    /// 대장간 특수 조건 체크
    /// requiredBaseEquipment 확인 등
    /// </summary>
    public override bool CanCraft(int recipeID)
    {
        if (!recipes.TryGetValue(recipeID, out var recipe))
        {
            Debug.LogError($"[SmithySystem] 존재하지 않는 레시피: {recipeID}");
            return false;
        }

        // 기본 조건 (부모 클래스)
        bool baseConditions = base.CanCraft(recipeID);
        if (!baseConditions)
            return false;

        return true;
    }
}


