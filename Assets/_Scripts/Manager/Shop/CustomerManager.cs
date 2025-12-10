using System.Collections;
using UnityEngine;
/// <summary>
/// 손님생성해주는 역할
/// /// </summary>

public class CustomerManager : MonoBehaviour
{
    [HideInInspector]
    public GameObject customer;     //손님 프리팹
    [HideInInspector]
    public Transform createPos;     //손님 생성 위치
    
    GameObject[] customers;         //손님들

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

    //모든 손님이 나감 or 없음
    public GameObject GetCustomerAllExit()
    {
        if (customers != null)
        {
            return customers[customers.Length -1];
        }
        return null;
    }
}
