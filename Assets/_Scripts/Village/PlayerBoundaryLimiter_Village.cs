using UnityEngine;

/// <summary>
/// 플레이어를 특정 범위 안에 제한
/// Ground 경계 안에 머무르도록 함
/// </summary>
public class PlayerBoundaryLimiter : MonoBehaviour
{
    [SerializeField] private float minX = -25f;
    [SerializeField] private float maxX = 25f;
    [SerializeField] private float minZ = -23f;
    [SerializeField] private float maxZ = 23f;
    [SerializeField] private float minY = -1f;
    [SerializeField] private float maxY = 10f;

    private Transform playerTransform;

    private void Start()
    {
        playerTransform = transform;
    }

    private void LateUpdate()
    {
        ClampPlayerPosition();
    }

    /// <summary>
    /// 플레이어 위치를 범위 안에 제한
    /// </summary>
    private void ClampPlayerPosition()
    {
        Vector3 currentPos = playerTransform.position;

        // 범위를 벗어났는지 확인
        float clampedX = Mathf.Clamp(currentPos.x, minX, maxX);
        float clampedY = Mathf.Clamp(currentPos.y, minY, maxY);
        float clampedZ = Mathf.Clamp(currentPos.z, minZ, maxZ);

        // 위치 조정 필요?
        if (clampedX != currentPos.x ||
            clampedY != currentPos.y ||
            clampedZ != currentPos.z)
        {
            Vector3 newPos = new Vector3(clampedX, clampedY, clampedZ);

            // 일반 Transform 이동
            playerTransform.position = newPos;


            Debug.Log($"[PlayerBoundaryLimiter] 플레이어 위치 제한: {newPos}");
        }
    }

    /// <summary>
    /// 범위 시각화 (Scene 뷰에서만 표시)
    /// </summary>
    private void OnDrawGizmos()
    {
        // 범위 박스 그리기
        Vector3 center = new Vector3(
            (minX + maxX) / 2,
            (minY + maxY) / 2,
            (minZ + maxZ) / 2
        );

        Vector3 size = new Vector3(
            maxX - minX,
            maxY - minY,
            maxZ - minZ
        );

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(center, size);

        // 모서리 강조
        Gizmos.color = Color.red;
        DrawCorner(new Vector3(minX, minY, minZ));
    }

    private void DrawCorner(Vector3 pos)
    {
        float cornerSize = 2f;
        Gizmos.DrawLine(pos, pos + Vector3.up * cornerSize);
    }
}
