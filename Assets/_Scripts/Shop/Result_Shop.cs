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
    public GameObject[] resultSlots = new GameObject[8];               // 각 아이템 슬롯 부모
    public Image[] itemSellImage = new Image[8];                       //판매한 아이템 이미지
    public TextMeshProUGUI[] itemSellCount = new TextMeshProUGUI[8];   //판매한 아이템 갯수 UI
    public TextMeshProUGUI[] itemSellPrice = new TextMeshProUGUI[8];   //판매한 아이템 가격 UI
    public Button closeBtn;
    public TextMeshProUGUI closeTxt;

    [SerializeField]
    GridLayoutGroup gridLayout;

    //실제 UI들이 존재하는 오브젝트
    public GameObject uiRoot;

    int[] resultCount;
    int[] resultPrice;

    void Awake()
    {
        resultCount = new int[8];
        resultPrice = new int[8];
        //uiRoot.SetActive(false);

        closeBtn.onClick.AddListener(CloseResultSell);
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

        closeTxt.text = $"닫기 [{KeySetting.keys[KeyInput.CANCLE]}]";

        var results = SalesResultManager.Instance.GetAllResults();
        int activeCount = GetActiveResultCount(results);

        UpdateGridLayout(activeCount);

        RefreshAllUI();
    }
    public void CloseResultSell()
    {
        uiRoot.SetActive(false);
    }
    public void RefreshAllUI()
    {
        for (int i = 0; i < resultSlots.Length; i++) UpdateResultUI(i);
    }
    void UpdateResultUI(int slotIndex)
    {
        uiRoot.SetActive(true);
        List<SellItemData> results = SalesResultManager.Instance.GetAllResults();

        //print($"판매결과 :{results[slotIndex]}");
        //if (slotIndex >= results.Count)
        //{
        //    resultSlots[slotIndex].SetActive(false);
        //    return;
        //}

        //SellItemData data = results[slotIndex];

        //if (data == null || data.soldCount <= 0)
        //{
        //    resultSlots[slotIndex].SetActive(false);
        //    return;
        //}

        //// 해당 슬롯에 판매된 아이템이 없는 경우
        //if (results[slotIndex].item == null || results[slotIndex].soldCount <= 0)
        //{
        //    resultSlots[slotIndex].SetActive(false);
        //    results.Remove(results[slotIndex]);
        //    return;
        //}

        //// 판매된 아이템이 있는 경우
        //else
        //{
        //    resultSlots[slotIndex].SetActive(true);
        //}

        // 범위 체크
        if (slotIndex >= results.Count)
        {
            resultSlots[slotIndex].SetActive(false);
            return;
        }

        SellItemData data = results[slotIndex];

        // 데이터 유효성 체크
        if (data == null || data.item == null || data.soldCount <= 0)
        {
            resultSlots[slotIndex].SetActive(false);
            return;
        }

        // UI 표시
        resultSlots[slotIndex].SetActive(true);

        // 이미지 설정
        itemSellImage[slotIndex].sprite = results[slotIndex].item.icon;

        // 텍스트 설정
        itemSellCount[slotIndex].text = results[slotIndex].soldCount.ToString();
        itemSellPrice[slotIndex].text = results[slotIndex].totalPrice.ToString();
    }

    int GetActiveResultCount(List<SellItemData> results)
    {
        int count = 0;
        foreach (var r in results)
        {
            if (r != null && r.item != null && r.soldCount > 0)
                count++;
        }
        return count;
    }
    void UpdateGridLayout(int activeCount)
    {
        if (activeCount >= 5)
        {
            gridLayout.cellSize = new Vector2(350, 100);
            gridLayout.spacing = new Vector2(50, 50);
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = 2;
        }
        else
        {
            gridLayout.cellSize = new Vector2(600, 100);
            gridLayout.spacing = new Vector2(0, 50);
            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = 1;
        }
    }

}
