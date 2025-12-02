using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

public class InventorySlotView : MonoBehaviour, IBeginDragHandler, IDragHandler, IDropHandler, IPointerClickHandler
{
    [Header("UI 컴포넌트")]
    public Image iconImage;
    public TextMeshProUGUI amountText;

    //클릭 시 자신의 인덱스를 실어 보냄
    public event Action<int> OnSlotClick;
    private int myIndex;

    //아이템 드래그 이동 시 보여줄 고스트 아이콘 변수
    private GameObject ghostIconObject;

    public void Initialize(int index)
    {
        myIndex = index;
    }

    //InventoryView 업데이트
    public void UpdateView(InventorySlotModel slotData)
    {
        //화면 갱신될 때, 혹시 남아있는 고스트가 있다면 삭제 (청소)
        ClearGhostIcon();

        if (slotData.IsEmpty)
        {
            iconImage.enabled = false;
            amountText.text = "";
        }
        else
        {
            iconImage.sprite = slotData.itemData.icon;
            iconImage.enabled = true;
            iconImage.color = Color.white; //[중요] 투명도 복구

            //수량이 1보다 클 때만 숫자 표시
            amountText.text = slotData.quantity > 1 ? slotData.quantity.ToString() : "";
        }
    }

    //아이템 드래그 이동
    public void OnBeginDrag(PointerEventData eventData)
    {
        //좌클릭이 아니면 드래그 못 하도록 미리 방지
        if (eventData.button != PointerEventData.InputButton.Left) return;

        //빈 슬롯 드래그 방지
        if (iconImage.enabled == false) return;

        //인벤토리 매니저 호출
        InventoryManager.Instance.OnDragStart(myIndex);

        //고스트 아이콘 생성
        CreateGhostIcon();

        //드래그 시 아이템 반 투명 상태
        iconImage.color = new Color(1, 1, 1, 0.5f);
    }

    //드래그 이벤트
    public void OnDrag(PointerEventData eventData)
    {
        if (ghostIconObject != null)
        {
            //마우스 좌표를 더 안전하게 변환
            //Screen Space - Camera 모드 등에서도 정확하게 따라다님
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
        //고스트 아이콘 삭제 (안전하게 함수로 처리)
        ClearGhostIcon();

        //아이템 투명도 복구
        iconImage.color = new Color(1, 1, 1, 1f);

        //드래그 종료, 매니저 호출 (-1을 보내서 초기화 유도)
        InventoryManager.Instance.OnDragEnd(-1);
    }

    //드롭 이벤트
    public void OnDrop(PointerEventData eventData)
    {
        //인벤토리 내 다른 슬롯에 아이템 드롭 시 매니저 호출
        InventoryManager.Instance.OnDragEnd(myIndex);
    }

    //아이템 클릭 (사용/장착)
    public void OnPointerClick(PointerEventData eventData)
    {
        //드래그 중이라면 클릭 안되도록 반환
        if (eventData.dragging) return;
        OnSlotClick?.Invoke(myIndex);

        //마우스 우클릭 시 아이템 사용/장착
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            //사용 시 디버그 로그
            Debug.Log($"{myIndex}번 인벤토리 아이템 사용");
            //InventoryManager.Instance.UseItem(myIndex);
        }
    }

    private void CreateGhostIcon()
    {
        //혹시 기존 게 있다면 지우고 시작
        ClearGhostIcon();

        //빈 오브젝트 생성
        ghostIconObject = new GameObject("GhostIcon");

        //캔버스 최상단을 부모로 설정 (다른 슬롯에 가려지지 않도록)
        Canvas canvas = GetComponentInParent<Canvas>();
        ghostIconObject.transform.SetParent(canvas.transform, false); // 부모 설정

        //부모 설정 후 위치를 현재 슬롯 아이콘 위치로 지정
        ghostIconObject.transform.position = iconImage.transform.position;

        //이미지 컴포넌트 추가 및 복사
        Image ghostImage = ghostIconObject.AddComponent<Image>();
        ghostImage.sprite = iconImage.sprite;
        //슬롯에 있는 아이템 보다는 조금 투명하게
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