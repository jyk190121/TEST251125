using Unity.VisualScripting;
using UnityEngine;
using System;

//유저 데이터를 저장하는 공간 - 던전 진행도/골드/체력 등
public class DataManager : MonoBehaviour
{
    //플레이어 정보 수정 관련
    PlayerModel modelstat;      //장비를 착용하지 않은 기본 스탯
    PlayerModel player;         //플레이어의 정보를 담을 그릇

    public Item EquipWeapon;    //장착한 무기

    //던전 정보 관련
    int dungeonCleared = 0;

    public static Action OnEquipmentChanged;
    public static Action OnStatChanged;

    //사망,던전 클리어, 펜던트 사용 이후의 복귀인가?
    bool isReturn = false;
    bool isPendant = false;
    bool isClear = false;

    // 던전 내의 전투 데이터 저장
    BattleRecord BR;


    public void Initialize()
    {
        modelstat = PlayerModel.SetStat();
        player = PlayerModel.SetStat();     
    }

    //플레이어 아이템 장착시 스탯 변경
    public void playerStatChanged(StatStruct stat)
    {
        //이후 장비 관련 변수 추가시 수정 필요
        player.HP = modelstat.HP + stat.hp;
        player.MaxHP = modelstat.HP + stat.hp;
        player.ATT = modelstat.ATT + stat.att;
        player.Defend = modelstat.Defend + stat.def;
        player.moveSpeed = modelstat.moveSpeed + stat.spd;
        player.attackSpeed = modelstat.attackSpeed + stat.spd;

        OnStatChanged?.Invoke();
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
    //얼마있냐
    public int HojuMoney()
    {
        return player.Money;
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

    public void ChangeWeapon(Item newItem)
    {
        EquipWeapon = newItem;

        // 모든 구독자(PlayerControll 등)에게 변경 사항을 알립니다.
        OnEquipmentChanged?.Invoke();
    }

    public Item GetWeapon()
    {
        return EquipWeapon;
    }

    public void ChangeHP(int amount)
    {
        player.HP -= amount;
    }

    //True = 포탈 타고 복귀 false = 그냥 아무것도 발생하지 않는 복귀
    public void ChangeReturn(bool Return)
    {
        isReturn = Return;
        if (isReturn)
        {
            BR.OpenResultPanel(isPendant, isClear);
        }
    }
    public bool GetReturn()
    {
        return isReturn;
    }
    public void SetisPendant(bool Pendant)
    {
        isPendant = Pendant;
    }
    public void SetisClear(bool Clear)
    {
        isClear = Clear;
    }


    //전투 기록 초기화
    public void RegisterBattleRecord(BattleRecord newBR)
    {
        BR = newBR;
        Debug.Log("BR등록함!");
    }
    //사냥한 몬스터 값 추가
    public void GetMonster(MonsterData MD)
    {
        BR.AddMonster(MD);
    }
    //얻은 아이템 값 추가
    public void GetItem(Item item)
    {
        BR.AddItem(item);
    }
}
