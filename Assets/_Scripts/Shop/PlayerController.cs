using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float speed = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float ver = Input.GetAxis("Vertical");      //앞뒤
        float hor = Input.GetAxis("Horizontal");    //좌우
        Vector3 pos = new Vector3(hor, 0, ver);

        pos.Normalize();

        transform.position += pos * speed * Time.deltaTime;
    }
}
