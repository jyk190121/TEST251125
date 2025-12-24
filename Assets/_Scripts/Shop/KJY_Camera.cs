using UnityEngine;

public class KJY_Camera : MonoBehaviour
{
    public Transform playerPos;
    public Transform shopPos;
    public Transform homePos;

    //bool movingHome = false;
    bool arrived = false;
    float dis = 2f;

    CameraArea currentArea;
    CameraArea prevArea;

    public enum CameraArea
    {
        Home,
        Shop
    }

    public static System.Action<CameraArea> OnCameraArrived;

    private void Start()
    {
        // 시작 Area 판별
        if (Vector3.Distance(playerPos.position, homePos.position) < dis)
            currentArea = CameraArea.Home;
        else
            currentArea = CameraArea.Shop;

        prevArea = currentArea;

        // 즉시 위치 세팅
        Camera.main.transform.position = GetTargetPos(currentArea);

        arrived = true; // 시작 위치는 이미 도착한 상태
    }

    void Update()
    {
        if (Vector3.Distance(playerPos.transform.position, homePos.transform.position) < dis)
        {
            currentArea = CameraArea.Home;
        }
        else if (Vector3.Distance(playerPos.transform.position, shopPos.transform.position) < dis)
        {
            currentArea = CameraArea.Shop;
        }

        //if (movingHome) currentArea = CameraArea.Home;
        //else currentArea = CameraArea.Shop;

        if (prevArea != currentArea)
        {
            prevArea = currentArea;
            arrived = false;
        }

        CameraMove(currentArea);

    }

    void CameraMove(CameraArea target)
    {
        //Vector3 targetPos =
        //   target == CameraArea.Home
        //   ? new Vector3(-2.2f, 8, -3.5f)
        //   : new Vector3(-2.2f, 8, -12.5f);

        //Camera.main.transform.position =
        //    Vector3.Lerp(Camera.main.transform.position, targetPos, Time.deltaTime * 10f);

        //CheckArrived(targetPos, target);

        Vector3 targetPos = GetTargetPos(target);

        Camera.main.transform.position =
            Vector3.Lerp(Camera.main.transform.position, targetPos, Time.deltaTime * 10f);

        CheckArrived(targetPos, target);
    }

    void CheckArrived(Vector3 targetPos, CameraArea area)
    {
        if (arrived) return;

        if (Vector3.Distance(Camera.main.transform.position, targetPos) < dis)
        {
            arrived = true;
            OnCameraArrived?.Invoke(area);
        }
    }
    Vector3 GetTargetPos(CameraArea target)
    {
        return target == CameraArea.Home
            ? new Vector3(-2.2f, 8, -3.5f)
            : new Vector3(-2.2f, 8, -12.5f);
    }
}
