using UnityEngine;
using System.Collections;

/// <summary>
/// 레이저 길이를 Z축으로 증가시키는 스크립트
/// - 소환 위치/회전 고정
/// - Z 스케일만 증가
/// - Grow → 유지 → 자동 제거
/// </summary>
public class LaserGrow : MonoBehaviour
{
    [Header("Laser Settings")]
    [Tooltip("레이저 최대 길이 (Z 스케일)")]
    public float maxLength = 20f;

    [Tooltip("레이저가 늘어나는 시간")]
    public float growDuration = 0.5f;

    [Tooltip("최대 길이 유지 시간")]
    public float holdDuration = 1.5f;

    Vector3 startScale;

    void OnEnable()
    {
        // 시작 스케일 저장
        startScale = transform.localScale;

        // Z를 0으로 시작
        transform.localScale = new Vector3(
            startScale.x,
            startScale.y,
            0f
        );

        StartCoroutine(GrowRoutine());
    }

    IEnumerator GrowRoutine()
    {
        float timer = 0f;

        // 🔹 1️⃣ Grow 구간 (0 → maxLength)
        while (timer < growDuration)
        {
            timer += Time.deltaTime;
            float t = timer / growDuration;

            float z = Mathf.Lerp(0f, maxLength, t);

            transform.localScale = new Vector3(
                startScale.x,
                startScale.y,
                z
            );

            yield return null;
        }

        // 🔹 길이 고정
        transform.localScale = new Vector3(
            startScale.x,
            startScale.y,
            maxLength
        );

        // 🔹 2️⃣ 유지 구간
        yield return new WaitForSeconds(holdDuration);

        // 🔹 3️⃣ 종료
        Destroy(gameObject);
    }
}






