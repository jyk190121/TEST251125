using UnityEngine;

public class RoomDoorController : MonoBehaviour
{
    public GameObject doorUp;
    public GameObject doorDown;
    public GameObject doorLeft;
    public GameObject doorRight;

    public void SetDoorActive(bool up, bool down, bool left, bool right)
    {
        doorUp?.SetActive(up);
        doorDown?.SetActive(down);
        doorLeft?.SetActive(left);
        doorRight?.SetActive(right);
    }
}
