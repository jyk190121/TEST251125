using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class BossHPBar : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] Image fillImage;

    IHPProvider currentBoss;
    float maxHP;

    void Awake()
    {
        Hide();
    }

    void LateUpdate()
    {
        if (currentBoss == null)
        {
            FindBoss();
            return;
        }

        if (!currentBoss.IsAlive)
        {
            currentBoss = null;
            Hide();
            return;
        }

        fillImage.fillAmount = currentBoss.CurrentHP / maxHP;
    }

    void FindBoss()
    {
        currentBoss = Object
            .FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
            .OfType<IHPProvider>()
            .FirstOrDefault();

        if (currentBoss == null)
            return;

        maxHP = currentBoss.MaxHP;
        Show();
    }

    void Show()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = false;
    }

    void Hide()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }
}

