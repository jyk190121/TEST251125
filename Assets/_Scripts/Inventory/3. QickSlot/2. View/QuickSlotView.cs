using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class QuickSlotView : MonoBehaviour, IPointerClickHandler, IDropHandler
{
    [Header("UI 컴포넌트")]
    public Image iconImage;
    public Image iconImage2;
    public TextMeshProUGUI amountText;
    public TextMeshProUGUI amountText2;

    private int myIndex;

    public void Initialize(int index)
    {
        myIndex = index;
    }

    public void UpdateSlotView(Item item, int count)
    {
        if (item != null && count > 0)
        {
            iconImage.sprite = item.icon;            
            iconImage.enabled = true;            
            iconImage.preserveAspect = true; //이미지 비율 유지 (찌그러짐/거대화 방지)
            
            iconImage2.sprite = item.icon;
            iconImage2.enabled = true;
            iconImage2.preserveAspect = true; //이미지 비율 유지 (찌그러짐/거대화 방지)

            amountText.text = count > 1 ? count.ToString() : "";            
            amountText.enabled = true;

            amountText2.text = count > 1 ? count.ToString() : "";
            amountText2.enabled = true;
        }
        else
        {
            iconImage.enabled = false;
            amountText.text = "";
            amountText.enabled = false;

            iconImage2.enabled = false;
            amountText2.text = "";
            amountText2.enabled = false;
        }
    }

    // 인벤토리 -> 퀵슬롯 드롭
    public void OnDrop(PointerEventData eventData)
    {
        Item draggedItem = InventoryManager.Instance.GetDraggedItem();
        if (draggedItem != null)
        {
            //Presenter에게 등록 요청
            QuickSlotPresenter.Instance.RegisterItem(myIndex, draggedItem);
        }
    }

    // 클릭 시 사용/해제
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (iconImage.enabled)
                QuickSlotPresenter.Instance.UseQuickSlotItem(myIndex);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (iconImage.enabled)
                QuickSlotPresenter.Instance.UnregisterItem(myIndex);
        }
    }
}