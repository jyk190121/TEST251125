using System;
using UnityEngine;

public class KeyBindManager : MonoBehaviour
{
    [Header("프리팹 및 부모 연결")]
    public GameObject KeyOptionSlotPrefab;  //프리팹
    public Transform contentArea;           //스크롤 뷰 contentArea

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreateKeyList();
    }
    
    private void CreateKeyList()
    {
        //기존 키 목록 삭제 (초기화)
        foreach (Transform child in contentArea) Destroy(child.gameObject);

        //생성되어 있는 Enum만큼 반복
        for (int i = 0; i < (int)KeyInput.KEYCOUNT; i++)
        {
            KeyInput inputType = (KeyInput)i;

            //현재 설정된 키가 딕셔너리에 있는지 확인
            if (!KeySetting.keys.ContainsKey(inputType)) continue;

            //현재 설정된 키 값 가져오기
            KeyCode currentKey = KeySetting.keys[inputType];

            //프리팹 생성
            GameObject go = Instantiate(KeyOptionSlotPrefab, contentArea);
            
            KeyOptionSlot slot = go.GetComponent<KeyOptionSlot>();
            
            slot.Initialize(i, inputType.ToString(), currentKey, OnSlotClicked);
        }
    }

    //슬롯 버튼 클릭 시 실행
    private void OnSlotClicked(int index, KeyOptionSlot slotUI)
    {
        //이미 변경 중이면 무시
        if (_MasterManager.Instance.InputManager.isRebinding) return;

        //UI에 "입력 중..." 표시
        slotUI.keyText.text = "...";

        //InputManager에게 키 변경 요청
        _MasterManager.Instance.InputManager.ChangeKey(index, (newKey) =>
        {
            // 콜백: 변경 성공 시 UI 텍스트 업데이트
            slotUI.UpdateKeyText(newKey);
        });
    }
}
