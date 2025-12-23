using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;
using Image = UnityEngine.UI.Image;

public class Intro : MonoBehaviour
{

    [Header("인트로 오브젝트")]
    public Image intro_1;
    public Image intro_2;
    public Image intro_3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(FadeTextToFullAlpha(1.5f, intro_1, intro_2, intro_3));
    }

    void Update()
    {
    }

    public IEnumerator FadeTextToFullAlpha(float t, Image i, Image j, Image l)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        j.color = new Color(j.color.r, j.color.g, j.color.b, 0);
        l.color = new Color(j.color.r, j.color.g, j.color.b, 0);

        //1번 이미지 출력
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            yield return null;
        }
        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            yield return null;
        }

        //2번 이미지 출력
        while (j.color.a < 1.0f)
        {
            j.color = new Color(j.color.r, j.color.g, j.color.b, j.color.a + (Time.deltaTime / t));
            yield return null;
        }
        j.color = new Color(j.color.r, j.color.g, j.color.b, 1);
        while (j.color.a > 0.0f)
        {
            j.color = new Color(j.color.r, j.color.g, j.color.b, j.color.a - (Time.deltaTime / t));
            yield return null;
        }

        //3번 이미ㅣ 출력
        while (l.color.a < 1.0f)
        {
            l.color = new Color(l.color.r, l.color.g, l.color.b, l.color.a + (Time.deltaTime / t));
            yield return null;
        }
        l.color = new Color(l.color.r, l.color.g, l.color.b, 1);
        while (l.color.a > 0.0f)
        {
            l.color = new Color(l.color.r, l.color.g, l.color.b, l.color.a - (Time.deltaTime / t));
            yield return null;
        }

        SceneManager.LoadScene("StartScene");
    }
}