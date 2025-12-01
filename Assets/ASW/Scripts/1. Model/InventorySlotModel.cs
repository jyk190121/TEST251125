using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

[System.Serializable]
public class InventorySlotModel
{
    public Item itemData;       //아이템 데이터
    public int quantity;        //아이템 수량    

    //ItamData가 null이거나 수량이 0보다 아래인지, 빈 슬롯인지 확인
    public bool IsEmpty => itemData == null || quantity <= 0;
  
    //아이템 채우기
    public void Set(Item item, int count)
    {
        this.itemData = item;
        this.quantity = count;
    }

    //슬롯 비우기
    public void Clear()
    {
        itemData = null;
        quantity = 0;
    }
}
