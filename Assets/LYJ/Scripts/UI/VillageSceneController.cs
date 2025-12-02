using UnityEngine;

/// <summary>
/// 마을 씬 전체 제어
/// 플레이어 이동 기반으로 상호작용 처리
/// </summary>
public class VillageSceneController : MonoBehaviour
{
    private DayManager dayManager;
    private VillageSystemManager villageSystemManager;

    private void Start()
    {
        dayManager = DayManager.Instance;
        villageSystemManager = VillageSystemManager.Instance;

        if (dayManager == null)
        {
            Debug.LogError("[VillageSceneController] DayManager를 찾을 수 없습니다");
            return;
        }

        if (villageSystemManager == null)
        {
            Debug.LogError("[VillageSceneController] VillageSystemManager를 찾을 수 없습니다");
            return;
        }

        Debug.Log("[VillageSceneController] 마을 씬 초기화 완료");
    }

    /// <summary>
    /// 던전 씬 로드
    /// FacilityInteraction에서 호출
    /// </summary>
    public void LoadDungeonScene()
    {
        Debug.Log("[VillageSceneController] 던전으로 이동...");
        // SceneManager.LoadScene("DungeonScene");
    }

    /// <summary>
    /// 시간 변경
    /// </summary>
    public void ChangeTimeOfDay()
    {
        DayManager.TimeOfDay newTime = dayManager.IsDay
            ? DayManager.TimeOfDay.Night
            : DayManager.TimeOfDay.Day;

        dayManager.ChangeTimeOfDay(newTime);
        Debug.Log($"[VillageSceneController] 시간 변경: {newTime}");
    }
}

