using UnityEngine;

public class CameraFollow_Village : MonoBehaviour
{
    [SerializeField] Transform target;

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
}
