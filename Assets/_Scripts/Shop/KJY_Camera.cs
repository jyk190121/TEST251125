using UnityEngine;

public class KJY_Camera : MonoBehaviour
{
    public Transform playerPos;
    public Transform shopPos;
    public Transform homePos;

    bool movingHome;

    float dis;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movingHome = false;
        dis = 0.1f;
    }

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

        if (movingHome) MovingHome();
        else MovingShop();
    }

    void MovingHome()
    {
        Camera.main.transform.position =
            Vector3.Lerp(Camera.main.transform.position,
            new Vector3(-2.2f, 10, 1.5f),
            Time.deltaTime * 10f);
    }

    void MovingShop()
    {
        Camera.main.transform.position =
          Vector3.Lerp(Camera.main.transform.position,
          new Vector3(-2.2f, 10, -7.5f),
          Time.deltaTime * 10f);
    }

}
