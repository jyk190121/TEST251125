using UnityEngine;
using UnityEngine.UI;
using static DayManager;
/// <summary>
/// 1.플레이어와 상호작용 (G키) - Collision 
/// </summary>
public class Bed : MonoBehaviour
{
    float keyDownTime;           // 상호작용을 위한 시간
    float sleepDuration;         // 플레이어 잠자는 시간

    bool playerInBed = false;   // 플레이어가 침대에 닿았는지 여부
    Vector3 inBedPos;           // 플레이어가 자기 전 위치
    bool isSleeping = false;    // 플레이어가 자고있는지 여부
    float keyTimer = 0f;        // 키 입력 시간
    float sleepTimer = 0f;      // 자는 시간
    public Image image;         // 키입력하는 동안 띄울 이미지

    PlayerController player;    // 플레이어 스크립트( 임시 )

    DayManager dayManager;      // 자고 일어나면 시간 가기

    private void Start()
    {
        keyDownTime = 1f;
        sleepDuration = 5f;
        playerInBed = false;
        isSleeping = false;
        keyTimer = 0f;
        sleepTimer = 0f;
        image.gameObject.SetActive(false);
        image.fillAmount = 0f;
        image.color = new Color (0, 150f, 0, 100f);
        //Outline outline = image.GetComponent<Outline>();
        //outline.effectDistance = new Vector2(5f, 5f);

        dayManager = GameObject.Find("TestManager").GetComponent<DayManager>();
    }


    void Update()
    {
        image.gameObject.SetActive(false);

        // 플레이어가 자는 중일 때 시간 체크
        if (isSleeping)
        {
            sleepTimer += Time.deltaTime;

            if (sleepTimer >= sleepDuration)
            {
                dayManager.ChangeTimeOfDay(TimeOfDay.Day);
                WakeUpPlayer();
            }
            return;
        }

        image.fillAmount = keyTimer;

        // 플레이어가 침대 근처에 있으면 상호작용 키 누름 시간 체크
        if (playerInBed)
        {
            if (Input.GetKey(KeyCode.G))
            {
                keyTimer += Time.deltaTime;
                image.gameObject.SetActive(true);

                //print($"KeyTimer : {keyTimer}");
                //print($"fillAmount : {image.fillAmount}");

                //잠들기 전 위치를 받아옴
                inBedPos = player.transform.position;

                if (keyTimer >= keyDownTime)
                {
                    SleepPlayer();
                }
            }
            else
            {
                keyTimer = 0f;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            playerInBed = true;
            player = collision.collider.GetComponent<PlayerController>();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            playerInBed = false;
            keyTimer = 0f;
            player = null;
        }
    }

    void SleepPlayer()
    {
        isSleeping = true;
        sleepTimer = 0f;

        Debug.Log("Zzz");

        // 플레이어 움직임을 비활성화
        if (player != null)
        {
            player.enabled = false;
            player.gameObject.transform.position = (transform.position) + Vector3.up;
        }
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
        }
    }
}
