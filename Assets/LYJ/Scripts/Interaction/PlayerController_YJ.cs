using UnityEngine;

/// <summary>
/// 플레이어 이동 컨트롤
/// WASD로 마을 내 이동
/// </summary>
public class PlayerController_YJ : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;

    private CharacterController characterController;
    private Animator animator;
    private Vector3 moveDirection = Vector3.zero;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (characterController == null)
            Debug.LogWarning("[PlayerController] CharacterController가 없습니다");
        if (animator == null)
            Debug.LogWarning("[PlayerController] Animator가 없습니다");
    }

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        // 입력 받기
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // 이동 방향 계산 (카메라 기준)
        Vector3 inputDirection = new Vector3(horizontal, 0, vertical).normalized;

        if (inputDirection.magnitude > 0.1f)
        {
            // 이동 방향으로 회전
            Quaternion targetRotation = Quaternion.LookRotation(inputDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            // 이동 속도 적용
            moveDirection = inputDirection * moveSpeed;
        }
        else
        {
            moveDirection = Vector3.zero;
        }

        // 중력 적용
        moveDirection.y -= 9.81f * Time.deltaTime;

        // 이동 실행
        if (characterController != null)
        {
            characterController.Move(moveDirection * Time.deltaTime);
        }

        // 애니메이터 업데이트
        if (animator != null)
        {
            bool isMoving = inputDirection.magnitude > 0.1f;
            animator.SetBool("IsMoving", isMoving);
        }

        Debug.Log($"[PlayerController] 위치: {transform.position}, 입력: ({horizontal}, {vertical})");
    }
}
