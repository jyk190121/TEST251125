using UnityEngine;


/// <summary>
/// 플레이어의 애니메이션 출력만을 담당
/// 인풋매니저의 키값을 받아와 출력(나중엔 속도 등으로 걷기 뛰기 등 나눌수도)
/// </summary>
public class PlayerAnimController : MonoBehaviour
{
    Animator anim;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    //파라미터 캐싱(아마 해당 파라미터를 미리 기억해두는 느낌인듯) -> 최적화
    readonly int hashMove = Animator.StringToHash("isRun");             //일단 뛰기애니메이션으로 지정
    readonly int hashRolling = Animator.StringToHash("Rolling");        //구르기 지정
    readonly int hashSpear = Animator.StringToHash("Spear");            //창
    readonly int hashSword = Animator.StringToHash("Sword");            //일단 검 공격 애니메이션으로 지정
    readonly int hasShield = Animator.StringToHash("Shield");           //방패
    readonly int hasArrowShoot = Animator.StringToHash("ArrowShoot");   //활

    public void HandleMovementAnim(bool isRun)
    {

        // Bool 파라미터 직접 넘기기
        anim.SetBool(hashMove, isRun);

        // isRun == true → Run 출력
        // isRun == false → Idle 출력
    }

    public void HandleRollingAnim()
    {
        anim.SetTrigger(hashRolling);
    }


    //일반 공격 관련 애니메이션
    public void HandleAttackSpear()
    {
        anim.SetTrigger(hashSpear);
    }
    public void HandleAttackSword()
    {
        anim.SetTrigger(hashSword);
    }
    public void HandleAttackBow()
    {
        anim.SetTrigger(hasArrowShoot);
    }

    //보조 공격
    public void HandleShieldAnim(bool onoff)
    {
        anim.SetBool(hasShield, onoff);
    }





}
