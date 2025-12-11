using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //float speed = 2f;

    // Update is called once per frame
    void Update()
    {
        //float ver = Input.GetAxis("Vertical");      //앞뒤
        //float hor = Input.GetAxis("Horizontal");    //좌우
        //Vector3 pos = new Vector3(hor, 0, ver);

        //pos.Normalize();

        //transform.position += pos * speed * Time.deltaTime;
        Vector3 pos = transform.position;

        if (pos.y > 0)
        {
            pos.y = 0;
            transform.position = pos;
        }
    }
}
