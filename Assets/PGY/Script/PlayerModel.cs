using UnityEngine;

//플레이어 기본 정보에 대한 데이터 스크립트
public class PlayerModel
{
    public int HP;
    public int MaxHP;
    public int Money;
    public int ATT;
    public int Defend;
    public int moveSpeed;

    public PlayerModel(int hp, int maxhp, int money, int att, int defend, int moveSpeed)
    {
        HP = hp;
        MaxHP = maxhp;
        Money = money;
        ATT = att;
        Defend = defend;
        this.moveSpeed = moveSpeed;
    }
    public static PlayerModel SetStat()
    {
        return new PlayerModel(
            100,
            100,
            100,
            25,
            10,
            2
        );
    }
}


