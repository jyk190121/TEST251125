using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;

public class ItemSplitPopup : MonoBehaviour
{
    [Header("팝업 UI")]
    public TMP_InputField inputField;       //아이템 수량 입력
    public Slider quantitySlider;           //아이템 수량 슬라이더
    public Button yesButton;                //확인 버튼
    public Button noButton;                 //취소 버튼
    public TextMeshProUGUI titleText;       //아이템 이름 표시
    public TextMeshProUGUI maxQuantityText; //아이템 최대 수량
    public Image iconImage;                 //아이템 이미지

    //팝업 수량을 돌려줄 콜백 함수
    private Action<int> onConfirmCallback;

    //현재 아이템 최대 수량
    private int maxQuantity;

    private void Awake()
    {
        //입력값이 변경되면 이벤트 실행
        inputField.onValueChanged.AddListener(OnInputFieldValueChanged);

        //슬러이더값이 변경되면 이벤트 실행
        quantitySlider.onValueChanged.AddListener(OnSliderValueChanged);

        //버튼 이벤트
        yesButton.onClick.AddListener(OnConfirm);
        noButton.onClick.AddListener(ClosePopup);
    }

    public void OpenPopup(Item item, string itemName, int maxCount, Action<int> onYes)
    {
        //팝업창 오픈
        gameObject.SetActive(true);

        iconImage.sprite = item.icon;
        titleText.text = $"{itemName}";        
        maxQuantity = maxCount;
        maxQuantityText.text = $"{maxCount}";
        onConfirmCallback = onYes;

        //초기화
        quantitySlider.minValue = 1;
        quantitySlider.maxValue = maxCount;
        quantitySlider.value = 1;
        inputField.text = "1";
    }

    //슬라이드 이동 시 inputField 값 변경
    private void OnSliderValueChanged(float value)
    {
        inputField.text = ((int)value).ToString();
    }

    //inputField 값 변경 시 슬라이드 변경
    private void OnInputFieldValueChanged(string value)
    {
        if (int.TryParse(value, out int result))
        {
            //최대 / 최소 범위 체한
            result = Mathf.Clamp(result, 1, maxQuantity);

            //텍스트가 다르면 갱신
            if (quantitySlider.value != result)
            {
                quantitySlider.value = result;
            }
        }
        
        else
        {
            //숫자가 아니면 1로 초기화 (문자 입력 시)
            inputField.text = "1";
        }
    }

    //확인 버튼
    private void OnConfirm()
    {
        if(int.TryParse(inputField.text, out int count))
        {
            // 콜백으로 수량 전달
            onConfirmCallback?.Invoke(count);
        }
        ClosePopup();
    }

    //팝업창 닫기
    public void ClosePopup()
    {
        gameObject.SetActive(false);
    }
}
