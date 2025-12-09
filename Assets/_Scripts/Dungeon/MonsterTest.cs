using UnityEngine;

public class MonsterTest : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "Player")
        {
            Dead();
        }
    }
    void Dead()
    {
        gameObject.SetActive(false);
    }
             
     
}
