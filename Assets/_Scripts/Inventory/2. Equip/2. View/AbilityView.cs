using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityView : MonoBehaviour
{
    [Header("능력치")]
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI defenceText;
    public TextMeshProUGUI speedText;

    PlayerModel stat;

    private void OnEnable()
    {
        //장비 변경 등으로 스탯이 바뀔 때 UI 자동 갱신
        DataManager.OnStatChanged += PlayerStat;
    }

    private void Start()
    {
        //처음 UI 표시
        PlayerStat();
    }

    public void PlayerStat()
    {
        //null 체크
        if (_MasterManager.Instance == null || _MasterManager.Instance.DataManager == null) return;

        stat = _MasterManager.Instance.DataManager.GetStat();
        if (stat == null) return;

        hpText.text = $"{stat.MaxHP}";
        attackText.text = $"{stat.ATT}";
        defenceText.text = $"{stat.Defend}";
        speedText.text = $"{stat.moveSpeed}";
    }
}