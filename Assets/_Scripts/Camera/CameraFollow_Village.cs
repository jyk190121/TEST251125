using UnityEngine;

public class CameraFollow_Village : MonoBehaviour
{
    [SerializeField] Transform target;

    [Header("카메라 범위 제한")]
    [SerializeField] private float minX = -7.4f;
    [SerializeField] private float maxX = 7.4f;
    [SerializeField] private float minZ = -12.3f;
    [SerializeField] private float maxZ = 12.3f;
    [SerializeField] private float minY = -1f;
    [SerializeField] private float maxY = 10f;

    void Update()
    {
        FollowTarget();
    }

    void FollowTarget()
    {
        if (target == null) return;

        transform.position = new Vector3(
            target.position.x,
            transform.position.y,
            target.position.z);
    }

    private void LateUpdate()
    {
        ClampPosition();
    }

    /// <summary>
    /// 카메라 위치를 범위 안에 제한
    /// </summary>
    private void ClampPosition()
    {
        Vector3 currentPos = transform.position;

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
            transform.position = newPos;

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

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(center, size);
    }
}