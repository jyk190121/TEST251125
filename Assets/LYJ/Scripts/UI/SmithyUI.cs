using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 대장간 UI (무기 제작)
/// </summary>
public class SmithyUI : MonoBehaviour
{
    [SerializeField] private GameObject smithyPanel;

    [SerializeField] private Transform recipeListContainer;
    [SerializeField] private GameObject recipeButtonPrefab;

    [SerializeField] private TextMeshProUGUI recipeNameText;
    [SerializeField] private TextMeshProUGUI recipeCostText;
    [SerializeField] private TextMeshProUGUI requirementsText;
    [SerializeField] private Button craftButton;

    private SmithySystem craftingSystem;
    private int selectedRecipeID;
    private List<Button> createdButtons = new List<Button>();  // 생성된 버튼 추적

    private void Start()
    {
        craftingSystem = SmithySystem.Instance;

        if (craftingSystem == null)
        {
            Debug.LogError("[SmithyUI] SmithySystem을 찾을 수 없습니다");
            return;
        }
        // ===== 버튼 이벤트 연결 =====
        if (craftButton != null)
            craftButton.onClick.AddListener(OnCraftButtonClicked);

        // ===== 제작 시스템 이벤트 구독 =====
        craftingSystem.OnCraftingComplete += HandleCraftingComplete;

        // 초기 상태: 패널 비활성화
        if (smithyPanel != null)
            smithyPanel.SetActive(false);

        Debug.Log("[SmithyUI] 대장간 UI 초기화 완료");
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

    /// <summary>
    /// 제작 가능한 레시피 목록을 UI에 동적으로 생성
    /// </summary>
    private void PopulateRecipeList()
    {
        // 기존 버튼 정리
        foreach (Transform child in recipeListContainer)
        {
            Button btn = child.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();  // 이벤트 리스너 제거
            }
            Destroy(child.gameObject);
        }

        var recipes = craftingSystem.GetAllRecipes();

        if (recipes == null || recipes.Count == 0)
        {
            Debug.LogWarning("[SmithyUI] 표시할 레시피가 없습니다");
            return;
        }

        // 모든 레시피에 대해 버튼 생성
        foreach (var recipe in recipes.Values)
        {
            if (recipe == null)
                continue;

            GameObject buttonObj = Instantiate(recipeButtonPrefab, recipeListContainer);
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

            if (buttonText != null)
                buttonText.text = recipe.recipeName;

            if (button != null)
            {
                // 클로저 문제 방지: 로컬 변수 사용
                int recipeID = recipe.recipeID;
                button.onClick.AddListener(() => SelectRecipe(recipeID));
                createdButtons.Add(button);
            }
        }
    }

    private void SelectRecipe(int recipeID)
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
            recipeCostText.text = $"제작 비용: {recipe.goldCost} 골드";

        // 필요 재료 표시
        if (requirementsText != null)
        {
            string requirementsStr = "필요 재료:\n";
            var materials = craftingSystem.GetRequiredMaterials(recipeID);

            if (materials != null && materials.Length > 0)
            {
                foreach (var material in materials)
                {
                    if (material.materialItem != null)
                    {
                        // TODO: 실제 플레이어 보유량을 InventoryManager에서 가져오기
                        // int playerQuantity = InventoryManager.Instance.GetItemQuantity(material.materialItem.itemID);
                        // requirementsStr += $"- {material.materialItem.itemName} x{material.amount} (보유: {playerQuantity})\n";

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


        // 제작 버튼 활성화/비활성화
        if (craftButton != null)
        {
            bool canCraft = craftingSystem.CanCraft(recipeID);
            craftButton.interactable = canCraft;
        }
    }

    private void OnCraftButtonClicked()
    {
        if (selectedRecipeID == 0)
            return;

        bool success = craftingSystem.TryCraft(selectedRecipeID);

        if (success)
        {
            var recipe = craftingSystem.GetRecipe(selectedRecipeID);
            // TODO: recipe.resultItemName이 Recipe SO에 있는지 확인
            // Debug.Log($"[SmithyUI] 제작 시도: {recipe.outputItem.itemName}");
        }
        else
        {
            Debug.Log("[SmithyUI] 제작 실패");
        }
    }

    /// <summary>
    /// 제작 성공 이벤트 핸들러
    /// </summary>
    private void HandleCraftingComplete(int itemID, int quantity)
    {
        var recipe = craftingSystem.GetRecipe(selectedRecipeID);
        string itemName = recipe != null ? recipe.outputItem.itemName : "아이템";

        Debug.Log($"[SmithyUI] 제작 성공: {itemName} x{quantity}");

        // 선택 해제 및 UI 갱신
        SelectRecipe(selectedRecipeID);
    }

    private void OnDestroy()
    {
        // 모든 이벤트 리스너 정리 (메모리 누수 방지)

        if (craftButton != null)
            craftButton.onClick.RemoveListener(OnCraftButtonClicked);

        // 제작 시스템 이벤트 구독 해제
        if (craftingSystem != null)
        {
            craftingSystem.OnCraftingComplete -= HandleCraftingComplete;
        }

        // 동적으로 생성한 버튼의 이벤트 정리
        foreach (var button in createdButtons)
        {
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
            }
        }
        createdButtons.Clear();

        Debug.Log("[SmithyUI] 대장간 UI 정리 완료");
    }
}



