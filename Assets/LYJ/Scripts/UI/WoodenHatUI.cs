using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 나무 모자 UI (물약 제작 + 무기 강화)
/// 탭 전환 기능 포함
/// </summary>
public class WoodenHatUI : MonoBehaviour
{
    [SerializeField] private GameObject woodenHatPanel;

    // 탭 버튼
    [SerializeField] private Button craftTabButton;
    [SerializeField] private Button enhanceTabButton;
    [SerializeField] private Image craftTabHighlight;
    [SerializeField] private Image enhanceTabHighlight;

    [Header("레시피 목록")]
    [SerializeField] private Transform recipeListContainer;
    [SerializeField] private GameObject recipeButtonPrefab;

    [Header("레시피 상세 정보")]
    [SerializeField] private TextMeshProUGUI recipeNameText;
    [SerializeField] private TextMeshProUGUI recipeCostText;
    [SerializeField] private TextMeshProUGUI requirementsText;
    [SerializeField] private Button craftButton;
    [SerializeField] private TextMeshProUGUI craftButtonText;

    private WoodenHatSystem craftingSystem;
    private int selectedRecipeID;
    private bool isPocionTab = true;  // true: 포션, false: 강화

    private List<Button> createdButtons = new List<Button>();  // 생성된 버튼 추적

    private void Start()
    {
        craftingSystem = WoodenHatSystem.Instance;

        if (craftingSystem == null)
        {
            Debug.LogError("[WoodenHatUI] WoodenHatSystem을 찾을 수 없습니다");
            return;
        }

        // ===== UI 버튼 이벤트 연결 =====
        if (craftTabButton != null)
            craftTabButton.onClick.AddListener(() => SelectTab(true));  // 포션 탭

        if (enhanceTabButton != null)
            enhanceTabButton.onClick.AddListener(() => SelectTab(false));  // 강화 탭

        if (craftButton != null)
            craftButton.onClick.AddListener(OnCraftButtonClicked);

        // ===== 제작 시스템 이벤트 구독 =====
        craftingSystem.OnCraftingComplete += HandleCraftingComplete;

        // 초기 상태: 패널 비활성화
        if (woodenHatPanel != null)
            woodenHatPanel.SetActive(false);

        Debug.Log("[WoodenHatUI] 나무 모자 UI 초기화 완료");
    }

    public void OpenUI()
    {
        if (woodenHatPanel == null)
        {
            Debug.LogError("[WoodenHatUI] woodenHatPanel이 할당되지 않았습니다");
            return;
        }

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
        selectedRecipeID = 0;  // 선택 초기화

        // 탭 UI 시각화 업데이트
        UpdateTabHighlight();

        // 레시피 목록 갱신
        PopulateRecipeList();

        if (isPocionTab)
        {
            Debug.Log("[WoodenHatUI] 포션 제작 탭 선택");
        }
        else
        {
            Debug.Log("[WoodenHatUI] 강화 탭 선택");
        }
    }

    /// <summary>
    /// 현재 선택된 탭을 시각적으로 표시
    /// </summary>
    private void UpdateTabHighlight()
    {
        if (isPocionTab)
        {
            // 포션 탭 활성화
            if (craftTabButton != null)
                craftTabButton.interactable = false;
            if (enhanceTabButton != null)
                enhanceTabButton.interactable = true;
        }
        else
        {
            // 강화 탭 활성화
            if (craftTabButton != null)
                craftTabButton.interactable = true;
            if (enhanceTabButton != null)
                enhanceTabButton.interactable = false;
        }
    }

    /// <summary>
    /// 현재 탭에 맞는 레시피 목록을 UI에 동적으로 생성
    /// </summary>
    private void PopulateRecipeList()
    {
        // 기존 버튼 정리 (메모리 누수 방지)
        foreach (Transform child in recipeListContainer)
        {
            Button btn = child.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.RemoveAllListeners();
            }
            Destroy(child.gameObject);
        }
        createdButtons.Clear();

        var allRecipes = craftingSystem.GetAllRecipes();

        if (allRecipes == null || allRecipes.Count == 0)
        {
            Debug.LogWarning("[WoodenHatUI] 표시할 레시피가 없습니다");
            return;
        }

