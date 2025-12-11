using UnityEngine;
/// <summary>
/// 1. 상점 나가기 (마을로 이동) 
/// 2. 판매중일 떄 못나가게 하기
/// </summary>

public class Move_Village : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            print("플레이어 나가기");
            GameSceneManager.game.LoadScene("Villiage");
        }

    }
}
