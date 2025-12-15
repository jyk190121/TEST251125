using UnityEngine;
/// <summary>
/// 1. 상점 나가기 (마을로 이동) 
/// 2. 판매중일 떄 못나가게 하기
/// </summary>

public class Move_Village : MonoBehaviour
{
    POS_playerSalas pos_palyer;

    private void OnTriggerEnter(Collider other)
    {
        pos_palyer = FindAnyObjectByType<POS_playerSalas>();

        if (other.CompareTag("Player") && !pos_palyer.shopOpenCheck)
        {
            print("플레이어 나가기");
            GameSceneManager.game.LoadScene("Villiage");
        }
        else
        {
            print("상점이 열려있어 못나감");
        }

    }
}
