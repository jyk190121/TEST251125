using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QickSlotView : MonoBehaviour, IPointerClickHandler
{
    [Header("UI 컴포넌트")]
    public Image iconImage;             //아이콘 이미지
    public TextMeshProUGUI amountText;  //수량 텍스트

    //클릭 시 자신의 인덱스를 실어 보냄
    public event Action<int> OnSlotClick;
    private Item currentItem;
    private int myIndex;


    public void SetItem(Item item, int count)
    {
        currentItem = item;
        iconImage.sprite = item.icon;
        iconImage.enabled = true;
        amountText.text = count > 1 ? count.ToString() : "";
    }

    public void ClearSlot()
    {
        currentItem = null;
        iconImage.sprite = null;
        iconImage.enabled = false;
        amountText.text = "";
        amountText.enabled = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(currentItem == null) return;

        //좌클릭: 아이템 사용
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            
        }

        //우클릭: 아이템 등록 해제
        else if (eventData.button == PointerEventData.InputButton.Right)
        {

        }
    }
}
