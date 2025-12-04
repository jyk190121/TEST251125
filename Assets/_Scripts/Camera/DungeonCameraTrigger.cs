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
            DungeonPortal player = GameObject.FindFirstObjectByType <DungeonPortal>();

            switch (moveDirection)
            {
                case MoveDir.Left:
                    cam.LeftMove();
                    player.LeftMove();
                    break;
                case MoveDir.Right:
                    cam.RightMove();
                    player.RightMove(); 
                    break;
                case MoveDir.Up:
                    cam.UpMove();
                    player.UpMove();
                    break;
                case MoveDir.Down:
                    cam.DownMove();
                    player.DownMove();
                    break;
            }
        }
    }
}
