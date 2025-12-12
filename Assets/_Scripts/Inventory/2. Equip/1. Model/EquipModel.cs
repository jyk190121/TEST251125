using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class EquipModel
{
    //장비 슬롯 배열 (0: 무기, 1: 투구, 2: 갑옷, 3: 신발)
    private Item[] equipSlots = new Item[4];

    //장비 슬롯 인덱스 상수
    public const int SLOT_WEAPON = 0;
    public const int SLOT_HEAD = 1;
    public const int SLOT_BODY = 2;
    public const int SLOT_FOOT = 3;    

    //장비 장착
    public void SetEquip(int index, Item item)
    {
        //유요한 인덱스인지 확인
        if (index >= 0 && index < equipSlots.Length)
        {
            //맞으면 아이템 장착
            equipSlots[index] = item;            
        }
        //Debug.Log($"공격력 {stat.att}");
        //Debug.Log($"체력 {stat.hp}");
        //Debug.Log($"방어 {stat.def}");
        //Debug.Log($"속도 {stat.spd}");
        _MasterManager.Instance.DataManager.playerStatChanged(CalculateTotalStats());
    }

    //장비 해제
    public Item Unequip(int index)
    {
        //유요한 인덱스인지 확인
        if(index >= 0 && index < equipSlots.Length)
        {
            //맞으면 아템 해제
            Item item = equipSlots[index];

            //Debug.Log($"공격력 {stat.att}");
            //Debug.Log($"체력 {stat.hp}");
            //Debug.Log($"방어 {stat.def}");
            //Debug.Log($"속도 {stat.spd}");

            //슬롯을 null로 비우기
            equipSlots[index] = null;

            _MasterManager.Instance.DataManager.playerStatChanged(CalculateTotalStats());
            //해제한 아이템 반환
            return item;
        }                
        //유요하지 않으면 null반환
        return null;
    }

    //현재 장착된 아이템 가져오기
    public Item GetEquip(int index)
    {
        if (index >= 0 && index < equipSlots.Length)
            return equipSlots[index];
        return null;
    }

    //전체 장비 배열을 가져오는 함수 (UI 전체 갱신용)
    public Item[] GetAllEquips() => equipSlots;

    //(int att, int hp, int def, int spd)
    public StatStruct CalculateTotalStats()        
    {
        StatStruct stat = new StatStruct();

        //배열을 순회하며 장착된 아이템들의 능력치를 모두 더합니다.
        foreach (var item in equipSlots)
        {
            //빈 슬롯은 패스
            if (item != null)
            {
                stat.att += item.attack;
                stat.hp += item.hpPlus;
                stat.def += item.defense;                
                stat.spd += item.speed;
            }
        }
        return stat;        
    }
}
