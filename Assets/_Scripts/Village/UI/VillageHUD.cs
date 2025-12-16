using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 화면 HUD
/// </summary>
public class VillageHUD : MonoBehaviour
{
    [SerializeField] private Light directionalLight;
    [SerializeField] private GameObject streetLights;

    // 낮 설정
    [SerializeField] private float dayIntensity = 1.2f;
    [SerializeField] private Color dayColor = Color.white;

    // 밤 설정
    [SerializeField] private float nightIntensity = 0.4f;
    [SerializeField] private Color nightColor = new Color(0.3f, 0.3f, 0.6f, 1f);

    private DayManager dayManager;

    private void Start()
    {
        dayManager = _MasterManager.Instance.DayManager;

        if (dayManager == null)
        {
            Debug.LogError("[VillageHUD] DayManager를 찾을 수 없습니다");
            return;
        }

        // Directional Light 자동 검색 (설정되지 않은 경우)
        if (directionalLight == null)
        {
            directionalLight = FindAnyObjectByType<Light>();
            if (directionalLight != null && directionalLight.type != LightType.Directional)
            {
                directionalLight = null;
            }
        }

        if (directionalLight == null)
        {
            Debug.LogError("[VillageHUD] Directional Light를 찾을 수 없습니다");
            return;
        }

        // DayManager 이벤트 구독
        dayManager.OnTimeChanged += OnTimeChanged;
        dayManager.OnDayChanged += OnDayChanged;

        // 초기 UI 업데이트
        UpdateDisplay();
    }

    private void OnDestroy()
    {
        if (dayManager != null)
        {
            dayManager.OnTimeChanged -= OnTimeChanged;
            dayManager.OnDayChanged -= OnDayChanged;
        }
    }

    private void OnTimeChanged(DayManager.TimeOfDay newTime)
    {
        _MasterManager.Instance.SoundManager.StopBGM();
        UpdateDisplay();
    }

    private void OnDayChanged(int newDay)
    {
        _MasterManager.Instance.SoundManager.StopBGM();
        UpdateDisplay();
    }

    /// <summary>
    /// Directional Light의 강도와 색상 조정
    /// </summary>
    private void UpdateDisplay()
    {
        if (directionalLight == null) return;

        if (dayManager.IsDay)
        {
            // 낮: 밝은 흰색 빛
            directionalLight.intensity = dayIntensity;
            directionalLight.color = dayColor;
            streetLights.SetActive(false);

            //사운드
            _MasterManager.Instance.SoundManager.PlayBGM("유정", 0);
        }
        else
        {
            // 밤: 어두운 파란색 빛
            directionalLight.intensity = nightIntensity;
            directionalLight.color = nightColor;
            streetLights.SetActive(true);

            //사운드
            _MasterManager.Instance.SoundManager.PlayBGM("유정", 1);
        }

        Debug.Log($"[VillageHUD] 업데이트: {dayManager.CurrentDay}일 " +
                  $"({(dayManager.IsDay ? "낮" : "밤")})");
    }
}
