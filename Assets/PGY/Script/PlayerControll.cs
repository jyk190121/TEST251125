using UnityEngine;

//PlayerMove에서 입력받은 값에 따라 실행되는 함수를 정리한 스크립트
public class PlayerControll : MonoBehaviour
{
    Rigidbody rb;
    PlayerModel model;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
    }

    // Update is called once per frame
    void Update()
    {
        model = DataManager.Instance.GetPlayer();
    }

    public void Move(Vector3 dir)
    {
        rb.linearVelocity = dir * model.moveSpeed;
    }

    public void Attack()
    {

    }

    public void SpecialAttack()
    {

    }

    public void Roll()
    {

    }

    public void Die()
    {

    }
}
