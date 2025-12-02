using UnityEngine;
using System.Collections.Generic; 

public class Room : MonoBehaviour
{
    [Header("--- Door References ---")]
    [Tooltip("상(Up, +Z축) 방향에 있는 문 오브젝트를 연결하세요.")]
    public GameObject upDoor;

    [Tooltip("하(Down, -Z축) 방향에 있는 문 오브젝트를 연결하세요.")]
    public GameObject downDoor;

    [Tooltip("좌(Left, -X축) 방향에 있는 문 오브젝트를 연결하세요.")]
    public GameObject leftDoor;

    [Tooltip("우(Right, +X축) 방향에 있는 문 오브젝트를 연결하세요.")]
    public GameObject rightDoor;

    public void InitializeDoors(bool hasUp, bool hasDown, bool hasLeft, bool hasRight)
    {
        if (upDoor != null)
        {
            // SetActive(bool value): 게임 오브젝트를 활성화하거나 비활성화합니다.
            upDoor.SetActive(hasUp);
        }

        // if문: downDoor 오브젝트의 활성화 상태를 `hasDown` 값으로 설정합니다.
        if (downDoor != null)
        {
            downDoor.SetActive(hasDown);
        }

        // if문: leftDoor 오브젝트의 활성화 상태를 `hasLeft` 값으로 설정합니다.
        if (leftDoor != null)
        {
            leftDoor.SetActive(hasLeft);
        }

        // if문: rightDoor 오브젝트의 활성화 상태를 `hasRight` 값으로 설정합니다.
        if (rightDoor != null)
        {
            rightDoor.SetActive(hasRight);
        }

        Debug.Log($"Master, Room at {transform.position} initialized. Doors: U:{hasUp}, D:{hasDown}, L:{hasLeft}, R:{hasRight}");
    }

    public void ActivateRoom()
    {
        
    }
}