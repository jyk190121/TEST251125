using UnityEngine;

public class PlayerDungeonPortal : MonoBehaviour
{ 
    GameObject player;

    private void Start()
    {
        player = this.gameObject;
        player.transform.position = Vector3.zero;
        player.transform.rotation = Quaternion.identity;
    }

    //왼쪽방으로 갈 때
    public void LeftMove(Collider player)
    {
        player.transform.position += new Vector3(-10, 0, 0);
    }

    //오른쪽방으로 갈 때
    public void RightMove(Collider player)
    {
        player.transform.position += new Vector3(10, 0, 0);
    }

    //위쪽방으로 갈 때
    public void UpMove(Collider player)
    {
        player.transform.position += new Vector3(0, 0, 8);
    }

    //아래쪽방으로 갈 때
    public void DownMove(Collider player)
    {
        player.transform.position += new Vector3(0, 0, -8);
    }
}
