using UnityEngine;

public class KJY_Camera : MonoBehaviour
{
    public Transform playerPos;
    public Transform shopPos;
    public Transform homePos;

    bool movingHome = false;
    float dis = 2f;

    CameraArea currentArea;

    public enum CameraArea
    {
        Home,
        Shop
    }

    public static System.Action<CameraArea> OnCameraArrived;

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(playerPos.transform.position, homePos.transform.position) < dis)
        {
            movingHome = true;
        }
        else if (Vector3.Distance(playerPos.transform.position, shopPos.transform.position) < dis)
        {
            movingHome = false;
        }

        if (movingHome) currentArea = CameraArea.Home;
        else currentArea = CameraArea.Shop;

        CameraMove(currentArea);

    }

    void CameraMove(CameraArea target)
    {
        Vector3 targetPos =
           target == CameraArea.Home
           ? new Vector3(-2.2f, 8, -3.5f)
           : new Vector3(-2.2f, 8, -12.5f);

        Camera.main.transform.position =
            Vector3.Lerp(Camera.main.transform.position, targetPos, Time.deltaTime * 10f);

        OnCameraArrived?.Invoke(target);
    }
}
