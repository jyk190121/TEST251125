using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Unity.VisualScripting;
using System.Collections;

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

        if (currentBoss.CurrentHP <= 0)
        {
            fillImage.fillAmount = 0;
            StartCoroutine(HideHPBar());
            return;
        }

        fillImage.fillAmount = currentBoss.CurrentHP / maxHP;
    }

    IEnumerator HideHPBar()
    {
        yield return new WaitForSeconds(3f);
        Hide();
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

