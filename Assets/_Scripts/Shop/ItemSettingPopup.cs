using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static RegisteredItem;

public class ItemSettingPopup : MonoBehaviour
{
    [Header("팝업 UI")]
    public TextMeshProUGUI regiItemText;
    public Button yesButton;
    public Button noButton;
    public Image iconImage;
    public TMP_InputField priceInput;
    public TMP_InputField quantityInput;

    //팝업이 실행 될 때 행동을 담아둘 변수
    private Action onYesCallback;
    private Action onNoCallback;

    private Item currentItem;
    private int maxQuantity;
    private int inventoryIndex;


    public void OpenPopup(Item item, Action onYes, Action onNo)
    {
        //텍스트 설정
        regiItemText.text = $"{item.name} 등록";

        //콜백 저장
        onYesCallback = onYes;
        onNoCallback = onNo;

        //버튼 이벤트 연결 (기존 연결 제거 후 재연결)
        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(() => {
            onYesCallback?.Invoke();
            ClosePopup();
        });

        noButton.onClick.RemoveAllListeners();
        noButton.onClick.AddListener(() => {
            onNoCallback?.Invoke();
            ClosePopup();
        });

        //패널 켜기
        gameObject.SetActive(true);
    }

    public void Open(Item item, int availableCount, int invenIndex)
    {
        gameObject.SetActive(true);

        //텍스트 설정

        currentItem = item;
        maxQuantity = availableCount;
        inventoryIndex = invenIndex;

        iconImage.sprite = item.icon;
        regiItemText.text = $"{item.name} 등록";

        priceInput.text = item.sellPrice.ToString();
        quantityInput.text = "1";
    }

    public void OnClickRegister()
    {
        int price = int.Parse(priceInput.text);
        int quantity = int.Parse(quantityInput.text);

        quantity = Mathf.Clamp(quantity, 1, maxQuantity);

        RegisteredItemData data = new RegisteredItemData(currentItem, quantity, price);
        //{
        //    item = currentItem,
        //    count = quantity,
        //    price = price
        //};

        // 인벤토리 수량 감소
        InventoryManager.Instance.DecreaseItemAtIndex(inventoryIndex, quantity);

        ClosePopup();
    }
    public void ClosePopup()
    {
        gameObject.SetActive(false);
    }

}
