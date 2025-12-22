using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 대장간 UI (무기 제작)
/// </summary>
public class SmithyUI : MonoBehaviour
{
    [SerializeField] private GameObject smithyPanel;

    // 탭 버튼
    [SerializeField] private Button UpButton;
    [SerializeField] private Button DownButton;

    [Header("레시피 목록")]
    [SerializeField] private Transform recipeListContainer;
    [SerializeField] private GameObject recipeButtonPrefab;

    [Header("레시피 상세 정보")]
    [SerializeField] private TextMeshProUGUI recipeNameText;
    [SerializeField] Image recipeImage;
    [SerializeField] private TextMeshProUGUI recipeCostText;
    [SerializeField] private TextMeshProUGUI requirementsText;
    [SerializeField] private Button craftButton;

    [Header("레시피 스탯 변화 정보")]
    [SerializeField] private TextMeshProUGUI HPText;
    [SerializeField] private TextMeshProUGUI AttText;
    [SerializeField] private TextMeshProUGUI DefText;
    [SerializeField] private TextMeshProUGUI SpeedText;

    [SerializeField] Image madeImage;

    [SerializeField] TextMeshProUGUI CloseButtonText;

    private SmithySystem craftingSystem;
    private int selectedRecipeID;
    private bool isFirstTab = true;  // true: 무기, false: 장비

    private List<Button> createdButtons = new List<Button>();  // 생성된 버튼 추적
    private Coroutine madeImageCoroutine;

    private void Start()
    {
        craftingSystem = SmithySystem.Instance;

        if (craftingSystem == null)
        {
            Debug.LogError("[SmithyUI] SmithySystem을 찾을 수 없습니다");
            return;
        }
        // ===== 버튼 이벤트 연결 =====
        if (UpButton != null)
            UpButton.onClick.AddListener(() => SelectTab(true));  // 무기 탭

        if (DownButton != null)
            DownButton.onClick.AddListener(() => SelectTab(false));  // 장비 탭

        if (craftButton != null)
            craftButton.onClick.AddListener(OnCraftButtonClicked);

        // ===== 제작 시스템 이벤트 구독 =====
        craftingSystem.OnCraftingComplete += HandleCraftingComplete;

        CloseButtonText.text = $"안녕! [{KeySetting.GetKeyString(KeyInput.CANCLE)}]";

        // 초기 상태: 패널 비활성화

        if (smithyPanel != null)
            smithyPanel.SetActive(false);

        Debug.Log("[SmithyUI] 대장간 UI 초기화 완료");
    }

    private void Update()
    {
        // 캔슬 키로 UI 닫기
        if (Input.GetKeyDown(KeySetting.keys[KeyInput.CANCLE]))
        {
            CloseUI();
        }
    }

    public void OpenUI()
    {
        if (smithyPanel == null)
            return;

        if (smithyPanel.activeSelf)
            return;

        _MasterManager.Instance.UIManager.SetRightPanelActive(false);

        SelectTab(true);
        int RecipeID = 2001;
        SelectRecipe(RecipeID); // 선택 초기화

        smithyPanel.SetActive(true);

        UpdateTabHighlight();
        PopulateRecipeList();
    }

    public void CloseUI()
    {
        if (smithyPanel == null)
            return;

        smithyPanel.SetActive(false);
        _MasterManager.Instance.UIManager.SetRightPanelActive(true);
    }

    private void SelectTab(bool isFirst)
    {
        isFirstTab = isFirst;

        // 탭 UI 시각화 업데이트
        UpdateTabHighlight();

        // 레시피 목록 갱신
        PopulateRecipeList();
    }

