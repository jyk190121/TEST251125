using UnityEngine;
/// <summary>
/// 1. 상점 나가기 (마을로 이동) 
/// 2. 손님 상점이동
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

        //if (other.gameObject.layer == LayerMask.NameToLayer("Customer"))
        //{
        //    print("손님 들어오게 처리");
        //    other.gameObject.transform.position = new Vector3(-0.9f, 0.98f, -10.48f);
        //}
    }
}
