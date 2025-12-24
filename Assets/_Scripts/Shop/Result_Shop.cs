using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static RegisteredItem;
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

    RegisteredItem registeredItem;

    int[] resultCount;
    int[] resultPrice;

    void Awake()
    {
        resultCount = new int[4];
        resultPrice = new int[4];
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        SaleEvent.OnItemSold += OnItemSold;
    }

    void OnDisable()
    {
        SaleEvent.OnItemSold -= OnItemSold;
    }

    void OnItemSold(int slotIndex, int soldCount, int price)
    {
        gameObject.SetActive(true);
        resultCount[slotIndex] += soldCount;
        resultPrice[slotIndex] += soldCount * price;

        UpdateResultUI(slotIndex);
    }

    void UpdateResultUI(int slotIndex)
    {
        // 🔹 해당 슬롯에 판매된 아이템이 없는 경우
        if (resultCount[slotIndex] <= 0)
        {
            resultSlots[slotIndex].SetActive(false);
            return;
        }

        // 판매된 아이템이 있는 경우
        resultSlots[slotIndex].SetActive(true);

        // 이미지 설정
        itemSellImage[slotIndex].sprite = registeredItem.itemImages[slotIndex].sprite;

        // 텍스트 설정
        itemSellCount[slotIndex].text = resultCount[slotIndex].ToString();
        itemSellPrice[slotIndex].text = resultPrice[slotIndex].ToString();
    }

    public int GetResultCount(int slotIndex) => resultCount[slotIndex];

    public void OpenResultSell()
    {
        gameObject.SetActive(true);
    }
    public void CloseResultSell()
    {
        gameObject.SetActive(false);
    }


}
