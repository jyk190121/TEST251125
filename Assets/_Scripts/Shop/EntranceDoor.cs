using UnityEngine;

public class EntranceDoor: MonoBehaviour
{
    [Header("홈상점 프리팹")]
    public GameObject homeShop;

    [Header("마을 프리팹")]
    public GameObject village;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            bool state = !homeShop.activeSelf; // homeShop의 현재 상태 반전
            homeShop.gameObject.SetActive(state);
            village.gameObject.SetActive(!state);

            if (state)
            {
                other.transform.position = other.transform.position - new Vector3(0, 0,-1.5f);
            }
            else
            {
                other.transform.position = other.transform.position - new Vector3(0, 0, 1.5f);
            }
        }
    }
}
