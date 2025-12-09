[System.Serializable]
public class WarehouseSlotModel
{
    public Item itemDate;       //아이템 데이터
    public int quantity;        //아이템 수량

    //ItamData가 null이거나 수량이 0보다 아래인지, 빈 슬롯인지 확인
    public bool IsEmpty => itemDate == null || quantity <= 0;

    //아이템 채우기
    public void Set(Item item, int count)
    {
        this.itemDate = item;
        this.quantity = count;
    }

    //슬롯 비우기
    public void Clear()
    {
        itemDate = null;
        quantity = 0;
    }
}
