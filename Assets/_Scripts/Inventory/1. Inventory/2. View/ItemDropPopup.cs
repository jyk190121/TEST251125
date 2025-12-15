using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ItemDropPopup : MonoBehaviour
{
    [Header("팝업 UI")]
    public TextMeshProUGUI messageText;
    public Button yesButton;
    public Button noButton;

    //팝업이 실행 될 때 행동을 담아둘 변수
    private Action onYesCallback;
    private Action onNoCallback;

    public void OpenPopup(string itemName, Action onYes, Action onNo)
    {
        //텍스트 설정
        messageText.text = $"{itemName}을 버리시겠습니까?";

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

    public void ClosePopup()
    {
        gameObject.SetActive(false);
    }
}
