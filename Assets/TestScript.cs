using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class TestScript : MonoBehaviour
{
    UIDocument uiDoc;
    VisualElement vi;

    Button startBtn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        uiDoc = GetComponent<UIDocument>();

        //최상위 판넬 가져오기
        vi = uiDoc.rootVisualElement;
        //Btn 가져오기
        startBtn = vi.Q<Button>("StartBtn");
        startBtn.clickable.clicked += OnclickButton;
    }


    void OnclickButton()
    {
        SceneManager.LoadScene("GameScene");
    }
}
