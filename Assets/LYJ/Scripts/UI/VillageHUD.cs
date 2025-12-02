using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// 마을 화면 HUD
/// </summary>
public class VillageHUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private Image backgroundImage;

    // 낮/밤 색상
    [SerializeField] private Color dayColor = Color.white;
    [SerializeField] private Color nightColor = new Color(0.3f, 0.3f, 0.5f);

    private DayManager dayManager;

    private void Start()
    {
        dayManager = DayManager.Instance;

        if (dayManager == null)
        {
            Debug.LogError("[VillageHUD] DayManager를 찾을 수 없습니다");
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
        UpdateDisplay();
    }

    private void OnDayChanged(int newDay)
    {
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (dayText != null)
        {
            dayText.text = $"{dayManager.CurrentDay}일";
        }

        // 배경 색상 변경 (낮/밤)
        if (backgroundImage != null)
        {
            if (dayManager.IsDay)
            {
                backgroundImage.color = dayColor;
            }
            else
            {
                backgroundImage.color = nightColor;
            }
        }

        Debug.Log($"[VillageHUD] 업데이트: {dayManager.CurrentDay}일 " +
                  $"({(dayManager.IsDay ? "낮" : "밤")})");
    }
}
