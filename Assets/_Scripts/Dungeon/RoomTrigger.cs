using UnityEngine;

public class RoomEnterTrigger : MonoBehaviour
{
    private RoomController room;

    void Start()
    {
        room = GetComponentInParent<RoomController>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            room.SpawnMonstersOnce();
        }
    }
}
