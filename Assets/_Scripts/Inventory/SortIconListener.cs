using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class SortIconListener : MonoBehaviour, IPointerClickHandler
{
    //옵져버 패턴
    public event Action OnClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        //왼쪽 마우스 클릭만 허용
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            //클릭 알림
            OnClick?.Invoke();
        }
    }
}