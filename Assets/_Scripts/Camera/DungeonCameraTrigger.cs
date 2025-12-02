using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    public enum MoveDir { Left, Right, Up, Down }
    public MoveDir moveDirection;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DungeonCamera cam = Camera.main.GetComponent<DungeonCamera>();

            switch (moveDirection)
            {
                case MoveDir.Left:
                    cam.LeftMove();
                    break;
                case MoveDir.Right:
                    cam.RightMove();
                    break;
                case MoveDir.Up:
                    cam.UpMove();
                    break;
                case MoveDir.Down:
                    cam.DownMove();
                    break;
            }
        }
    }
}
