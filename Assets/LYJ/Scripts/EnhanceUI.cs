using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnhanceUI : UIBase
{
    [SerializeField] private Button closeButton;
    [SerializeField] private Button enhanceButton;
    [SerializeField] private TextMeshProUGUI enhanceCostText;
    [SerializeField] private TextMeshProUGUI enhanceDescriptionText;

    private int enhanceCost = 500;

    void Start()
    {
        if (titleText != null)
        {
            titleText.text = "강화소";
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Close);
        }

        if (enhanceButton != null)
        {
            enhanceButton.onClick.AddListener(OnEnhanceButtonClicked);
        }

        if (enhanceCostText != null)
        {
            enhanceCostText.text = $"강화 비용: <color=yellow>{enhanceCost}</color>";
        }

        if (enhanceDescriptionText != null)
        {
            enhanceDescriptionText.text = "장비를 강화하여 공격력을 올릴 수 있습니다.";
        }
    }

    void Update()
    {
        if (gameObject.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
        }
    }

    void OnEnhanceButtonClicked()
    {
        DataManager dataManager = MasterManager.Instance.DataManager;

        if (dataManager.RemoveGold(enhanceCost))
        {
            Debug.Log("장비 강화 완료!");

            if (enhanceDescriptionText != null)
            {
                enhanceDescriptionText.text = "강화가 완료되었습니다!";
                enhanceDescriptionText.color = Color.green;
            }
        }
        else
        {
            Debug.LogWarning("골드가 부족합니다");

            if (enhanceDescriptionText != null)
            {
                enhanceDescriptionText.text = "골드가 부족합니다...";
                enhanceDescriptionText.color = Color.red;
            }
        }
    }
}
