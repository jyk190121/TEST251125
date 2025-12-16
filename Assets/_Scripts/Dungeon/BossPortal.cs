using UnityEngine;
using UnityEngine.SceneManagement;

public class BossPortal : MonoBehaviour
{
    [Header("설정")]
    [SerializeField] private string SceneName = "Villiage";
    [SerializeField] private float holdDuration = 3.0f; // 필요 유지 시간

    private float timer = 0f;
    private bool isPlayerIn = false;

    private void Update()
    {
        // 1. 플레이어가 포탈 안에 있고 T 키를 누르고 있는 경우
        if (isPlayerIn && Input.GetKey(KeyCode.T))
        {
            timer += Time.deltaTime;
            Debug.Log($"복귀 중... {timer:F1}초");

            // 2. 3초를 채웠을 때
            if (timer >= holdDuration)
            {
                ReturnToTown();
            }
        }
        else
        {
            // 키를 떼거나 포탈을 나가면 타이머 초기화
            timer = 0f;
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

        Debug.Log("3초 유지 완료! 마을로 이동합니다.");
        SceneManager.LoadScene(SceneName);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerIn = true;
            Debug.Log("포탈 진입: T 키를 3초간 누르세요.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerIn = false;
            timer = 0f; // 나가면 초기화
        }
    }

   

}
