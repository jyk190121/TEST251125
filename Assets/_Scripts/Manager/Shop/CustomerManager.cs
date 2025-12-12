using System.Collections;
using System.Linq;
using UnityEngine;
/// <summary>
/// 손님생성해주는 역할
/// /// </summary>

public class CustomerManager : MonoBehaviour
{
    public GameObject customer;     //손님 프리팹
    [HideInInspector]
    public Transform createPos;     //손님 생성 위치

    bool createCheck;               //손님이 생성된 적이 있는가
    //bool[] customerExit;            //모든 손님이 나갔는가

    GameObject[] customers;         //손님들

    //임시
    //void Start()
    //{
    //    StartCoroutine(CreateCustomer(4));
    //}

    private void Start()
    {
        createCheck = false;
    }

    //손님 생성
    public IEnumerator CreateCustomer(int r)
    {
        customers = new GameObject[r];
        createCheck = true;
        //customerExit = new bool[r];

        for (int i = 0; i < r; i++)
        {
            customers[i] = Instantiate(customer, createPos.position, Quaternion.identity);
            customers[i].name = $"손님 {i + 1}";

            yield return new WaitForSeconds(5f);
        }
        createCheck = false;
    }

    //모든 손님이 나감
    public bool GetCustomerAllExit()
    {
        //if (createCheck)
        //{
        //    //return customers[customers.Length - 1];
        //    return customers.All(c => c == null);

        //}
        //return false;

        // 손님 생성중이면 false
        if (createCheck) return false;

        if (customers == null) return false;

        // 모든 손님이 true(나갔음)인지 확인
        return customers.All(c => c == null);
    }
}
