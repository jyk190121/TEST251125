using UnityEngine;

public class PlayerController_Shop : MonoBehaviour
{
    //float speed = 2f;
    //public bool isSleeping;

    PlayerControll PC;
    float moveSpeed = 1f;
    public bool isSleeping;


    private void Start()
    {
        PC = GetComponent<PlayerControll>();
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
        if (Input.GetKey(KeySetting.keys[KeyInput.UP])) moveZ = 1;
        if (Input.GetKey(KeySetting.keys[KeyInput.DOWN])) moveZ = -1;
        if (Input.GetKey(KeySetting.keys[KeyInput.LEFT])) moveX = -1;
        if (Input.GetKey(KeySetting.keys[KeyInput.RIGHT])) moveX = 1;

        Vector3 dir = new Vector3(moveX, 0, moveZ);     //방향 설정
        dir.y = 0f;
        dir = dir.normalized;

        if (!isSleeping)
        {
            if (dir != Vector3.zero)
            {
                PC.Move(dir * moveSpeed * Time.deltaTime);
            }

            if (dir == new Vector3(0, 0, 0))
            {
                PC.Idle();
            }

            if (Input.GetKeyDown(KeySetting.keys[KeyInput.MAINATTACK]))
            {
                PC.Attack();
            }

            if (Input.GetKeyDown(KeySetting.keys[KeyInput.SUBATTACK]))
            {
                PC.SubCharge();
            }
            else if (Input.GetKeyUp(KeySetting.keys[KeyInput.SUBATTACK]))
            {
                PC.SubAttack();
            }

            Vector3 rolldir = transform.forward;

            if (Input.GetKeyDown(KeySetting.keys[KeyInput.ROLL]))
            {
                PC.Roll(rolldir);
            }
        }
      

    }
}
