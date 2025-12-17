using UnityEngine;
using UnityEngine.UIElements;

public class StartSetting : MonoBehaviour
{
    UIDocument document;
    VisualElement panel;

    Button continueBtn;  //이어하기
    Button startBtn;        //처음 시작
    Button optionBtn;       //설정
    Button exitBtn;         //게임 종료

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        document = GetComponent<UIDocument>();
        panel = document.rootVisualElement;

        continueBtn = panel.Q<Button>("ContinueBtn");
        startBtn = panel.Q<Button>("StartBtn");
        optionBtn = panel.Q<Button>("OptionBtn");
        exitBtn = panel.Q<Button>("ExitBtn");

        continueBtn.clickable.clicked += OnContinueButtonClicked;
        startBtn.clickable.clicked += StartMoonlighter;
        optionBtn.clickable.clicked += Option;
        exitBtn.clickable.clicked += Exit;

        UpdateContinueButton();
    }

    /// <summary>
    /// 새 게임 시작 버튼 (처음 시작)
    /// </summary>
    void StartMoonlighter()
    {
        if (SaveManager.HasSaveData())
        {
            // TODO: "기존 세이브가 있습니다. 덮어쓰시겠습니까?" 팝업 표시
            // 확인 후 진행
        }

        // 새 게임 시작: DataManager 초기화
        _MasterManager.Instance.DataManager.Initialize();

        Debug.Log("새 게임 시작!");
        GameSceneManager.game.LoadScene("Villiage");
    }

    /// <summary>
    /// 이어하기
    /// </summary>
    private void OnContinueButtonClicked()
    {
        Debug.Log("[StartSetting] 이어하기 클릭");

        if (_MasterManager.Instance.DataManager == null)
        {
            Debug.LogError("DataManager를 찾을 수 없습니다!");
            return;
        }

        Debug.Log("[StartSetting] DataManager 확인됨");

        if (!SaveManager.HasSaveData())
        {
            Debug.LogError("저장 데이터가 없습니다!");
            return;
        }

        Debug.Log("[StartSetting] 저장 데이터 확인됨, LoadGame 호출");

        _MasterManager.Instance.SaveManager.LoadGame(_MasterManager.Instance.DataManager);

        Debug.Log("[StartSetting] LoadGame 완료, 씬 이동");
        _MasterManager.Instance.GameSceneManager.LoadScene("Villiage");
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

    // <summary>
    /// 이어하기 버튼 활성화/비활성화
    /// </summary>
    private void UpdateContinueButton()
    {
        bool hasSaveData = SaveManager.HasSaveData();

        continueBtn.SetEnabled(hasSaveData);

        Debug.Log("저장 데이터 존재: " + hasSaveData);
    }
}
