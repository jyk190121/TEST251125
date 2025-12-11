using TMPro;
using UnityEngine;
public class UIManager : MonoBehaviour
{
    //Left UP Panel
    [SerializeField] TextMeshProUGUI PlayerGoldText;
    [SerializeField] TextMeshProUGUI PlayerHpText;

    //RIGHT UP Panel
    [SerializeField] TextMeshProUGUI PlayerPortionText;
    [SerializeField] TextMeshProUGUI PlayerRollText;
    [SerializeField] TextMeshProUGUI PlayerSubAttackText;
    [SerializeField] TextMeshProUGUI PlayerMainAttackText;
    [SerializeField] TextMeshProUGUI PlayerInvetoryText;

    //필요한 정보
    PlayerModel player;             //플레이어 정보


    void Start()
    {
        player = _MasterManager.Instance.DataManager.GetStat();     //최초 플레이어 정보 초기화
    }

    void Update()
    {
        PlayerGoldText.text = $"{player.Money}";
        PlayerHpText.text = $"{player.HP} / {player.MaxHP}";

        // 단축키 UI 텍스트 업데이트
        PlayerPortionText.text = $"{KeySetting.keys[KeyInput.PENDANT]}";
        PlayerRollText.text = $"{KeySetting.keys[KeyInput.ROLL]}";
        PlayerSubAttackText.text = $"{KeySetting.keys[KeyInput.SUBATTACK]}";
        PlayerMainAttackText.text = $"{KeySetting.keys[KeyInput.MAINATTACK]}";
        PlayerInvetoryText.text = $"{KeySetting.keys[KeyInput.INVENTORY]}";
    }

    public void Initialize()
    {

    }
}
