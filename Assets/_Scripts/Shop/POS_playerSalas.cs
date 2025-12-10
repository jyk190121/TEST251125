using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class POS_playerSalas : MonoBehaviour
{
    public Image image;
    public Transform salasPos;      //손님이 왔을 때 확인 여부

    public TextMeshProUGUI key;     //상호작용 키
    public TextMeshProUGUI sales;   //문구
    public bool shopOpenCheck;      //상점 열었는지 확인

    public void posUpdate()
    {
        if (shopOpenCheck) sales.text = "판매";
        else sales.text = "판매 시작";
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            //판매 UI 열기
            image.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //판매 UI 닫기
            image.gameObject.SetActive(false);
        }
    }
}
