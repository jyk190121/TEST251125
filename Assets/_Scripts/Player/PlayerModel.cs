using UnityEngine;

//플레이어 기본 정보에 대한 데이터 스크립트
public class PlayerModel
{
    public int HP;
    public int MaxHP;
    public int Money;
    public int ATT;
    public int Defend;
    public float moveSpeed;
    public float attackSpeed; 

    public PlayerModel(int hp, int maxhp, int money, int att, int defend, float moveSpeed)
    {
        HP = hp;
        MaxHP = maxhp;
        Money = money;
        ATT = att;
        Defend = defend;
        this.moveSpeed = moveSpeed;
        attackSpeed = 0;
    }
    public static PlayerModel SetStat()
    {
        return new PlayerModel(
            100,
            100,
            1000,
            5,
            5,
            12
        );
    }
}


