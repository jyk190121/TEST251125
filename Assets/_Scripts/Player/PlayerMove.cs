using UnityEditor.Networking.PlayerConnection;
using UnityEngine;

//플레이어의 키입력을 받고 함수를 실행 시키는 스크립트
public class PlayerMove : MonoBehaviour
{
    PlayerControll PC;

    private void Start()
    {
        PC = GetComponent<PlayerControll>();
    }
    void Update()
    {
        float moveX = 0f;
        float moveZ = 0f;


        // 방향키 입력은 KeySetting 기반으로 수정
        if (Input.GetKey(KeySetting.keys[KeyInput.UP])) moveZ = 1;
        if (Input.GetKey(KeySetting.keys[KeyInput.DOWN])) moveZ = -1;
        if (Input.GetKey(KeySetting.keys[KeyInput.LEFT])) moveX = -1;
        if (Input.GetKey(KeySetting.keys[KeyInput.RIGHT])) moveX = 1;

        Vector3 dir = new Vector3(moveX, 0, moveZ);     //방향 설정


        if (dir != new Vector3(0, 0, 0))    //new Vector3(0,0,0);
        {
            PC.Move(dir);     // 이동
        }

        if (Input.GetKeyDown(KeySetting.keys[KeyInput.MAINATTACK]))
        {
            PC.Attack();
        }

        if (Input.GetKeyDown(KeySetting.keys[KeyInput.SUBATTACK]))
        {
            PC.SpecialAttack();
        }

        if (Input.GetKeyDown(KeySetting.keys[KeyInput.ROLL]))
        {
            PC.Roll();
        }

        //던전 안이면
        if (Input.GetKeyDown(KeySetting.keys[KeyInput.PENDANT]))
        {

        }
        if (Input.GetKeyDown(KeySetting.keys[KeyInput.SWITCHWEAPON]))
        {

        }
    }
}
