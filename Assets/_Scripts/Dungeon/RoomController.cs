using UnityEngine;

public class RoomController : MonoBehaviour
{
    public GameObject doorUp;
    public GameObject doorDown;
    public GameObject doorLeft;
    public GameObject doorRight;

    public static bool isCleared = false;

    public void SetDoorActive(bool up, bool down, bool left, bool right)
    {
        doorUp?.SetActive(up);
        doorDown?.SetActive(down);
        doorLeft?.SetActive(left);
        doorRight?.SetActive(right);
    }

    public static void ClearDungeon()
    {
        isCleared = true;
        Debug.Log("던전 클리어! 문이 열렸습니다.");
    }
}
