using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static RegisteredItem;

public class SalesResultManager : MonoBehaviour
{

    public static SalesResultManager Instance;

    public List<SellItemData> soldItems = new List<SellItemData>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddSale(Item item, int count, int price)
    {
        SellItemData data = soldItems.Find(x => x.item.itemID == item.itemID);

        if (data == null)
        {
            data = new SellItemData(item);
            soldItems.Add(data);
        }

        data.AddSale(count, price);
    }

    public List<SellItemData> GetAllResults()
    {
        return soldItems;
    }

    public void ClearResults()
    {
        soldItems.Clear();
    }
}
