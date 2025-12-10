using System.Collections;
using UnityEngine;
/// <summary>
/// 손님생성해주는 역할
/// - 랜덤 명
/// - 성향별 손님 등장
/// /// </summary>

public class CustomerManager : MonoBehaviour
{
    [HideInInspector]
    public GameObject customer;     //손님 프리팹
    [HideInInspector]
    public Transform createPos;     //손님 생성 위치
    
    GameObject[] customers;  //손님들
    

    //임시
    //void Start()
    //{
    //    StartCoroutine(CreateCustomer(4));
    //}

    //손님 생성
    public IEnumerator CreateCustomer(int r)
    {
        customers = new GameObject[r];

        for (int i = 0; i < r; i++)
        {
            customers[i] = Instantiate(customer, createPos.position , Quaternion.identity);
            customers[i].name = $"손님 {i+1}";

            yield return new WaitForSeconds(5f);
        }
    }
    
    //public int GetExistCustomer()
    //{
    //    foreach (GameObject customer in customers)
    //    {
    //        if (customer.layer != LayerMask.NameToLayer("Customer")) return 0;
    //    }
    //    return 1;
    //}
}
