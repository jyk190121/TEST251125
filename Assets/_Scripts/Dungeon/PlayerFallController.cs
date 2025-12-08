using System;
using UnityEngine;

public class PlayerFallController : MonoBehaviour
{
    [SerializeField]
    private string TrapLayerName = "Trap";
     
    [SerializeField]
    private Vector3 respawnPoint = Vector3.zero; // 플레이어가 리스폰될 위치입니다.

    private int TrapLayer; // 낭떠러지 Layer의 정수형 ID를 저장
    private bool isFall = false; // 낭떠러지 처리 중인지 확인하는 플래그

    CharacterController cc;

    private void Start()
    {
        cc = GetComponent<CharacterController>();
        TrapLayer = LayerMask.NameToLayer(TrapLayerName);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isFall && other.gameObject.layer == TrapLayer )
        {
            isFall = true; 
            Fall();
            isFall = false;
        }
    }

    private void Fall()
    {
        cc.enabled = false;

        transform.position = respawnPoint;

        cc.enabled = true;
    }
}
