using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class TimeLineCamera : MonoBehaviour
{
    public CinemachineCamera[] cameras;
    //public GameObject BossRoomPrfeb;        //보스방 프리팹

    int activePriority = 1;                //활성화된 카메라 우선순위 값
    int inactivePriority = 0;

    private void Awake()
    {
        //cameras[1] = BossRoomPrfeb.GetComponentInChildren<CinemachineCamera>();
    }

    public IEnumerator CameraChange()
    {
        print("이거 타는지");
        if (cameras[1] == null) yield return null;


        cameras[0].Priority = inactivePriority;
        cameras[1].Priority = activePriority;

        yield return new WaitForSeconds(3f);

        cameras[0].Priority = activePriority;
        cameras[1].Priority = inactivePriority;

    }
}
