using UnityEngine;

public class RoomEnterTrigger : MonoBehaviour
{
    private RoomController room;
    private TimeLineCamera cam;

    void Awake()
    {
        room = GetComponentInParent<RoomController>();
        cam = FindAnyObjectByType<TimeLineCamera>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (room != null && !room.isStartRoom && !room.isRestRoom)
            {
                if (!room.isSpawned)
                {
                    room.SpawnMonstersOnce();
                    if (room.IsBoss)
                    {
                        StartCoroutine(cam.CameraChange());
                        room.IsBoss = false;
                    }
                    Debug.Log("RoomEnterTrigger 감지: 몬스터 스폰 및 문 잠금 완료.");
                }
            }
        }
    }
}
