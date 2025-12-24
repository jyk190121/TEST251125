using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static RegisteredItem;

using System.Collections.Generic;
/// <summary>
/// 판매한 아이템 전체 결과 보여주기
///  - 아이템 이미지
///  - 아이템 갯수 (count)
///  - 아이템 가격 (count*price)
/// </summary>
public class Result_Shop : MonoBehaviour
{
    public GameObject[] resultSlots = new GameObject[4];               // 각 아이템 슬롯 부모
    public Image[] itemSellImage = new Image[4];                       //판매한 아이템 이미지
    public TextMeshProUGUI[] itemSellCount = new TextMeshProUGUI[4];   //판매한 아이템 갯수 UI
    public TextMeshProUGUI[] itemSellPrice = new TextMeshProUGUI[4];   //판매한 아이템 가격 UI

    [SerializeField]
    RegisteredItem registeredItem;

    public GameObject uiRoot;

    int[] resultCount;
    int[] resultPrice;

    void Awake()
    {
        resultCount = new int[4];
        resultPrice = new int[4];
        //uiRoot.SetActive(false);
    }

    //void OnEnable()
    //{
    //    SaleEvent.OnItemSold += OnItemSold;

    //    RefreshAllUI();
    //}


    //void OnDisable()
    //{
    //    SaleEvent.OnItemSold -= OnItemSold;
    //}

    //void OnItemSold(int slotIndex, int soldCount, int price)
    //{
    //    resultCount[slotIndex] += soldCount;
    //    resultPrice[slotIndex] += soldCount * price;

    //    SaleEvent.ItemSold(slotIndex, resultCount[slotIndex], resultPrice[slotIndex]);

    //    RefreshAllUI();
    //}

    //public int GetResultCount(int slotIndex) => resultCount[slotIndex];

    public void OpenResultSell()
    {
        uiRoot.SetActive(true);
        RefreshAllUI();
    }
    public void CloseResultSell()
    {
        uiRoot.SetActive(false);
    }
    void RefreshAllUI()
    {
        for (int i = 0; i < resultSlots.Length; i++) UpdateResultUI(i);
    }
    void UpdateResultUI(int slotIndex)
    {
        uiRoot.SetActive(true);

        // 판매된 아이템이 있는 경우
        resultSlots[slotIndex].SetActive(true);
       
        List<SellItemData> results = SalesResultManager.Instance.GetAllResults();

        // 해당 슬롯에 판매된 아이템이 없는 경우
        if (results[slotIndex].soldCount <= 0)
        {
            resultSlots[slotIndex].SetActive(false);
            return;
        }

        foreach (SellItemData data in results)
        {
            // 슬롯 하나 할당
            results[slotIndex].item.icon = data.item.icon;
            results[slotIndex].soldCount = data.soldCount;
            results[slotIndex].totalPrice = data.totalPrice;
        }

        // 이미지 설정
        itemSellImage[slotIndex].sprite = results[slotIndex].item.icon;

        // 텍스트 설정
        itemSellCount[slotIndex].text = results[slotIndex].soldCount.ToString();
        itemSellPrice[slotIndex].text = results[slotIndex].totalPrice.ToString();
    }

}
