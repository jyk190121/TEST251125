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
    private int selectedRecipeID;  // ← string에서 int로 변경
    private bool isPocionTab = true;  // ← true: 포션, false: 강화

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
            craftTabButton.onClick.AddListener(() => SelectTab(true));  // 포션 탭

        if (enhanceTabButton != null)
            enhanceTabButton.onClick.AddListener(() => SelectTab(false));  // 강화 탭

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
        SelectTab(true);  // 기본 탭: 포션 제작
    }

    public void CloseUI()
    {
        if (woodenHatPanel == null)
            return;

        woodenHatPanel.SetActive(false);
    }

    private void SelectTab(bool isPotion)
    {
        isPocionTab = isPotion;
        selectedRecipeID = 0;  // ← 선택 초기화
        PopulateRecipeList();

        // 탭 버튼 강조 (시각 효과)
        if (isPocionTab)
        {
            Debug.Log("[WoodenHatUI] 포션 제작 탭 선택");
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

        var allRecipes = craftingSystem.GetAllRecipes();

        foreach (var recipe in allRecipes.Values)
        {
            // 포션 탭: AlchemyRecipe만 표시
            if (isPocionTab && !(recipe is AlchemyRecipe))
                continue;

            // 강화 탭: EnchantRecipe만 표시
            if (!isPocionTab && !(recipe is EnchantRecipe))
                continue;

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

    private void SelectRecipe(int recipeID)  // ← string에서 int로 변경
    {
        selectedRecipeID = recipeID;

        var recipe = craftingSystem.GetRecipe(recipeID);
        if (recipe == null)
            return;

        // 레시피 이름 표시
        if (recipeNameText != null)
        {
            string typeStr = isPocionTab ? "포션 제작" : "강화";
            recipeNameText.text = $"{typeStr}: {recipe.recipeName}";
        }

        // 비용 표시
        if (recipeCostText != null)
            recipeCostText.text = $"비용: {recipe.goldCost} 골드";  // ← craftCost에서 goldCost로 변경

        // 필요 재료 표시
        if (requirementsText != null)
        {
            string requirementsStr = "필요 재료:\n";
            var materials = craftingSystem.GetRequiredMaterials(recipeID);  // ← 메서드 사용

            if (materials != null && materials.Length > 0)
            {
                foreach (var material in materials)
                {
                    if (material.materialItem != null)
                    {
                        requirementsStr += $"- {material.materialItem.itemName} x{material.amount}\n";
                    }
                }
            }
            else
            {
                requirementsStr += "필요한 재료 없음";
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
                string btnLabel = isPocionTab ? "포션 제작" : "강화";
                btnText.text = btnLabel;
            }
        }
    }

    private void OnCraftButtonClicked()
    {
        if (selectedRecipeID == 0)  // ← int 기본값 0으로 변경
            return;

        bool success = craftingSystem.TryCraft(selectedRecipeID);

        if (success)
        {
            var recipe = craftingSystem.GetRecipe(selectedRecipeID);
            string action = isPocionTab ? "포션 제작" : "강화";
            // TODO: recipe.outputItem.itemName 확인
            Debug.Log($"[WoodenHatUI] {action} 완료: {recipe.outputItem.itemName}");
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

