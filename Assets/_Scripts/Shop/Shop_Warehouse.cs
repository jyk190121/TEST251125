using UnityEngine;
using UnityEngine.UI;

public class Warehouse : MonoBehaviour
{

    float keyDownTime;          // 상호작용을 위한 시간

    bool playerIn;              // 플레이어가 창고에 닿았는지 여부
    float keyTimer;             // 키 입력 시간
    public Image image;         // 키입력하는 동안 띄울 이미지
    public Image key;           // 상호작용 키 알려줄 이미지

    PlayerController player;    // 플레이어 스크립트( 임시 )

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        keyDownTime = 1.5f;
        keyTimer = 0f;
        image.gameObject.SetActive(false);
        key.gameObject.SetActive(false);
        image.fillAmount = 0f;
        image.color = new Color(0, 0, 150f, 50f);
    }

    // Update is called once per frame
    void Update()
    {
        // 플레이어가 창고 근처에 있으면 상호작용 키 누름 시간 체크
        if (playerIn)
        {
            if (Input.GetKey(KeySetting.keys[KeyInput.INTERACTIVE]))
            {
                keyTimer += Time.deltaTime;
                key.gameObject.SetActive(false);
                image.gameObject.SetActive(true);
                image.fillAmount = keyTimer;

                if (keyTimer >= keyDownTime)
                {
                    image.gameObject.SetActive(false);
                    //창고개방
                    print("창고개방");
                }
            }
            else
            {
                keyTimer = 0f;
                //image.fillAmount = 0f;
                image.gameObject.SetActive(false);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            playerIn = true;
            player = collision.collider.GetComponent<PlayerController>();
            key.gameObject.SetActive(true);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            playerIn = false;
            keyTimer = 0f;
            player = null;
            image.gameObject.SetActive(false);
            key.gameObject.SetActive(false);
        }
    }
}
