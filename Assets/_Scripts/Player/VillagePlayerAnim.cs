using UnityEngine;

/// <summary>
/// 플레이어의 애니메이션 출력만을 담당
/// 인풋매니저의 키값을 받아와 출력
/// </summary>
public class VillagePlayerAnim : MonoBehaviour
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
        // 1. 모든 상태를 일단 false로 초기화 (중복 방지 핵심)
        bool isUp = false;
        bool isDown = false;
        bool isSide = false;

        // 2. 이동 스크립트와 똑같은 우선순위로 체크 (상 -> 하 -> 좌 -> 우)
        // 하나가 걸리면 다른 건 쳐다보지 않음 (else if 사용)

        if (Input.GetKey(KeySetting.keys[KeyInput.UP]))
        {
            isUp = true;
            sr.flipX = false; // 위로 갈 땐 뒤집지 않음
        }
        else if (Input.GetKey(KeySetting.keys[KeyInput.DOWN]))
        {
            isDown = true;
            sr.flipX = false; // 아래로 갈 땐 뒤집지 않음
        }
        else if (Input.GetKey(KeySetting.keys[KeyInput.LEFT]))
        {
            isSide = true;
            sr.flipX = true;  // 왼쪽 볼 때만 뒤집음!
        }
        else if (Input.GetKey(KeySetting.keys[KeyInput.RIGHT]))
        {
            isSide = true;
            sr.flipX = false; // 오른쪽 볼 땐 원래대로
        }

        // 3. 결정된 값만 애니메이터에 전달
        // 이제 isUp, isDown, isSide 중 하나만 true이거나, 모두 false(Idle)입니다.
        anim.SetBool(hashMoveUp, isUp);
        anim.SetBool(hashMoveDown, isDown);
        anim.SetBool(hashMoveSide, isSide);
    }
}
