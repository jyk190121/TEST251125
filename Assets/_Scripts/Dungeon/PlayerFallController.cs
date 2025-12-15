using System;
using System.Collections;
using System.Threading;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerFallController : MonoBehaviour
{
    [SerializeField]
    private string TrapLayerName = "Trap";

    [SerializeField]
    private float fallDuration = 1.5f;

    [SerializeField]
    private float fallSpeed = 5f;

    private Vector3 lastSafePosition; // 마지막으로 땅에 닿아있던 안전한 위치를 저장합니다.
    private Vector3 initialScale; // 플레이어 오브젝트의 원래 크기를 저장합니다.

    private int TrapLayer; // 낭떠러지 Layer의 정수형 ID를 저장
    private bool isFall = false; // 낭떠러지 처리 중인지 확인하는 플래그

    CharacterController cc;

    float saveTimer = 0.8f;

    private void Start()
    {
        cc = GetComponent<CharacterController>();
        TrapLayer = LayerMask.NameToLayer(TrapLayerName);
        lastSafePosition = transform.position;
        initialScale = transform.localScale;
    }

    private void Update()
    {
        if (!isFall)
        {
            saveTimer -= Time.deltaTime;
            if(saveTimer < 0f)
            {
                saveTimer = 0.8f;
                lastSafePosition = transform.position;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isFall && other.gameObject.layer == TrapLayer )
        {
            if (this.gameObject.layer == 30) return;
            StartCoroutine(FallAndRespawn());
        }
    }

   IEnumerator FallAndRespawn()
   {
        isFall = true;
        cc.enabled = false;

        float timer = 0f;

        while (timer < fallDuration)
        {
            timer += Time.deltaTime;

            float t = timer / fallDuration;

            transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t);

            transform.position += Vector3.down * fallSpeed * Time.deltaTime;

            yield return null;
        }

        transform.position = lastSafePosition;
        transform.localScale = initialScale;

        cc.enabled = true;

        isFall = false;
    }
}
