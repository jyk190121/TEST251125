using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 1. 플레이어가 계산대 위치로 이동했는지 파악
/// 2. CustomerManager에게 플레이어가 판매선택했는지 넘겨줌
/// </summary>
public class POS_playerSalas : MonoBehaviour
{
    Image image; 
    private void Start()
    {
        image= GetComponentInChildren<Image>();
        image.gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            //판매라는 버튼 띄우기
            image.gameObject.SetActive(true);
        }
        else
        {
            image.gameObject.SetActive(false);
        }
    }
}
