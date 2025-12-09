using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// 제작과 강화 시스템의 부모 클래스
/// SmithySystem과 WoodenHatSystem이 상속받음
/// </summary>
public abstract class CraftingSystemBase : MonoBehaviour
{
    protected Dictionary<int, Recipe> recipes = new Dictionary<int, Recipe>();

    public event Action<int, int> OnCraftingComplete;      // (아이템 ID, 개수)
    public event Action<string> OnCraftingFailed;

    /// <summary>
    /// 제작 가능한지 확인
    /// </summary>
    public virtual bool CanCraft(int recipeID)
    {
        if (!recipes.TryGetValue(recipeID, out var recipe))
        {
            Debug.LogError($"[CraftingSystemBase] 존재하지 않는 레시피: {recipeID}");
            return false;
        }

        bool hasMaterials = CheckMaterials(recipe);
        bool hasGold = CheckGold(recipe.goldCost);

        return hasMaterials && hasGold;
    }
    protected virtual bool CheckMaterials(Recipe recipe)
    {
        if (recipe.inputMaterials == null || recipe.inputMaterials.Length == 0)
            return true;

        foreach (var material in recipe.inputMaterials)
        {
            // TODO: InventoryManager에서 플레이어 보유 재료 확인
            // int playerQuantity = InventoryManager.Instance.GetItemQuantity(material.materialItem.itemID);
            // if (playerQuantity < material.amount)
            //     return false;
        }
        return true;
    }

    protected virtual bool CheckGold(int goldCost)
    {
        // DataManager에서 플레이어 골드 확인
        int playerGold = _MasterManager.Instance.DataManager.HojuMoney();
        return playerGold >= goldCost;
    }


    /// <summary>
    /// 제작/강화 시도
    /// </summary>
    public virtual bool TryCraft(int recipeID)
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

        // TODO: InventoryManager에서 재료 차감
        // foreach (var material in recipe.inputMaterials)
        // {
        //     InventoryManager.RemoveItem(material.materialItem.itemID, material.amount);
        // }

        // TODO: DataManager에서 골드 차감
        // _MasterManager.Instance.DataManager.SpendGold(recipe.goldCost);

        // TODO: InventoryManager에서 결과 아이템 추가
        // InventoryManager.AddItem(recipe.outputItem, 1);

        Debug.Log($"[CraftingSystemBase] 제작 완료: {recipe.outputItem.itemName}");
        OnCraftingComplete?.Invoke(recipe.outputItem.itemID, 1);
        return true;
    }

    public Recipe GetRecipe(int recipeID)
    {
        if (recipes.TryGetValue(recipeID, out var recipe))
        {
            return recipe;
        }
        return null;
    }

    public Dictionary<int, Recipe> GetAllRecipes()
    {
        return new Dictionary<int, Recipe>(recipes);
    }

    public int GetCraftCost(int recipeID)
    {
        if (recipes.TryGetValue(recipeID, out var recipe))
        {
            return recipe.goldCost;
        }
        return -1;
    }

    public MaterialCost[] GetRequiredMaterials(int recipeID)
    {
        if (recipes.TryGetValue(recipeID, out var recipe))
        {
            return recipe.inputMaterials;
        }
        return null;
    }

    public abstract void InitializeRecipes();
}

