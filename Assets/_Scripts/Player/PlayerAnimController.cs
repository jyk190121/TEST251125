using UnityEngine;
using UnityEngine.InputSystem.Utilities;


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

    //이동 및 상태
    readonly int hashMove = Animator.StringToHash("isWalk");             //일단 뛰기애니메이션으로 지정
    readonly int hashRolling = Animator.StringToHash("isRoll");          //구르기 지정
    readonly int hashFalling = Animator.StringToHash("isFall");          //낙사

    //공격

    readonly int hashAttack = Animator.StringToHash("Attack");

    readonly int hashComboCount = Animator.StringToHash("ComboCount");

    readonly int hashCharge = Animator.StringToHash("Charge");

    //무기 구분
    readonly int hashWeapon = Animator.StringToHash("WeaponType");

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
    public void HandleFallingAnim(bool isFalling)
    {
        anim.SetBool(hashFalling, isFalling);
    }


    //일반 공격 관련 애니메이션
    public void HandleAttack(int weaponType, int combo)
    {
        anim.SetInteger(hashWeapon, weaponType);
        anim.SetInteger(hashComboCount, combo);
        anim.SetTrigger(hashAttack); 
    }

    public void HandleCharge(bool charge, int weaponType) // 무기 타입 정보를 받습니다.
    {
        // 1. 무기 타입 설정
        anim.SetInteger(hashWeapon, weaponType);

        // 2. 통합된 Charge Bool 설정
        anim.SetBool(hashCharge, charge);
    }
}
