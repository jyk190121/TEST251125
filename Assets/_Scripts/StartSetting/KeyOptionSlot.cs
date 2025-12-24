using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshProUGUI를 사용하기 위해 필요
using System;

public class KeyOptionSlot : MonoBehaviour
{
    [Header("UI 연결")]
    public TextMeshProUGUI actionText;  // 행동 이름 텍스트
    public TextMeshProUGUI keyText;     // 키 이름 텍스트
    public Button targetButton;         // 클릭할 버튼

    private int myKeyIndex; // 내가 담당하는 키 번호

    // 초기화 함수
    public void Initialize(int index, string actionName, KeyCode currentKey, Action<int, KeyOptionSlot> managerOnClick)
    {
        myKeyIndex = index;
        actionText.text = actionName;
        keyText.text = currentKey.ToString();

        // 버튼 기능 연결 (기존 연결 제거 후 새로 연결)
        targetButton.onClick.RemoveAllListeners();
        targetButton.onClick.AddListener(() => managerOnClick(myKeyIndex, this));
    }

    // 키 텍스트 갱신 함수
    public void UpdateKeyText(KeyCode newKey)
    {
        keyText.text = newKey.ToString();
        Debug.Log("UI 텍스트 갱신 완료");
    }
}