        int recipeCount = 0;

        foreach (var recipe in allRecipes.Values)
        {
            if (recipe == null)
                continue;

            // 포션 탭: AlchemyRecipe만 표시
            if (isPocionTab && !(recipe is AlchemyRecipe))
                continue;

            // 강화 탭: EnchantRecipe만 표시
            if (!isPocionTab && !(recipe is EnchantRecipe))
                continue;

            // 버튼 생성
            GameObject buttonObj = Instantiate(recipeButtonPrefab, recipeListContainer);
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

            if (buttonText != null)
                buttonText.text = "";

            if (button != null)
            {
                // 클로저 문제 방지: 로컬 변수 사용
                int recipeID = recipe.recipeID;
                button.onClick.AddListener(() => SelectRecipe(recipeID));
                createdButtons.Add(button);
                recipeCount++;
            }
        }

        Debug.Log($"[WoodenHatUI] {recipeCount}개의 레시피 버튼 생성 완료 ({(isPocionTab ? "포션" : "강화")} 탭)");
    }

    /// <summary>
    /// 레시피 선택 시 상세 정보 표시
    /// </summary>
    private void SelectRecipe(int recipeID)
    {
        selectedRecipeID = recipeID;

        var recipe = craftingSystem.GetRecipe(recipeID);
        if (recipe == null)
        {
            Debug.LogError($"[WoodenHatUI] 레시피를 찾을 수 없음: {recipeID}");
            return;
        }

        // ===== 레시피 정보 UI에 표시 =====

        // 1. 레시피 이름
        if (recipeNameText != null)
        {
            recipeNameText.text = $"{recipe.recipeName}";
        }

        // 2. 제작 비용
        if (recipeCostText != null)
            recipeCostText.text = $"{recipe.goldCost}";

        // 3. 필요 재료
        if (requirementsText != null)
        {
            string requirementsStr = "";
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

        // 4. 제작/강화 버튼 활성화 및 텍스트 설정
        if (craftButton != null)
        {
            bool canCraft = craftingSystem.CanCraft(recipeID);
            craftButton.interactable = canCraft;

            // 버튼 텍스트 업데이트 (캐시된 TextMeshProUGUI 사용)
            if (craftButtonText != null)
            {
                string btnLabel = isPocionTab ? "제작하기" : "강화하기";
                craftButtonText.text = btnLabel;
            }
        }

        Debug.Log($"[WoodenHatUI] 레시피 선택: {recipe.recipeName}");
    }

    /// <summary>
    /// 제작/강화 버튼 클릭 처리
    /// </summary>
    private void OnCraftButtonClicked()
    {
        if (selectedRecipeID == 0) return;
        
        var recipe = craftingSystem.GetRecipe(selectedRecipeID);

        string action = isPocionTab ? "포션 제작" : "강화";
        Debug.Log($"[WoodenHatUI] {action} 시도: {recipe.recipeName}");

        bool success = craftingSystem.TryCraft(selectedRecipeID);

        if (success)
        {
            Debug.Log($"[WoodenHatUI] {action} 요청 전송");
        }
        else
        {
            Debug.Log($"[WoodenHatUI] {action} 요청 실패");
        }
    }

    /// <summary>
    /// 제작/강화 성공 이벤트 핸들러
    /// </summary>
    private void HandleCraftingComplete(int itemID, int quantity)
    {
        var recipe = craftingSystem.GetRecipe(selectedRecipeID);
        string itemName = recipe != null ? recipe.outputItem.itemName : "아이템";
        string action = isPocionTab ? "포션 제작" : "강화";

        Debug.Log($"[WoodenHatUI] {action} 성공: {itemName} x{quantity}");

        // 선택 해제 및 UI 갱신
        SelectRecipe(selectedRecipeID);
    }

    private void OnDestroy()
    {
        // 모든 이벤트 리스너 정리 (메모리 누수 방지)
        if (craftTabButton != null)
            craftTabButton.onClick.RemoveAllListeners();

        if (enhanceTabButton != null)
            enhanceTabButton.onClick.RemoveAllListeners();

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

        Debug.Log("[WoodenHatUI] 나무 모자 UI 정리 완료");
    }
}

