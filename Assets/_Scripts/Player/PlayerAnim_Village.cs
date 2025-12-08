using UnityEngine;

/// <summary>
/// 플레이어의 애니메이션 출력만을 담당
/// 인풋매니저의 키값을 받아와 출력
/// </summary>
public class PlayerAnim_Village : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    Animator anim;
    SpriteRenderer sr;

    //파라미터 캐싱
    readonly int hashMoveUp = Animator.StringToHash("isWalking_Up");
    readonly int hashMoveSide = Animator.StringToHash("isWalking_Side");
    readonly int hashMoveDown = Animator.StringToHash("isWalking_Down");

    private void Start()
    {
        sr = playerPrefab.GetComponent<SpriteRenderer>();
        anim = playerPrefab.GetComponent <Animator>();
    }

    void Update()
    {
        HandleMovementAnim();
    }

    void HandleMovementAnim()
    {
        // 4방향 입력 체크
        bool isWalking_Up =
            Input.GetKey(KeySetting.keys[KeyInput.UP]);
        bool isWalking_Side =
            Input.GetKey(KeySetting.keys[KeyInput.LEFT]) ||
            Input.GetKey(KeySetting.keys[KeyInput.RIGHT]);
        bool isWalking_Down =
            Input.GetKey(KeySetting.keys[KeyInput.DOWN]);

        if (Input.GetKey(KeySetting.keys[KeyInput.LEFT]))
        {
            sr.flipX = true;
        }
        else if (Input.GetKey(KeySetting.keys[KeyInput.UP])||
                 Input.GetKey(KeySetting.keys[KeyInput.RIGHT])||
                 Input.GetKey(KeySetting.keys[KeyInput.DOWN]))
        {
            sr.flipX = false;
        }

        // Bool 파라미터 직접 넘기기
        anim.SetBool(hashMoveUp, isWalking_Up);
        anim.SetBool(hashMoveSide, isWalking_Side);
        anim.SetBool(hashMoveDown, isWalking_Down);

    }
}
