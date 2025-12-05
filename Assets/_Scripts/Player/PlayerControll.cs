using UnityEngine;
using UnityEngine.Animations;

//PlayerMove에서 입력받은 값에 따라 실행되는 함수를 정리한 스크립트
public class PlayerControll : MonoBehaviour
{
    CharacterController CC;
    PlayerModel model;
    PlayerAnimController PAC;


    bool isMove = false;

    //구르기
    //1. 구르기 거리
    //2. 구르기 했나?
    float rollDistance = 2f;
    bool isRolling = false;
    float rollTimer = 1.5f;

    void Start()
    {
        PAC = GetComponentInChildren<PlayerAnimController>();
        CC = GetComponent<CharacterController>();   
        model = _MasterManager.Instance.DataManager.GetStat();
        Debug.Log("모델" + model);
    }

    public void Update()
    {
        if (isRolling)
        {
            // 1. 구르기 타이머 감소
            rollTimer -= Time.deltaTime;

            // 2. 구르기 이동 처리 (CharacterController 사용)
            // rolldir은 Roll() 호출 시 transform.forward로 설정되었으므로, 현재 방향으로 이동합니다.
            // Roll 시에는 model.moveSpeed보다 빠른 Roll 전용 속도를 사용합니다.
            float rollSpeed = 5f; // 구르기 속도 (임의의 값, 필요에 따라 조정)
            Vector3 rollMovement = transform.forward * rollSpeed;
            CC.Move(rollMovement * Time.deltaTime);

            // 3. 구르기 종료 처리
            if (rollTimer <= 0f)
            {
                rollTimer = 1.5f; // 타이머 초기화 (쿨다운 또는 다음 구르기 대기 시간으로 사용할 경우)
                isRolling = false;
                gameObject.layer = 7;
                // 구르기 애니메이션이 끝나면 Idle 상태로 돌아가도록 애니메이션 컨트롤러에 알려줄 수 있습니다.
                // PAC.EndRollingAnim(); 같은 함수를 호출할 수 있습니다.
                Debug.Log("구르기 끝");
            }
        }
    }


    //장비 장착시 호출! player의 스탯값 변경!
    public void RefreshStat()
    {
        model = _MasterManager.Instance.DataManager.GetStat();
    }

    public void Idle()
    {
        if (!isMove) return;
        isMove = false;
        PAC.HandleMovementAnim(isMove);
        Debug.Log("정지");
    }
    public void Move(Vector3 dir)
    {
        if (isRolling) return;

        isMove = true;
        PAC.HandleMovementAnim(isMove);
        // 수평 이동
        Vector3 move = dir.normalized * model.moveSpeed;
        CC.Move(move * Time.deltaTime);

        // 회전
        if (dir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                Time.deltaTime * 10f
            );
        }
  
    }

    public void Attack()
    {

    }

    public void SpecialAttack()
    {

    }

    public void Roll(Vector3 rolldir)
    {
        if (isRolling) return;
        PAC.HandleRollingAnim();
        isRolling = true;
        gameObject.layer = 31;

        Debug.Log("구르기 시작");
    }

    public void Die()
    {

    }
}
