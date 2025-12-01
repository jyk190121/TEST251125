using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        MoveWithGetAxis();
    }

    void MoveWithGetAxis()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(moveX, 0f, moveZ);

        dir.Normalize();

        transform.Translate(dir * speed * Time.deltaTime);
    }
}
