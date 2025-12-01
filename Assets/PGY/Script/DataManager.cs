using Unity.VisualScripting;
using UnityEngine;

//유저 데이터를 저장하는 공간 - 던전 진행도/골드/체력 등
public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    PlayerModel player;         //플레이어의 정보를 담을 그릇 -> 기본 체력, 돈, 공격력 등이 담겨있음.

    private void Awake()        //인스턴스 지정
    {
        Instance = this;
    }
    void Start()
    {
        player = PlayerModel.SetStat();     //player에 기본 스탯 설정
    }

    public PlayerModel GetPlayer()          //다른 스크립트에서도 PlayerModel 변수명 = DataManager.Instance.GetPlayer(); 로 접근 가능, 변수명.HP 변수명.MP 등
    {
        return player;
    }

    public void ChangePlayerStat()
    {
        
    }
}
