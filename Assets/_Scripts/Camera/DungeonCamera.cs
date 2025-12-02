using UnityEngine;

public class DungeonCamera : MonoBehaviour
{
    //던전 씬의 카메라에 붙여서 던전 이동에 따른 카메라 움직임
    Camera cam;
    [SerializeField] int cameraHeigth = 23;

    private void Start()
    {
        cam = GetComponent<Camera>();
        cam.transform.position = new Vector3(0, cameraHeigth, 0);
        cam.transform.rotation = Quaternion.Euler(90, 0, 0);
    }

    //왼쪽방으로 갈 때
    public void LeftMove()
    {
        cam.transform.position += new Vector3(-50, 0, 0);
    }
    
    //오른쪽방으로 갈 때
    public void RightMove()
    {
        cam.transform.position += new Vector3(50, 0, 0);
    }

    //위 방으로 갈 때
    public void UpMove()
    {
        cam.transform.position += new Vector3(0, 0, 30);
    }

    //아래 방으로 갈 때
    public void DownMove()
    {
        cam.transform.position += new Vector3(0, 0, -30);
    }
}
