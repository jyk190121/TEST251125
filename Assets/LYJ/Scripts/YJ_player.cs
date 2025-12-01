using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    void Update()
    {
        // 플레이어 이동
        Move1();
    }

        void Move1()
    {
        // 1. 입력 받기 (키보드, 마우스 등 입력은 Input매니저가 담당한다)
        // Input.GetAxis("Horizontal") -> -1 ~ 1        (실수)
        // Input.GetAxisRaw("Horizontal") -> -1 ~ 1     (정수)

        float moveX = Input.GetAxis("Horizontal"); //좌우 입력
        float moveZ = Input.GetAxis("Vertical");   //상하 입력

        // 2. 이동 벡터 만들기
        Vector3 dir = new Vector3(moveX, 0f, moveZ); //z축은 0으로 고정

        // 3. 방향 벡터 정규화 (대각선 이동시 속도 보정)
        //dir = dir.normalized;
        dir.Normalize();

        // 4. 이동해라
        //Vector3 pos = transform.position + dir * moveSpeed * Time.deltaTime;
        //transform.position = pos;
        transform.position += dir * moveSpeed * Time.deltaTime;
    }
}


