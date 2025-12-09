using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static VillageSystemManager;

/// <summary>
/// 시설 구매 UI
/// 플레이어가 시설 구매대에서 시설을 구매
/// </summary>
public class FacilitiesBuyUI : MonoBehaviour
{
    // 시설 선택 버튼
    [SerializeField] private Button smithySelectButton;
    [SerializeField] private Button woodenHatSelectButton;

    // 시설 상세 정보 표시 (사용x)
    private TextMeshProUGUI facilityNameText;
    private TextMeshProUGUI facilityCostText;
    //요거 사용
    [SerializeField] private GameObject SmithyPanel;
    [SerializeField] private GameObject WoodenHatPanel;

    // 구매 버튼
    [SerializeField] private Button purchaseButton;
    [SerializeField] private TextMeshProUGUI purchaseButtonText;

    [SerializeField] TextMeshProUGUI CloseButtonText;

    private VillageSystemManager villageSystemManager;
    private string selectedFacilityID;

    private void Start()
    {
        villageSystemManager = VillageSystemManager.Instance;

        if (villageSystemManager == null)
        {
            Debug.LogError("[FacilitiesBuyUI] VillageSystemManager을 찾을 수 없습니다");
            return;
        }

        // 버튼 이벤트 연결
        if (smithySelectButton != null)
            smithySelectButton.onClick.AddListener(() => OnFacilitySelected("smithy"));

        if (woodenHatSelectButton != null)
            woodenHatSelectButton.onClick.AddListener(() => OnFacilitySelected("wooden_hat"));

        if (purchaseButton != null)
            purchaseButton.onClick.AddListener(OnPurchaseButtonClicked);

        // 초기 상태: 대장간 선택
        ClearDisplay();

        CloseButtonText.text = $"닫기 [{KeySetting.GetKeyString(KeyInput.INTERACTIVE)}]";
    }

    private void Update()
    {
        // 인터렉티브 키로 UI 닫기
        if (Input.GetKeyDown(KeySetting.keys[KeyInput.INTERACTIVE]))
        {
            CloseUI();
        }
    }

    /// <summary>
    /// 시설 선택 (Smithy 또는 WoodenHat 버튼 클릭)
    /// </summary>
    private void OnFacilitySelected(string facilityID)
    {
        selectedFacilityID = facilityID;
        UpdateDisplay();

        Debug.Log($"[FacilitiesBuyUI] 시설 선택: {facilityID}");
    }

    /// <summary>
    /// 선택된 시설 정보 표시
    /// </summary>
    private void UpdateDisplay()
    {
        if (string.IsNullOrEmpty(selectedFacilityID))
        {
            ClearDisplay();
            return;
        }

        // 시설 정보 가져오기
        string facilityName = villageSystemManager.GetFacilityName(selectedFacilityID);
        int facilityCost = villageSystemManager.GetFacilityUnlockCost(selectedFacilityID);
        bool isUnlocked = villageSystemManager.IsFacilityUnlocked(selectedFacilityID);

        // UI 업데이트
        if (selectedFacilityID == "wooden_hat")
        {
            SmithyPanel.SetActive(false);
            WoodenHatPanel.SetActive(true);
        }
        else 
        {
            SmithyPanel.SetActive(true);
            WoodenHatPanel.SetActive(false);
        }
        /*
        if (facilityNameText != null)
            facilityNameText.text = facilityName;

        if (facilityCostText != null)
        {
            if (isUnlocked)
            {
                facilityCostText.text = "";
            }
            else
            {
                facilityCostText.text = $"{facilityCost}";
            }
        }
        */

        // 구매 버튼 상태 업데이트
        if (purchaseButton != null)
        {
            purchaseButton.interactable = !isUnlocked;

            if (purchaseButtonText != null)
            {
                if (isUnlocked)
                {
                    if (selectedFacilityID == "smithy")
                    {
                        smithySelectButton.image.color = new Color(1f, 1f, 1f, 1f);
                    }
                    else if (selectedFacilityID == "wooden_hat")
                    {
                        woodenHatSelectButton.image.color = new Color(1f, 1f, 1f, 1f);
                    }

                    purchaseButton.image.color = new Color(1f, 1f, 1f, 0f);
                    purchaseButtonText.text = "구매 완료";
                }
                else
                {
                    purchaseButton.image.color = new Color(1f, 1f, 1f, 1f);
                    purchaseButtonText.text = "구매하기";
                }
            }
        }
    }

    /// <summary>
    /// 표시 초기화
    /// </summary>
    private void ClearDisplay()
    {
        OnFacilitySelected("smithy");
    }

    /// <summary>
    /// 구매 버튼 클릭
    /// </summary>
    private void OnPurchaseButtonClicked()
    {
        // villageSystemManager와 연동
        bool success = villageSystemManager.TryUnlockFacility(selectedFacilityID);

        if (success)
        {
            string facilityName = villageSystemManager.GetFacilityName(selectedFacilityID);
            Debug.Log($"[FacilitiesBuyUI] {facilityName} 구매 완료!");
            UpdateDisplay();
        }
        else
        {
            Debug.Log("[FacilitiesBuyUI] 구매 실패");
        }
    }

    public void CloseUI()
    {
        gameObject.SetActive(false);
        ClearDisplay();
    }

    private void OnDestroy()
    {
        if (smithySelectButton != null)
            smithySelectButton.onClick.RemoveAllListeners();
        if (woodenHatSelectButton != null)
            woodenHatSelectButton.onClick.RemoveAllListeners();

        if (purchaseButton != null)
            purchaseButton.onClick.RemoveAllListeners();
    }
}

