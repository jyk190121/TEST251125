using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 등록하기 버튼 UI 띄우고 지우기
/// </summary>
public class DisplayStand : MonoBehaviour
{
    public Image image;
    public Image regiItemUI;
    public Image nightImage;
    public TextMeshProUGUI key;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            image.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            GameObject inventory = FindAnyObjectByType<ShopManager>().inventoeyPanel;

            image.gameObject.SetActive(false);
            regiItemUI.gameObject.SetActive(false);
            inventory.SetActive(false);
        }
    }
}
