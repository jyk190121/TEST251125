using JetBrains.Annotations;
using UnityEngine;
/// <summary>
/// 플레이어를 상점으로 이동
/// </summary>
public class HomePortal : MonoBehaviour
{
    [Header("순간이동할 위치")]
    public Transform pos;

    private void OnTriggerEnter(Collider other)
    {
        CharacterController cc = other.GetComponent<CharacterController>();

       

        if (other.CompareTag("Player"))
        {
            cc.enabled = false;
            other.transform.position = pos.transform.position;
            cc.enabled = true;

            cc.Move(Vector3.zero);
        }
    }
}
