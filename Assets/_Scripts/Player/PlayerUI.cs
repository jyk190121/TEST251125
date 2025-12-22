using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public static PlayerUI Instance { get; private set; }

    [SerializeField] private GameObject leftPanel;
    [SerializeField] private GameObject rightPanel;

    [SerializeField] private Image weaponImage;
    [SerializeField] private Slider playerHpSlider;
    [SerializeField] private TextMeshProUGUI playerGoldText;
    [SerializeField] private TextMeshProUGUI playerHpText;
    [SerializeField] private TextMeshProUGUI playerPortionText;
    [SerializeField] private TextMeshProUGUI playerRollText;
    [SerializeField] private TextMeshProUGUI playerSubAttackText;
    [SerializeField] private TextMeshProUGUI playerMainAttackText;
    [SerializeField] private TextMeshProUGUI playerInventoryText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // 이미 다른 PlayerUI가 있으면 자신은 제거
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Update()
    {
        if (_MasterManager.Instance == null || _MasterManager.Instance.DataManager == null)
            return;

        var player = _MasterManager.Instance.DataManager.GetStat();

        if (playerGoldText != null)
            playerGoldText.text = $"{player.Money}";

        if (playerHpSlider != null)
        {
            playerHpSlider.minValue = 1;
            playerHpSlider.maxValue = player.MaxHP;
            playerHpSlider.value = player.HP;
        }            

        if (playerHpText != null)
            playerHpText.text = $"{player.HP} / {player.MaxHP}";

        if (playerPortionText != null)
            playerPortionText.text = $"{KeySetting.keys[KeyInput.QUICKSLOT]}";

        if (playerRollText != null)
            playerRollText.text = $"{KeySetting.keys[KeyInput.ROLL]}";

        if (playerSubAttackText != null)
            playerSubAttackText.text = $"{KeySetting.keys[KeyInput.SUBATTACK]}";

        if (playerMainAttackText != null)
            playerMainAttackText.text = $"{KeySetting.keys[KeyInput.MAINATTACK]}";

        if (playerInventoryText != null)
            playerInventoryText.text = $"{KeySetting.keys[KeyInput.INVENTORY]}";

        if (_MasterManager.Instance.DataManager.EquipWeapon != null && weaponImage != null)
        {
            weaponImage.sprite = _MasterManager.Instance.DataManager.EquipWeapon.icon;
        }
    }

    /// <summary>
    /// 씬 전환 후, 해당 씬의 Canvas 안에 있는 패널/텍스트들을 다시 찾아서 연결
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        leftPanel = GameObject.Find("Left");
        rightPanel = GameObject.Find("Right");
    }

    public void SetLeftPanelActive(bool active)
    {
        if (leftPanel == null) return;
        leftPanel.SetActive(active);
    }

    public void SetRightPanelActive(bool active)
    {
        if (rightPanel == null) return;
        rightPanel.SetActive(active);
    }
}


