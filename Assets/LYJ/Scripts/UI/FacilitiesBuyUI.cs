using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// 시설 구매 UI
/// 플레이어가 시설 구매대에서 시설을 구매
/// </summary>
public class FacilitiesBuyUI : MonoBehaviour
{
    [SerializeField] private GameObject buyPanel;
    [SerializeField] private Button closeButton;

    [SerializeField] private Button buySmithyButton;
    [SerializeField] private TextMeshProUGUI smithyCostText;

    [SerializeField] private Button buyWoodenHatButton;
    [SerializeField] private TextMeshProUGUI woodenHatCostText;

    private VillageSystemManager villageSystemManager;

    private void Start()
    {
        villageSystemManager = VillageSystemManager.Instance;

        if (villageSystemManager == null)
        {
            Debug.LogError("[FacilitiesBuyUI] VillageSystemManager을 찾을 수 없습니다");
            return;
        }

        if (closeButton != null)
            closeButton.onClick.AddListener(CloseUI);

        if (buySmithyButton != null)
            buySmithyButton.onClick.AddListener(() => OnBuyFacility("smithy"));

        if (buyWoodenHatButton != null)
            buyWoodenHatButton.onClick.AddListener(() => OnBuyFacility("wooden_hat"));

        if (buyPanel != null)
            buyPanel.SetActive(false);

        UpdateUI();
    }

    public void OpenUI()
    {
        if (buyPanel == null)
            return;

        buyPanel.SetActive(true);
        UpdateUI();
    }

    public void CloseUI()
    {
        if (buyPanel == null)
            return;

        buyPanel.SetActive(false);
    }

    private void UpdateUI()
    {
        // 대장간 정보
        bool smithyUnlocked = villageSystemManager.IsFacilityUnlocked("smithy");
        int smithyCost = villageSystemManager.GetFacilityUnlockCost("smithy");

        if (smithyCostText != null)
        {
            if (smithyUnlocked)
            {
                smithyCostText.text = "구매됨";
            }
            else
            {
                smithyCostText.text = $"비용: {smithyCost}G";
            }
        }

        if (buySmithyButton != null)
            buySmithyButton.interactable = !smithyUnlocked;

        // 나무모자 정보
        bool woodenHatUnlocked = villageSystemManager.IsFacilityUnlocked("wooden_hat");
        int woodenHatCost = villageSystemManager.GetFacilityUnlockCost("wooden_hat");

        if (woodenHatCostText != null)
        {
            if (woodenHatUnlocked)
            {
                woodenHatCostText.text = "구매됨";
            }
            else
            {
                woodenHatCostText.text = $"비용: {woodenHatCost}G";
            }
        }

        if (buyWoodenHatButton != null)
            buyWoodenHatButton.interactable = !woodenHatUnlocked;
    }

    private void OnBuyFacility(string facilityID)
    {
        // DataManager와 연동해 골드 차감 (임시: 직접 진행)
        bool success = villageSystemManager.TryUnlockFacility(facilityID);

        if (success)
        {
            string facilityName = villageSystemManager.GetFacilityName(facilityID);
            Debug.Log($"[FacilitiesBuyUI] {facilityName} 구매 완료!");
            UpdateUI();
        }
        else
        {
            Debug.Log("[FacilitiesBuyUI] 구매 실패");
        }
    }

    private void OnDestroy()
    {
        if (closeButton != null)
            closeButton.onClick.RemoveListener(CloseUI);

        if (buySmithyButton != null)
            buySmithyButton.onClick.RemoveAllListeners();

        if (buyWoodenHatButton != null)
            buyWoodenHatButton.onClick.RemoveAllListeners();
    }
}

