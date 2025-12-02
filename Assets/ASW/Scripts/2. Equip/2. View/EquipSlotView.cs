using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipSlotView : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    [Header("슬롯 설정")]
    public int slotIndex;               //장비 슬롯 인덱스
    public EquipmentSlot targetType;    //이 슬롯이 담당하는 장비 타입

    [Header("UI 컴포넌트")]
    public Image iconImage;             //아이콘 이미지
    public GameObject emptyIcon;        //빈 슬롯 아이콘

    private Item currentItem;           //현재 장착된 아이템

    public void UpdateSlot(Item item)
    {
        currentItem = item;

        if(item != null)
        {
            //아이템이 있으면 아이콘을 보여준다.
            iconImage.sprite = item.icon;
            iconImage.enabled = true;
            if(emptyIcon != null) emptyIcon.SetActive(false);
        }
        else
        {
            //아이콘이 없으면 아이콘을 숨긴다.
            iconImage.sprite = null;
            iconImage.enabled = false;
            if(emptyIcon != null ) emptyIcon.SetActive(true);
        }
    }
    //인벤토리에서 이 슬롯으로 아이템을 드래그해서 놓았을 때
    public void OnDrop(PointerEventData eventData)
    {
        //해당 슬롯(slotIndex)에, 이곳에 맞는 타입(targetType)을 장착하라고 매니저에게 알림
        EquipManager.Instance.OnDropToEquipSlot(slotIndex, targetType);
    }

    //우클릭 했을 때 (장비 해제)
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            //해당 슬롯(slotIndex)에 있는 거 장비 해제하라고 매니저에게 알림
            EquipManager.Instance.UnEquipItem(slotIndex);
        }
    }
}