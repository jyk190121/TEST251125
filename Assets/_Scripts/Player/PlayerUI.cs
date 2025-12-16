using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject leftPanel;
    [SerializeField] private GameObject rightPanel;

    [SerializeField] Image weaponImage;
    [SerializeField] private TextMeshProUGUI playerGoldText;
    [SerializeField] private TextMeshProUGUI playerHpText;
    [SerializeField] private TextMeshProUGUI playerPortionText;
    [SerializeField] private TextMeshProUGUI playerRollText;
    [SerializeField] private TextMeshProUGUI playerSubAttackText;
    [SerializeField] private TextMeshProUGUI playerMainAttackText;
    [SerializeField] private TextMeshProUGUI playerInventoryText;

    private void Update()
    {
        var player = _MasterManager.Instance.DataManager.GetStat();

        playerGoldText.text = $"{player.Money}";
        playerHpText.text = $"{player.HP} / {player.MaxHP}";

        playerPortionText.text = $"{KeySetting.keys[KeyInput.PENDANT]}";
        playerRollText.text = $"{KeySetting.keys[KeyInput.ROLL]}";
        playerSubAttackText.text = $"{KeySetting.keys[KeyInput.SUBATTACK]}";
        playerMainAttackText.text = $"{KeySetting.keys[KeyInput.MAINATTACK]}";
        playerInventoryText.text = $"{KeySetting.keys[KeyInput.INVENTORY]}";

        weaponImage.sprite = _MasterManager.Instance.DataManager.EquipWeapon.icon;
    }

    public void SetLeftPanelActive(bool active) => leftPanel.SetActive(active);
    public void SetRightPanelActive(bool active) => rightPanel.SetActive(active);
}

