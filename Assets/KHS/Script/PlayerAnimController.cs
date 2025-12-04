using UnityEngine;


/// <summary>
/// 플레이어의 애니메이션 출력만을 담당
/// 인풋매니저의 키값을 받아와 출력(나중엔 속도 등으로 걷기 뛰기 등 나눌수도)
/// </summary>
public class PlayerAnimController : MonoBehaviour
{
    public Animator anim;

    //파라미터 캐싱(아마 해당 파라미터를 미리 기억해두는 느낌인듯)
    readonly int hashMove = Animator.StringToHash("isRun");             //일단 뛰기애니메이션으로 지정
    readonly int hashAttack = Animator.StringToHash("Sword");           //일단 검 공격 애니메이션으로 지정
    readonly int hashRolling = Animator.StringToHash("Rolling");        //구르기 지정


    void Update()
    {
        HandleMovementAnim();
        HandleRollingAnim();
    }

    void HandleMovementAnim()
    {
        // 4방향 입력 체크
        bool isRun =
            Input.GetKey(KeySetting.keys[KeyInput.UP]) ||
            Input.GetKey(KeySetting.keys[KeyInput.DOWN]) ||
            Input.GetKey(KeySetting.keys[KeyInput.LEFT]) ||
            Input.GetKey(KeySetting.keys[KeyInput.RIGHT]);

        // Bool 파라미터 직접 넘기기
        anim.SetBool(hashMove, isRun);

        // isRun == true → Run 출력
        // isRun == false → Idle 출력
    }

    void HandleRollingAnim()
    {
        if (Input.GetKeyDown(KeySetting.keys[KeyInput.ROLL]))
        {
            anim.SetTrigger(hashRolling);
        }
    }
}
