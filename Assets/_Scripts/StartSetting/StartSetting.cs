using UnityEngine;
using UnityEngine.UIElements;

public class StartSetting : MonoBehaviour
{
    UIDocument document;
    VisualElement panel;

    Button startBtn;
    Button optionBtn;
    Button exitBtn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        document = GetComponent<UIDocument>();
        panel = document.rootVisualElement;

        startBtn = panel.Q<Button>("StartBtn");
        optionBtn = panel.Q<Button>("OptionBtn");
        exitBtn = panel.Q<Button>("ExitBtn");

        startBtn.clickable.clicked += StartMoonlighter;
        optionBtn.clickable.clicked += Option;
        exitBtn.clickable.clicked += Exit;


    }

 
    void StartMoonlighter()
    {
        GameSceneManager.game.LoadScene("Villiage");
    }

    void Option()
    {
        //옵션
    }

    void Exit()
    {
        Application.Quit();

        #if UNITY_EDITOR
        // 에디터에서는 플레이 모드 종료
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
