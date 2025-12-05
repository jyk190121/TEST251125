using UnityEngine;
/// <summary>
/// 1. 상점 나가기 (마을로 이동) 
/// </summary>
public class NewMonoBehaviourScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            print("플레이어 나가기");
            GameSceneManager.game.LoadScene("Shop_VillageSceneTest");
        }
    }
}
