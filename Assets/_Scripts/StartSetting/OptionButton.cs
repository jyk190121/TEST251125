using UnityEngine;
using UnityEngine.UI;

public class OptionButton : MonoBehaviour
{
    [Header("UI 버튼")]
    public Button soundButton;
    public Button inputButton;

    [Header("옵션 패널")]
    public GameObject soundPanel;
    public GameObject inputPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //버튼 클릭 시 실행될 함수 연결
        if (soundButton != null)
            soundButton.onClick.AddListener(OpenSoundSetting);

        if (inputButton != null)
            inputButton.onClick.AddListener(OpenInputSetting);

        //처음에 창을 열 때 기본적으로 '소리 설정'을 먼저 보여주기
        OpenSoundSetting();
    }

    //소리 설정 열기 (인풋 닫기)
    public void OpenSoundSetting()
    {
        if (soundPanel != null) soundPanel.SetActive(true);  //켜기
        if (inputPanel != null) inputPanel.SetActive(false); //끄기

        Debug.Log("소리 설정 탭 활성화");
    }

    //인풋 설정 열기 (소리 닫기)
    public void OpenInputSetting()
    {
        if (soundPanel != null) soundPanel.SetActive(false); // 끄기
        if (inputPanel != null) inputPanel.SetActive(true);  // 켜기

        Debug.Log("키 설정 탭 활성화");
    }
}
