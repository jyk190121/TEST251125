using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 테스트용 낮/밤 변경 버튼
/// 개발 중에만 사용, 배포 전 제거
/// </summary>
public class DayNightTestButton : MonoBehaviour
{
    [SerializeField] private Button toggleDayNightButton;
    [SerializeField] private TextMeshProUGUI statusText;

    private DayManager dayManager;

    private void Start()
    {
        dayManager = _MasterManager.Instance.DayManager;

        if (dayManager == null)
        {
            Debug.LogError("[DayNightTestButton] DayManager를 찾을 수 없습니다");
            return;
        }

        // 버튼 클릭 이벤트 연결
        if (toggleDayNightButton != null)
        {
            toggleDayNightButton.onClick.AddListener(OnToggleDayNightButtonClicked);
        }

        // 초기 상태 표시
        UpdateStatusText();

        // DayManager 이벤트 구독 (변경 감지)
        dayManager.OnTimeChanged += OnTimeChanged;
        dayManager.OnDayChanged += OnDayChanged;

        Debug.Log("[DayNightTestButton] 테스트 버튼 준비 완료!");
    }

    private void OnToggleDayNightButtonClicked()
    {
        // 현재 시간 반대로 변경
        DayManager.TimeOfDay newTime = dayManager.IsDay
            ? DayManager.TimeOfDay.Night
            : DayManager.TimeOfDay.Day;

        dayManager.ChangeTimeOfDay(newTime);

        Debug.Log($"[DayNightTestButton] 버튼 클릭! 변경: {newTime}");
    }

    private void OnTimeChanged(DayManager.TimeOfDay newTime)
    {
        Debug.Log($"[DayNightTestButton] 시간 변경 감지: {newTime}");
        UpdateStatusText();
    }

    private void OnDayChanged(int newDay)
    {
        Debug.Log($"[DayNightTestButton] 날짜 변경 감지: {newDay}일");
        UpdateStatusText();
    }

    private void UpdateStatusText()
    {
        if (statusText != null)
        {
            string timeStr = dayManager.IsDay ? " 낮" : " 밤";
            statusText.text = $"{dayManager.CurrentDay}일 | {timeStr}";
        }
    }

    private void OnDestroy()
    {
        if (dayManager != null)
        {
            dayManager.OnTimeChanged -= OnTimeChanged;
            dayManager.OnDayChanged -= OnDayChanged;
        }

        if (toggleDayNightButton != null)
        {
            toggleDayNightButton.onClick.RemoveListener(OnToggleDayNightButtonClicked);
        }
    }
}

