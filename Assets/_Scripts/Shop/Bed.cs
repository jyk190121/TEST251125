using UnityEngine;
/// <summary>
/// 1.플레이어가 가까이 왔을 때 상호작용 키입력 (Collision) 
///
/// </summary>
public class Bed : MonoBehaviour
{
    float keyDownTime;           // 키 길게 누르는 시간
    float sleepDuration;         // 잠자는 시간

    bool playerInBed = false;   // 플레이어 접촉 여부
    Vector3 inBedPos;           // 플레이어 자기전 위치
    bool isSleeping = false;    // 플레이어 자는지 여부
    float keyTimer = 0f;        // 키 입력 시간
    float sleepTimer = 0f;      // 자고 있는 시간

    PlayerController player;   // 플레이어 스크립트(이름은 원하는 걸로 변경)

    private void Start()
    {
        keyDownTime = 1f;
        sleepDuration = 5f;
        playerInBed = false;
        isSleeping = false;
        keyTimer = 0f;
        sleepTimer = 0f;
    }


    void Update()
    {
        // 플레이어가 자는 중이면 시간 체크
        if (isSleeping)
        {
            sleepTimer += Time.deltaTime;
            if (sleepTimer >= sleepDuration)
            {
                WakeUpPlayer();
            }
            return;
        }

        // 플레이어가 침대 근처에 있을 때 상호작용 키 길게 누르기 체크
        if (playerInBed)
        {
            if (Input.GetKey(KeyCode.G))
            {
                keyTimer += Time.deltaTime;

                //자기전 위치값 받아오기
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

        // 플레이어 움직임 비활성화
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

        // 플레이어 움직임 다시 활성화
        if (player != null)
        {
            player.enabled = true;
            player.gameObject.transform.position = inBedPos;
        }
    }
}
