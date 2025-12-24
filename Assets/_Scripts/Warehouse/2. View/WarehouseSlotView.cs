using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class WarehouseSlotView : MonoBehaviour, IBeginDragHandler, IDragHandler, IDropHandler, IPointerClickHandler
{
    [Header("UI 컴포넌트")]
    public Image iconImage;
    public TextMeshProUGUI amountText;

    //클릭 시 자신의 인덱스를 담아 보냄
    public event Action<int> OnSlotClick;
    private int myIndex;

    //아이템 드래그 이동 시 보여줄 고스트 아이콘 변수
    private GameObject ghostIconObject;

    public void Initialize(int index)
    {
        myIndex = index;
    }

    //WarehouseView 업데이트
    public void UpdateView(WarehouseSlotModel slotData)
    {
        //화면 갱신될 때, 혹시 남아있는 고스트가 있다면 삭제
        ClearGhostIcon();

        if (slotData.IsEmpty)
        {
            iconImage.enabled = false;
            amountText.text = "";
        }

        else
        {
            iconImage.sprite = slotData.itemDate.icon;
            iconImage.enabled = true;
            iconImage.color = Color.white;  //투명도 복구

            amountText.text = slotData.quantity > 1 ? slotData.quantity.ToString() : "";
        }
    }

    //슬롯 클릭시 인덱스값 전달
    public void OnPointerClick(PointerEventData eventData)
    {
        OnSlotClick?.Invoke(myIndex);
    }

    //아이템 드래그 이동
    public void OnBeginDrag(PointerEventData eventData)
    {
        //좌클릭이 아니면 드래그 못 하도록 미리 방지
        if (eventData.button != PointerEventData.InputButton.Left) return;

        //빈 슬롯 드래그 방지
        if (iconImage.enabled == false) return;

        //WarehousePresenter 호출
        WarehousePresenter.Instance.OnDragStart(myIndex);

        //고스트 아이콘 생성
        CreateGhostIcon();

        //드래그 시 아이템 반 투명 상태
        iconImage.color = new Color(1, 1, 1, 0.5f);
    }

    //드래그 이벤트
    public void OnDrag(PointerEventData eventData)
    {
        if(ghostIconObject != null)
        {
            //마우스 좌표 변환
            Vector3 globalMousePos;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                GetComponentInParent<Canvas>().transform as RectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out globalMousePos))
            {
                ghostIconObject.transform.position = globalMousePos;
            }
        }
    }

    //드래그 이벤트 끝
    public void OnEndDrag(PointerEventData eventData)
    {
        //고스트 아이콘 삭제
        ClearGhostIcon();

        //아이템 투명도 복구
        iconImage.color = new Color(1, 1, 1, 1f);

        //드래그 종료, -1 초기화 관리자 호출
        WarehousePresenter.Instance.OnDragEnd(-1);
    }

    //드롭 이벤트
    public void OnDrop(PointerEventData eventData)
    {
        //드롭 처리 관리자 호출
        WarehousePresenter.Instance.OnDragEnd(myIndex);
    }

    //고스트 아이콘 생성
    private void CreateGhostIcon()
    {
        //잔여 고스트 아이콘 삭제
        ClearGhostIcon();

        //빈 오브젝트 생성
        ghostIconObject = new GameObject("GhostIcon");
        
        //if (WarehousePresenter.Instance != null)
        //{
        //    //고스트 아이콘을 지정된 패널 아래에 설정
        //    ghostIconObject.transform.SetParent(WarehousePresenter.Instance.ghostIconParent, false);
        //}
        //else
        //{
        //캔버스 최상단을 부모로 설정(다른 슬롯에 가려지지 않도록)
        Canvas canvas = GetComponentInParent<Canvas>();
        ghostIconObject.transform.SetParent(canvas.transform, false); //부모 설정
        //}

        //부모 설정 후 위치를 현재 슬롯 아이콘 위치로 지정
        ghostIconObject.transform.position = iconImage.transform.position;

        //이미지 컴포넌트 추가 및 복사
        Image ghostImage = ghostIconObject.AddComponent<Image>();
        ghostImage.sprite = iconImage.sprite;
        //슬롯에 있는 아이템 보다 조금 덜 투명하게
        ghostImage.color = new Color(1, 1, 1, 0.8f);

        //고스트 아이콘이 마우스 입력을 가로채지 않도록 raycastTarget을 꺼준다.
        ghostImage.raycastTarget = false;

        //아이콘 크기를 원본과 같은 사이즈로 맞춤
        RectTransform ghostRect = ghostIconObject.GetComponent<RectTransform>();
        ghostRect.sizeDelta = iconImage.rectTransform.sizeDelta;
    }

    //슬롯이 꺼지거나 파괴될 때, 고스트 아이콘이 남아있다면 강제로 삭제
    private void OnDisable()
    {
        //함수로 깔끔하게 정리
        ClearGhostIcon();

        if (iconImage != null)
        {
            //색깔도 원상복구
            iconImage.color = new Color(1, 1, 1, 1f);
        }
    }

    //고스트 아이콘 삭제를 전담하는 함수 (중복 제거용)
    private void ClearGhostIcon()
    {
        if (ghostIconObject != null)
        {
            Destroy(ghostIconObject);
            //참조 비우기
            ghostIconObject = null;
        }
    }
}
