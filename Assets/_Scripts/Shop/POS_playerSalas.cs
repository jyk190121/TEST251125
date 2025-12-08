using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 1. 플레이어가 계산대 위치로 이동했는지 파악
/// 2. CustomerManager에게 플레이어가 판매선택했는지 넘겨줌
/// </summary>
public class POS_playerSalas : MonoBehaviour
{
    public Image image;
    public Transform salasPos;      //손님이 왔을 때 확인 여부

    bool customerCheck;


    private void Start()
    {
        image.gameObject.SetActive(false);
        customerCheck = false;
    }

    private void Update()
    {
        print(customerCheck);

        if (salasPos != null)
        {
            customerCheck = true;
        }
        else
        {
            customerCheck = false;
        }
    }


    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            //판매 열기
            image.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //판매 닫기
            image.gameObject.SetActive(false);
        }
    }
}
