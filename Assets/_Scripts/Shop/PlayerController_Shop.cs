using Unity.Android.Gradle.Manifest;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController_Shop : MonoBehaviour
{
    //float speed = 2f;
    //public bool isSleeping;

    PlayerControll pc;
    float moveSpeed = 0.1f;
    public bool isSleeping;
    Camera mainCamera;
    CharacterController cc;

    float gravity = -9.8f;
    float yVelocity;

    private void Start()
    {
        pc = GetComponent<PlayerControll>();
        mainCamera = Camera.main;
        cc = pc.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        //float ver = Input.GetAxis("Vertical");      //앞뒤
        //float hor = Input.GetAxis("Horizontal");    //좌우
        //Vector3 pos = new Vector3(hor, 0, ver);

        //pos.Normalize();

        //transform.position += pos * speed * Time.deltaTime;

        float moveX = 0f;
        float moveZ = 0f;

        // 방향키 입력은 KeySetting 기반으로 수정
        if (Input.GetKey(KeySetting.keys[KeyInput.UP]))    moveZ  =  moveSpeed;
        if (Input.GetKey(KeySetting.keys[KeyInput.DOWN]))  moveZ  = -moveSpeed;
        if (Input.GetKey(KeySetting.keys[KeyInput.LEFT]))  moveX  = -moveSpeed;
        if (Input.GetKey(KeySetting.keys[KeyInput.RIGHT])) moveX  =  moveSpeed;

        //Vector3 dir = new Vector3(moveX, 0, moveZ);     //방향 설정
        //dir.y = 0f;
        //dir = dir.normalized;

        if (!isSleeping)
        {
            //캐릭터 뜨는거 보정
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.5f))
            {
                Vector3 pos = transform.position;
                pos.y = hit.point.y;
                transform.position = pos;
            }

            if(cc.isGrounded)
            {
                if (yVelocity < 0) yVelocity = -2f;
            }
            else
            {
                yVelocity += gravity * Time.deltaTime;
            }

            //이동 시에도 보정값
            Vector3 camForward = mainCamera.transform.forward;
            Vector3 camRight = mainCamera.transform.right;

            camForward.y = 0f;
            camRight.y = 0f;

            camForward.Normalize();
            camRight.Normalize();

            Vector3 move = (camForward * moveZ + camRight * moveX);
            move = move.normalized * moveSpeed;

            Vector3 moveDot = move + Vector3.up * yVelocity;

            cc.Move(moveDot * Time.deltaTime);

            // 애니메이션
            if (move.sqrMagnitude > 0.01f) pc.Move(move * Time.deltaTime);
            else pc.Idle();

            if (Input.GetKeyDown(KeySetting.keys[KeyInput.ROLL]))
            {
                pc.Roll(transform.forward);
            }
        }

    }

}
