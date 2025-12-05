using Unity.VisualScripting;
using UnityEngine;

//유저 데이터를 저장하는 공간 - 던전 진행도/골드/체력 등
public class DataManager : MonoBehaviour
{
    //플레이어 정보 수정 관련
    PlayerModel modelstat;      //장비를 착용하지 않은 기본 스탯
    PlayerModel player;         //플레이어의 정보를 담을 그릇


    //던전 정보 관련
    int dungeonCleared = 0;

    public void Initialize()
    {
        modelstat = PlayerModel.SetStat();
        player = PlayerModel.SetStat();     
    }

    //플레이어 아이템 장착시 스탯 변경
    public void playerStatChanged(int att, int def, int hp, int mspd = 0, int aspd)
    {
        //이후 장비 관련 변수 추가시 수정 필요
        player.HP = modelstat.HP + hp;
        player.MaxHP = modelstat.HP + hp;
        player.ATT = modelstat.ATT + att;
        player.Defend = modelstat.Defend + def;
        player.moveSpeed = modelstat.moveSpeed + mspd;
        player.attackSpeed = modelstat.attackSpeed + mspd;
    }

    //체력 회복
    public void AddHP(int amount)
    {
        player.HP += amount;
        if(player.HP > player.MaxHP)
        {
            player.HP = player.MaxHP;
        }
    }
    //체력 감소
    public void MinusHP(int amount)
    {
        player.HP -= amount;
        if(player.HP < 0)
        {
            player.HP = 0;
        }
    }
    //돈 벌었을때
    public void EarnMoney(int amount)
    {
        player.Money += amount;
    }
    //돈 썼을 때
    public void SpendMoney(int amount)
    {
        player.Money -= amount;
    }

    //던전 클리어 정보 갱신
    public void DugeonClear(int clearLevel)
    {
        dungeonCleared = clearLevel;
    }
    public PlayerModel GetStat()
    {
        return player;
    }
}
