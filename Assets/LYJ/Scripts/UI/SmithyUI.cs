using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 대장간 UI (무기 제작만)
/// </summary>
public class SmithyUI : MonoBehaviour
{
    [SerializeField] private GameObject smithyPanel;
    [SerializeField] private Button closeButton;

    [SerializeField] private Transform recipeListContainer;
    [SerializeField] private GameObject recipeButtonPrefab;

    [SerializeField] private TextMeshProUGUI recipeNameText;
    [SerializeField] private TextMeshProUGUI recipeCostText;
    [SerializeField] private TextMeshProUGUI requirementsText;
    [SerializeField] private Button craftButton;

    private SmithySystem craftingSystem;
    private string selectedRecipeID;

    private void Start()
    {
        craftingSystem = SmithySystem.Instance;

        if (craftingSystem == null)
        {
            Debug.LogError("[SmithyUI] SmithySystem을 찾을 수 없습니다");
            return;
        }

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseUI);

        if (craftButton != null)
            craftButton.onClick.AddListener(OnCraftButtonClicked);

        if (smithyPanel != null)
            smithyPanel.SetActive(false);
    }

    public void OpenUI()
    {
        if (smithyPanel == null)
            return;

        smithyPanel.SetActive(true);
        PopulateRecipeList();
    }

    public void CloseUI()
    {
        if (smithyPanel == null)
            return;

        smithyPanel.SetActive(false);
    }

    private void PopulateRecipeList()
    {
        // 기존 버튼 삭제
        foreach (Transform child in recipeListContainer)
        {
            Destroy(child.gameObject);
        }

        var recipes = craftingSystem.GetAllRecipes();

        foreach (var recipe in recipes.Values)
        {
            GameObject buttonObj = Instantiate(recipeButtonPrefab, recipeListContainer);
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

            if (buttonText != null)
                buttonText.text = recipe.recipeName;  // 레시피 이름 표시

            if (button != null)
            {
                button.onClick.AddListener(() => SelectRecipe(recipe.recipeID));
            }
        }
    }

    private void SelectRecipe(string recipeID)
    {
        selectedRecipeID = recipeID;

        var recipe = craftingSystem.GetRecipe(recipeID);
        if (recipe == null)
            return;

        // 레시피 이름 표시
        if (recipeNameText != null)
            recipeNameText.text = $"레시피: {recipe.recipeName}";

        // 비용 표시
        if (recipeCostText != null)
            recipeCostText.text = $"제작 비용: {recipe.craftCost} 골드";

        // 필요 재료 표시
        if (requirementsText != null)
        {
            string requirementsStr = "필요 재료:\n";
            foreach (var requirement in recipe.requiredMaterials)
            {
                requirementsStr += $"- {requirement.itemID} x{requirement.quantity}\n";
            }
            requirementsText.text = requirementsStr;
        }

        // 제작 버튼 활성화/비활성화
        if (craftButton != null)
        {
            bool canCraft = craftingSystem.CanCraft(recipeID);
            craftButton.interactable = canCraft;
        }
    }

    private void OnCraftButtonClicked()
    {
        if (string.IsNullOrEmpty(selectedRecipeID))
            return;

        bool success = craftingSystem.TryCraft(selectedRecipeID);

        if (success)
        {
            var recipe = craftingSystem.GetRecipe(selectedRecipeID);
            Debug.Log($"[SmithyUI] 제작 완료: {recipe.resultItemName}");
        }
        else
        {
            Debug.Log("[SmithyUI] 제작 실패");
        }

        PopulateRecipeList();
    }

    private void OnDestroy()
    {
        if (closeButton != null)
            closeButton.onClick.RemoveListener(CloseUI);

        if (craftButton != null)
            craftButton.onClick.RemoveListener(OnCraftButtonClicked);
    }
}

