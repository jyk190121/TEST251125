using JetBrains.Annotations;
using UnityEngine;
/// <summary>
/// 플레이어를 상점으로 이동
/// </summary>
public class HomePortal : MonoBehaviour
{
    [Header("순간이동할 위치")]
    public Transform pos;

    bool movingHome = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = pos.transform.position;

            if (movingHome) MovingHome();
            else MovingShop();
        }
    }


    void MovingHome()
    {
        Camera.main.transform.position =
            Vector3.Lerp(Camera.main.transform.position,
            new Vector3(-4,8,1),
            Time.deltaTime);

        movingHome = false;
    }

    void MovingShop()
    {
        Camera.main.transform.position =
          Vector3.Lerp(Camera.main.transform.position,
          new Vector3(-4, 8, -7.5f),
          Time.deltaTime);

        movingHome = true;
    }

}
