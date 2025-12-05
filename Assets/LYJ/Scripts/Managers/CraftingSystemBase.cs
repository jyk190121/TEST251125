using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// 제작과 강화 시스템의 부모 클래스
/// SmithySystem과 WoodenHatSystem이 상속받음
/// </summary>
public abstract class CraftingSystemBase : MonoBehaviour
{
    public enum RecipeType
    {
        Craft,   // 새 아이템 생성
        Enhance  // 기존 아이템 강화
    }

    [System.Serializable]
    public class CraftRequirement
    {
        public string itemID;
        public int quantity;
    }

    [System.Serializable]
    public class CraftRecipe
    {
        public string recipeID;
        public string recipeName;           // 레시피 이름
        public string resultItemID;
        public string resultItemName;
        public int craftCost;
        public CraftRequirement[] requiredMaterials;
        public RecipeType recipeType;       // Craft 또는 Enhance
    }

    protected Dictionary<string, CraftRecipe> recipes = new Dictionary<string, CraftRecipe>();

    public event Action<string, int> OnCraftingComplete;
    public event Action<string> OnCraftingFailed;

    /// <summary>
    /// 제작 가능한지 확인
    /// </summary>
    public virtual bool CanCraft(string recipeID)
    {
        if (!recipes.TryGetValue(recipeID, out var recipe))
        {
            Debug.LogError($"[CraftingSystemBase] 존재하지 않는 레시피: {recipeID}");
            return false;
        }

        // 재료 확인 (InventoryManager와 연동)
        bool hasMaterials = CheckMaterials(recipe);

        // 골드 확인 (DataManager와 연동)
        bool hasGold = true; // 임시

        return hasMaterials && hasGold;
    }

    protected virtual bool CheckMaterials(CraftRecipe recipe)
    {
        // InventoryManager와 연동해야 함 (임시: true)
        return true;
    }

    /// <summary>
    /// 제작/강화 시도
    /// </summary>
    public virtual bool TryCraft(string recipeID)
    {
        if (!recipes.TryGetValue(recipeID, out var recipe))
        {
            OnCraftingFailed?.Invoke("존재하지 않는 레시피");
            return false;
        }

        if (!CanCraft(recipeID))
        {
            OnCraftingFailed?.Invoke("재료가 부족하거나 골드가 부족합니다");
            return false;
        }

        // 제작/강화 실행
        if (recipe.recipeType == RecipeType.Craft)
        {
            // 새 아이템 생성
            // InventoryManager.AddItem(recipe.resultItemID, 1);
            Debug.Log($"[CraftingSystemBase] 제작 완료: {recipe.resultItemName}");
        }
        else if (recipe.recipeType == RecipeType.Enhance)
        {
            // 기존 아이템 강화
            // InventoryManager.EnhanceItem(recipe.resultItemID);
            Debug.Log($"[CraftingSystemBase] 강화 완료: {recipe.resultItemName}");
        }

        OnCraftingComplete?.Invoke(recipe.resultItemID, 1);
        return true;
    }

    public CraftRecipe GetRecipe(string recipeID)
    {
        if (recipes.TryGetValue(recipeID, out var recipe))
        {
            return recipe;
        }
        return null;
    }

    public Dictionary<string, CraftRecipe> GetAllRecipes()
    {
        return new Dictionary<string, CraftRecipe>(recipes);
    }

    /// <summary>
    /// 특정 RecipeType의 레시피만 반환
    /// </summary>
    public Dictionary<string, CraftRecipe> GetRecipesByType(RecipeType type)
    {
        var filtered = new Dictionary<string, CraftRecipe>();
        foreach (var recipe in recipes.Values)
        {
            if (recipe.recipeType == type)
            {
                filtered[recipe.recipeID] = recipe;
            }
        }
        return filtered;
    }

    public int GetCraftCost(string recipeID)
    {
        if (recipes.TryGetValue(recipeID, out var recipe))
        {
            return recipe.craftCost;
        }
        return -1;
    }

    public CraftRequirement[] GetRequiredMaterials(string recipeID)
    {
        if (recipes.TryGetValue(recipeID, out var recipe))
        {
            return recipe.requiredMaterials;
        }
        return null;
    }

    public abstract void ResetRecipes();
}

