using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using Image = UnityEngine.UI.Image;

public class FadeIn : MonoBehaviour
{

    [Header("인트로 오브젝트")]
    public Image img;
    public CanvasGroup canvasGroup;

    float fadeDuration = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        FadeEvent();
    }

    void Update()
    {
    }
    void FadeEvent()
    {
        StartCoroutine(LogoImage());
    }

    IEnumerator LogoImage()
    {
        StartCoroutine(FadeTextToFullAlpha(canvasGroup, 0f, 1f));

        yield return new WaitForSeconds(fadeDuration);

        StartCoroutine(FadeTextToFullAlpha(canvasGroup, 1f, 0f));
    }

    public IEnumerator FadeTextToFullAlpha(CanvasGroup cg, float start, float end)
    {
        float elapsed = 0f;

        //1번 이미지 출력
        while (elapsed < fadeDuration)
        { 
            elapsed += Time.deltaTime;

            canvasGroup.alpha = Mathf.Lerp(start, end, elapsed / fadeDuration);

            yield return null;
        }
        canvasGroup.alpha = end;
        cg.gameObject.SetActive(false);
    }
}