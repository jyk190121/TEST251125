using UnityEngine;

public class DungeonPortal : MonoBehaviour
{
    GameObject player;  //플레이어

    void Start()
    {
        player = GetComponent<GameObject>();
    }

    //왼쪽방으로 갈 때
    public void LeftMove()
    {
        player.transform.position = new Vector3(-10, 0, 0);
    }

    //오른쪽방으로 갈 때
    public void RightMove()
    {
        player.transform.position += new Vector3(10, 0, 0);
    }

    //위쪽방으로 갈 때
    public void UpMove()
    {
        player.transform.position += new Vector3(0, 0, 8);
    }

    //아래쪽방으로 갈 때
    public void DownMove()
    {
        player.transform.position += new Vector3(0, 0, -8);
    }
}
