using System.Collections;
using UnityEngine;

public class PlayerMoveStopTrigger : MonoBehaviour
{
    [SerializeField] private float stopTime = 3f;

    private bool isTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;

        PlayerMove playerMove = other.GetComponent<PlayerMove>();
        if (playerMove == null) return;

        isTriggered = true;
        StartCoroutine(StopMoveCoroutine(playerMove));
    }

    private IEnumerator StopMoveCoroutine(PlayerMove playerMove)
    {
        playerMove.SetActiveMove(false);
        Debug.Log("플레이어 이동 정지");

        yield return new WaitForSeconds(stopTime);

        playerMove.SetActiveMove(true);
        Debug.Log("플레이어 이동 재개");

        Destroy(gameObject);
    }
}
