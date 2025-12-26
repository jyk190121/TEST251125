using Unity.VisualScripting;
using UnityEditor.Networking.PlayerConnection;
using UnityEngine;

//플레이어의 키입력을 받고 함수를 실행 시키는 스크립트
public class VillagePlayerMove : MonoBehaviour
{
    VillagePlayerControll PC;


    private void Start()
    {
        PC = GetComponent<VillagePlayerControll>();
    }
    void Update()
    {
        // 1. 아무 입력이 없을 때를 위한 기본값
        Vector3 dir = Vector3.zero;

        // 2. 키 입력 체크 (우선순위: 상 -> 하 -> 좌 -> 우)
        // 키가 눌리는 즉시 dir을 설정하고, 아래 코드는 실행하지 않고 바로 PC.Move로 넘깁니다.

        if (Input.GetKey(KeySetting.keys[KeyInput.UP]))
        {
            RunMove(Vector3.forward);
            return; // 🌟 핵심: 위 키가 눌렸으면 아래 코드는 무시하고 함수 종료!
        }

        if (Input.GetKey(KeySetting.keys[KeyInput.DOWN]))
        {
            RunMove(Vector3.back);
            return;
        }

        if (Input.GetKey(KeySetting.keys[KeyInput.LEFT]))
        {
            RunMove(Vector3.left);
            return;
        }

        if (Input.GetKey(KeySetting.keys[KeyInput.RIGHT]))
        {
            RunMove(Vector3.right);
            return;
        }

    }

    // 중복 코드를 줄이기 위해 만든 함수
    void RunMove(Vector3 dir)
    {
        // 여기서도 혹시 모를 대각선 값을 방지하기 위해 정규화
        PC.Move(dir);
    }
}
