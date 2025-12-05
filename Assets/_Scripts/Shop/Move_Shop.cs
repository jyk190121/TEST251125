using UnityEngine;

public class Move_Shop : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            print("플레이어 들어가기");
            GameSceneManager.game.LoadScene("ShopScene");
        }
    }
}
