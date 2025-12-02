using UnityEngine;
using System;

/// <summary>
/// 낮/밤 시간대와 날짜를 관리하는 Manager
/// </summary>
public class DayManager : MonoBehaviour
{
    public static DayManager Instance { get; private set; }

    public enum TimeOfDay
    {
        Day,
        Night
    }

    [SerializeField] private TimeOfDay currentTime = TimeOfDay.Day;
    [SerializeField] private int currentDay = 1;

    public TimeOfDay CurrentTime => currentTime;
    public int CurrentDay => currentDay;

    public event Action<TimeOfDay> OnTimeChanged;
    public event Action<int> OnDayChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 시간대를 변경합니다 (낮 ↔ 밤)
    /// 밤에서 낮으로 변경될 때는 날짜가 1일 증가합니다
    /// </summary>
    public void ChangeTimeOfDay(TimeOfDay newTime)
    {
        if (currentTime == newTime)
            return;

        currentTime = newTime;

        // 밤에서 낮으로 변경되면 날짜 증가
        if (currentTime == TimeOfDay.Day)
        {
            AdvanceDay();
        }

        OnTimeChanged?.Invoke(currentTime);
        Debug.Log($"[DayManager] 시간 변경: {currentTime} | 현재 날짜: {currentDay}");
    }

    private void AdvanceDay()
    {
        currentDay++;
        OnDayChanged?.Invoke(currentDay);
        Debug.Log($"[DayManager] 날짜 변경: {currentDay}일");
    }

    public bool IsDay => currentTime == TimeOfDay.Day;
    public bool IsNight => currentTime == TimeOfDay.Night;

    public void ResetDay()
    {
        currentTime = TimeOfDay.Day;
        currentDay = 1;
    }
}
