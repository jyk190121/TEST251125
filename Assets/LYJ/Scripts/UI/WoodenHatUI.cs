using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// 나무 모자 UI (물약 제작 + 무기 강화)
/// 탭 전환 기능 포함
/// </summary>
public class WoodenHatUI : MonoBehaviour
{
    [SerializeField] private GameObject woodenHatPanel;
    [SerializeField] private Button closeButton;

    // 탭 버튼
    [SerializeField] private Button craftTabButton;
    [SerializeField] private Button enhanceTabButton;

    [SerializeField] private Transform recipeListContainer;
    [SerializeField] private GameObject recipeButtonPrefab;

    [SerializeField] private TextMeshProUGUI recipeNameText;
    [SerializeField] private TextMeshProUGUI recipeCostText;
    [SerializeField] private TextMeshProUGUI requirementsText;
    [SerializeField] private Button craftButton;

    private WoodenHatSystem craftingSystem;
    private string selectedRecipeID;
    private CraftingSystemBase.RecipeType currentTabType = CraftingSystemBase.RecipeType.Craft;

    private void Start()
    {
        craftingSystem = WoodenHatSystem.Instance;

        if (craftingSystem == null)
        {
            Debug.LogError("[WoodenHatUI] WoodenHatSystem을 찾을 수 없습니다");
            return;
        }

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseUI);

        if (craftTabButton != null)
            craftTabButton.onClick.AddListener(() => SelectTab(CraftingSystemBase.RecipeType.Craft));

        if (enhanceTabButton != null)
            enhanceTabButton.onClick.AddListener(() => SelectTab(CraftingSystemBase.RecipeType.Enhance));

        if (craftButton != null)
            craftButton.onClick.AddListener(OnCraftButtonClicked);

        if (woodenHatPanel != null)
            woodenHatPanel.SetActive(false);
    }

    public void OpenUI()
    {
        if (woodenHatPanel == null)
            return;

        woodenHatPanel.SetActive(true);
        SelectTab(CraftingSystemBase.RecipeType.Craft); // 기본 탭: 제작
    }

    public void CloseUI()
    {
        if (woodenHatPanel == null)
            return;

        woodenHatPanel.SetActive(false);
    }

    private void SelectTab(CraftingSystemBase.RecipeType tabType)
    {
        currentTabType = tabType;
        PopulateRecipeList();

        // 탭 버튼 강조 (시각 효과)
        if (currentTabType == CraftingSystemBase.RecipeType.Craft)
        {
            Debug.Log("[WoodenHatUI] 제작 탭 선택");
        }
        else
        {
            Debug.Log("[WoodenHatUI] 강화 탭 선택");
        }
    }

    private void PopulateRecipeList()
    {
        // 기존 버튼 삭제
        foreach (Transform child in recipeListContainer)
        {
            Destroy(child.gameObject);
        }

        // 현재 탭 타입의 레시피만 가져오기
        var recipes = craftingSystem.GetRecipesByType(currentTabType);

        foreach (var recipe in recipes.Values)
        {
            GameObject buttonObj = Instantiate(recipeButtonPrefab, recipeListContainer);
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

            if (buttonText != null)
                buttonText.text = recipe.recipeName;

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
        {
            string typeStr = recipe.recipeType == CraftingSystemBase.RecipeType.Craft ? "제작" : "강화";
            recipeNameText.text = $"{typeStr}: {recipe.recipeName}";
        }

        // 비용 표시
        if (recipeCostText != null)
            recipeCostText.text = $"비용: {recipe.craftCost} 골드";

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

        // 제작/강화 버튼 활성화/비활성화
        if (craftButton != null)
        {
            bool canCraft = craftingSystem.CanCraft(recipeID);
            craftButton.interactable = canCraft;

            // 버튼 텍스트 변경
            TextMeshProUGUI btnText = craftButton.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null)
            {
                string btnLabel = recipe.recipeType == CraftingSystemBase.RecipeType.Craft ? "제작" : "강화";
                btnText.text = btnLabel;
            }
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
            string action = recipe.recipeType == CraftingSystemBase.RecipeType.Craft ? "제작" : "강화";
            Debug.Log($"[WoodenHatUI] {action} 완료: {recipe.resultItemName}");
        }
        else
        {
            Debug.Log("[WoodenHatUI] 작업 실패");
        }

        PopulateRecipeList();
    }

    private void OnDestroy()
    {
        if (closeButton != null)
            closeButton.onClick.RemoveListener(CloseUI);

        if (craftTabButton != null)
            craftTabButton.onClick.RemoveAllListeners();

        if (enhanceTabButton != null)
            enhanceTabButton.onClick.RemoveAllListeners();

        if (craftButton != null)
            craftButton.onClick.RemoveListener(OnCraftButtonClicked);
    }
}
