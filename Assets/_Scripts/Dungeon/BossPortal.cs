using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BossPortal : MonoBehaviour
{
    [Header("설정")]
    [SerializeField] private string SceneName = "Villiage";
    [SerializeField] private float holdDuration; // 필요 유지 시간

    private float timer = 0f;
    private bool isPlayerIn = false;

    public Image image;                 // 키입력하는 동안 띄울 이미지
    public Image key;                   // 상호작용 키 알려줄 이미지


    private void Start()
    {
        image.gameObject.SetActive(false);
        key.gameObject.SetActive(false);
        holdDuration = 3.0f;
        image.fillAmount = 0f;
    }

    private void Update()
    {

        // 1. 플레이어가 포탈 안에 있고 G 키를 누르고 있는 경우
        if (isPlayerIn)
        {
            key.gameObject.SetActive(true);

            if (Input.GetKey(KeySetting.keys[KeyInput.INTERACTIVE]))
            {
                timer += Time.deltaTime;
                key.gameObject.SetActive(false);
                image.gameObject.SetActive(true);
                image.fillAmount = (timer / holdDuration);

                Debug.Log($"복귀 중... {timer:F1}초");

                // 2. 3초를 채웠을 때
                if (timer >= holdDuration)
                {
                    ReturnToTown();
                }
            }
        }
        if (!isPlayerIn || Input.GetKeyUp(KeySetting.keys[KeyInput.INTERACTIVE]))
        {
            // 키를 떼거나 포탈을 나가면 타이머 초기화
            timer = 0f;
            image.gameObject.SetActive(false);
            key.gameObject.SetActive(false);
            image.fillAmount = 0f;
            return;
        }
    }

    private void ReturnToTown()
    {
        // 데이터 저장 및 상태 변경
        DataManager dataManager = _MasterManager.Instance.DataManager;
        if (dataManager != null)
        {
            dataManager.ChangeReturn(true);
        }
        SceneManager.LoadScene(SceneName);
        Debug.Log("3초 유지 완료! 마을로 이동합니다.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerIn = true;
            Debug.Log("포탈 진입: G 키를 3초간 누르세요.");
            key.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerIn = false;
            timer = 0f; // 나가면 초기화
            key.gameObject.SetActive(false);
        }
    }

   

}
