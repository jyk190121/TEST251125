using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static DayManager;
/// <summary>
/// 1.플레이어와 상호작용 - Collision 
/// </summary>
public class Bed : MonoBehaviour
{
    float keyDownTime;                  // 상호작용을 위한 시간
    float sleepDuration;                // 플레이어 잠자는 시간

    bool playerIn;                      // 플레이어가 침대에 닿았는지 여부
    Vector3 inBedPos;                   // 플레이어가 자기 전 위치
    bool isSleeping;                    // 플레이어가 자고있는지 여부
    float keyTimer;                     // 키 입력 시간
    float sleepTimer;                   // 자는 시간
    public Image image;                 // 키입력하는 동안 띄울 이미지
    public Image key;                   // 상호작용 키 알려줄 이미지

    CharacterController player;         // 플레이어 스크립트( 임시 )

    DayManager dayManager;              // 자고 일어나면 시간 가기

    float detectRadius;                 // 침대 주변 감지 범위


    private void Start()
    {
        keyDownTime = 1f;
        sleepDuration = 2f;
        playerIn = false;
        isSleeping = false;
        keyTimer = 0f;
        sleepTimer = 0f;
        image.gameObject.SetActive(false);
        key.gameObject.SetActive(false);
        image.fillAmount = 0f;
        image.color = new Color(0, 150f, 0, 100f);
        //Outline outline = image.GetComponent<Outline>();
        //outline.effectDistance = new Vector2(5f, 5f);
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();

        dayManager = GameObject.Find("TestManager").GetComponent<DayManager>();
        detectRadius = 2f;
    }
    void Update()
    {
        //image.gameObject.SetActive(false);

        // 플레이어가 자는 중일 때 시간 체크
        if (isSleeping)
        {
            sleepTimer += Time.deltaTime;
            image.gameObject.SetActive(false);

            if (sleepTimer >= sleepDuration)
            {
                if(dayManager.IsDay)
                {
                    dayManager.ChangeTimeOfDay(TimeOfDay.Night);
                }
                else if(dayManager.IsNight)
                {
                    dayManager.ChangeTimeOfDay(TimeOfDay.Day);
                }
                WakeUpPlayer();
            }
            return;
        }

        // ★ 플레이어가 침대 주변 detectRadius 반경 안에 있는지 검사
        playerIn = Vector3.Distance(player.transform.position, transform.position) < detectRadius;

        // 주변에 없으면 UI 초기화
        if (!playerIn)
        {
            keyTimer = 0f;
            image.gameObject.SetActive(false);
            key.gameObject.SetActive(false);
            return;
        }

        // 주변에 있으면 상호작용 키 노출
        key.gameObject.SetActive(true);

        // 상호작용 키 입력 확인
        if (Input.GetKey(KeySetting.keys[KeyInput.INTERACTIVE]))
        {
            keyTimer += Time.deltaTime;

            key.gameObject.SetActive(false);
            image.gameObject.SetActive(true);
            image.fillAmount = keyTimer;

            inBedPos = player.transform.position;

            if (keyTimer >= keyDownTime)
            {
                SleepPlayer();
            }
        }
        else
        {
            keyTimer = 0f;
            image.gameObject.SetActive(false);
        }

        //// --------------------------------------------------------------------
        //if (Vector3.Distance(player.transform.position, transform.position) < 5f)
        //{
        //    // 플레이어가 침대 근처에 있으면 상호작용 키 누름 시간 체크
        //    if (playerIn)
        //    {
        //        if (Input.GetKey(KeySetting.keys[KeyInput.INTERACTIVE]))
        //        {
        //            keyTimer += Time.deltaTime;
        //            key.gameObject.SetActive(false);
        //            image.gameObject.SetActive(true);
        //            image.fillAmount = keyTimer;

        //            //print($"KeyTimer : {keyTimer}");
        //            //print($"fillAmount : {image.fillAmount}");

        //            //잠들기 전 위치를 받아옴
        //            inBedPos = player.transform.position;

        //            if (keyTimer >= keyDownTime)
        //            {
        //                image.gameObject.SetActive(false);
        //                SleepPlayer();
        //            }
        //        }
        //        else
        //        {
        //            keyTimer = 0f;
        //            //image.fillAmount = 0f;
        //            image.gameObject.SetActive(false);
        //        }
        //    }
        //}
        //else
        //{
        //    playerIn = false;
        //    keyTimer = 0f;
        //    image.gameObject.SetActive(false);
        //    key.gameObject.SetActive(false);
        //}

    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.collider.CompareTag("Player"))
    //    {
    //        rb.isKinematic = false;
    //        playerIn = true;
    //        //player = collision.collider.GetComponent<PlayerController>();
    //        key.gameObject.SetActive(true);
    //    }
    //}

    //private void OnCollisionExit(Collision collision)
    //{
    //    if (collision.collider.CompareTag("Player"))
    //    {
    //        rb.isKinematic = true;
    //        playerIn = false;
    //        keyTimer = 0f;
    //        image.gameObject.SetActive(false);
    //        key.gameObject.SetActive(false);
    //    }
    //}



    void SleepPlayer()
    {
        isSleeping = true;
        sleepTimer = 0f;

        Debug.Log("Zzz");

        // 플레이어 움직임을 비활성화
        if (player != null)
        {
            player.enabled = false;

            player.gameObject.transform.position = (transform.position) + new Vector3(-1 ,1 ,0);
            player.transform.rotation = Quaternion.Euler(-90f, -90f, 0);
        }

        PlayerController_Shop pc = player.gameObject.GetComponent<PlayerController_Shop>();
        pc.isSleeping = isSleeping;
        //pc.enabled = true;
    }

    void WakeUpPlayer()
    {
        isSleeping = false;
        Debug.Log("플레이어가 일어났습니다.");

        // 플레이어 움직임을 다시 활성화
        if (player != null)
        {
            player.enabled = true;

            player.gameObject.transform.position = inBedPos;
            player.transform.rotation = Quaternion.identity;
        }

        PlayerController_Shop pc = player.gameObject.GetComponent<PlayerController_Shop>();
        pc.isSleeping = isSleeping;
        //pc.enabled = false;
    }

}