    /// <summary>
    /// 현재 선택된 탭을 시각적으로 표시
    /// </summary>
    private void UpdateTabHighlight()
    {
        if (isFirstTab)
        {
            // 다운 버튼 활성화
            if (UpButton != null)
                UpButton.interactable = false;
            if (DownButton != null)
                DownButton.interactable = true;
        }
        else
        {
            // 업 버튼 활성화
            if (UpButton != null)
                UpButton.interactable = true;
            if (DownButton != null)
                DownButton.interactable = false;
        }
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
        createdButtons.Clear();

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

            Item items = recipe.outputItem;
            var type = items.equipmentType;

            // 무기만 표시
            if (isFirstTab && !(type is EquipmentType.Weapon))
                continue;

            // 장비만 표시
            if (!isFirstTab && !(type is EquipmentType.Armor))
                continue;

            GameObject buttonObj = Instantiate(recipeButtonPrefab, recipeListContainer);
            Button button = buttonObj.GetComponent<Button>();
            Image itemImage = buttonObj.GetComponent<Image>();
            itemImage.sprite = recipe.outputItem.icon;

            if (button != null)
            {
                // 클로저 문제 방지: 로컬 변수 사용
                int recipeID = recipe.recipeID;
                button.onClick.AddListener(() => SelectRecipe(recipeID));
                createdButtons.Add(button);
            }
        }
    }

    /// <summary>
    /// 레시피 선택 시 상세 정보 표시
    /// </summary>
    private void SelectRecipe(int recipeID)
    {
        selectedRecipeID = recipeID;

        var recipe = craftingSystem.GetRecipe(recipeID);
        if (recipe == null)
            return;

        var Item = recipe.outputItem;

        // 레시피 이름 표시
        if (recipeNameText != null)
            recipeNameText.text = $"{recipe.recipeName}";

        //아이템 아이콘 표시
        if (recipeImage != null)
            recipeImage.sprite = recipe.outputItem.icon;

        if (madeImage != null)
            madeImage.sprite = recipeImage.sprite;
        madeImage.color = new Color(1, 1, 1, 0);

        //스탯 변화 표시
        if (HPText != null)
            HPText.text = $"+{Item.hpPlus}";
        if (AttText != null)
            AttText.text = $"+{Item.attack}";
        if (DefText != null)
            DefText.text = $"+{Item.defense}";
        if (SpeedText != null)
            SpeedText.text = $"+{Item.speed}";

        // 비용 표시
        if (recipeCostText != null)
            recipeCostText.text = $"{recipe.goldCost}";

        // 필요 재료 표시
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
                        // 재료 보유량을 InventoryManager에서 가져오기
                        int playerQuantity = _MasterManager.Instance.InventoryManager.GetItemCount(material.materialItem);
                        requirementsStr += $"- {material.materialItem.itemName} x{material.amount} \n        (보유: {playerQuantity})\n";
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

        // 기존 애니메이션이 진행 중이면 중단
        if (madeImageCoroutine != null)
            StopCoroutine(madeImageCoroutine);

        // 새로운 애니메이션 시작
        madeImageCoroutine = StartCoroutine(ShowMadeImageEffect());

        _MasterManager.Instance.SoundManager.PlaySFX("유정", 0);

        Debug.Log($"[SmithyUI] 제작 성공: {itemName} x{quantity}");

        // 선택 해제 및 UI 갱신
        SelectRecipe(selectedRecipeID);
    }

    /// <summary>
    /// madeImage를 1초간 표시한 후 위로 움직이면서 사라지는 효과
    /// </summary>
    private IEnumerator ShowMadeImageEffect()
    {
        // 1. 이미지 표시 (알파값 1로)
        madeImage.color = new Color(1, 1, 1, 1);

        // RectTransform 초기 위치 설정
        RectTransform rectTransform = madeImage.GetComponent<RectTransform>();
        Vector3 startPos = rectTransform.localPosition;

        // 2. 1초 대기
        yield return new WaitForSeconds(1f);

        // 3. 위로 움직이면서 사라지기 (0.5초)
        float elapsedTime = 0f;
        float duration = 0.5f;
        Vector3 endPos = startPos + Vector3.up * 100f;  // 100픽셀 위로 이동

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;

            // 위치 이동
            rectTransform.localPosition = Vector3.Lerp(startPos, endPos, progress);

            // 알파값 감소 (0.5초에 걸쳐 투명해짐)
            Color color = madeImage.color;
            color.a = Mathf.Lerp(1f, 0f, progress);
            madeImage.color = color;

            yield return null;
        }

        // 4. 최종 상태 설정 (완벽하게 투명)
        madeImage.color = new Color(1, 1, 1, 0);
        rectTransform.localPosition = startPos;  // 원래 위치로 복원
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



