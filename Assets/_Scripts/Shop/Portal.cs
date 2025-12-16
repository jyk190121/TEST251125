using JetBrains.Annotations;
using UnityEngine;
/// <summary>
/// 플레이어를 상점 or 홈으로 이동
/// </summary>
public class HomePortal : MonoBehaviour
{
    [Header("순간이동할 위치")]
    public Transform pos;

    POS_playerSalas pos_palyer;

    private void OnTriggerEnter(Collider other)
    {
        CharacterController cc = other.GetComponent<CharacterController>();
        pos_palyer = FindAnyObjectByType<POS_playerSalas>();

        if (other.CompareTag("Player") && !pos_palyer.shopOpenCheck)
        {
            cc.enabled = false;
            other.transform.position = pos.transform.position;
            cc.enabled = true;

            cc.Move(Vector3.zero);
        }
        else
        {
            print("손님이 와있다");
        }
    }
}
