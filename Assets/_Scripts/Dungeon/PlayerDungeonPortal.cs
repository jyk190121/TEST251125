using UnityEngine;

public class PlayerDungeonPortal : MonoBehaviour
{ 
    GameObject player;
    CharacterController CC;

    private void Start()
    {
        CC = GetComponent<CharacterController>();
        player = this.gameObject;
        player.transform.position = Vector3.zero;
        player.transform.rotation = Quaternion.identity;
    }

    //왼쪽방으로 갈 때
    public void LeftMove(Collider player)
    {
        CC.enabled = false;
        player.transform.position += new Vector3(-20, 0, 0);
        CC.enabled = true;
    }

    //오른쪽방으로 갈 때
    public void RightMove(Collider player)
    {
        CC.enabled = false;
        player.transform.position += new Vector3(20, 0, 0);
        CC.enabled = true;
    }

    //위쪽방으로 갈 때
    public void UpMove(Collider player)
    {
        CC.enabled = false;
        player.transform.position += new Vector3(0, 0, 18);
        CC.enabled = true;
    }

    //아래쪽방으로 갈 때
    public void DownMove(Collider player)
    {
        CC.enabled = false;
        player.transform.position += new Vector3(0, 0, -18);
        CC.enabled = true;
    }
}
