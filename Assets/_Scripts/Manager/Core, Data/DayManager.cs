using UnityEngine;
using System;

/// <summary>
/// 낮/밤 시간대와 날짜를 관리하는 Manager
/// </summary>
public class DayManager : MonoBehaviour
{
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

    private bool isInitialized = false;

    public void Initialize()
    {
        if (isInitialized)
        {
            Debug.LogWarning("[DayManager] 이미 초기화됨");
            return;
        }

        currentTime = TimeOfDay.Day;
        currentDay = 1;

        isInitialized = true;

        Debug.Log($"[DayManager] 초기화 완료: {currentDay}일, {currentTime}");

        DataManager.OnDataLoaded += UpdateDayFromData;
    }

    /// <summary>
    /// 저장된 데이터로 시간 업데이트
    /// </summary>
    private void UpdateDayFromData()
    {
        var dm = _MasterManager.Instance.DataManager;
        currentDay = dm.currentDay;
        currentTime = dm.currentTime;

        Debug.Log($"[DayManager] 데이터 로드: Day {currentDay}, Time {currentTime}");
    }


    /// <summary>
    /// 시간대 변경 (낮 ↔ 밤)
    /// 밤에서 낮으로 변경될 때는 날짜가 1일 증가
    /// </summary>
    public void ChangeTimeOfDay(TimeOfDay newTime)
    {
        if (currentTime == newTime)
            return;

        currentTime = newTime;

        // DataManager에 저장
        _MasterManager.Instance.DataManager.SetTimeOfDay(currentTime);

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
        _MasterManager.Instance.DataManager.SetCurrentDay(currentDay);

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
