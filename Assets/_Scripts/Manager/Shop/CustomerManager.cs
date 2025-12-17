using System.Collections;
using System.Linq;
using UnityEngine;
/// <summary>
/// 손님생성해주는 역할
/// /// </summary>

[RequireComponent(typeof(CustomerManager))]
public class CustomerManager : MonoBehaviour
{
    public GameObject customer;     //손님 프리팹
    [HideInInspector]
    public Transform createPos;     //손님 생성 위치

    bool createCheck;               //손님이 생성된 적이 있는가

    GameObject[] customers;         //손님들
    SalesCustomer salesCustomer;    //계산여부


    //임시
    //void Start()
    //{
    //    StartCoroutine(CreateCustomer(4));
    //}

    private void Start()
    {
        createCheck = false;
        salesCustomer = FindAnyObjectByType<SalesCustomer>();
    }

    //손님 생성
    public IEnumerator CreateCustomer(int r)
    {
        customers = new GameObject[r];
        createCheck = true;
        //customerExit = new bool[r];

        //손님 등장 주기 랜덤설정
        float random = Random.Range(3, 6);

        for (int i = 0; i < r; i++)
        {
            customers[i] = Instantiate(customer, createPos.position, Quaternion.identity);
            customers[i].name = $"손님 {i + 1}";

            yield return new WaitForSeconds(random);
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

        // 모든 손님이 나갔는지 확인
        return customers.All(c => c == null);
    }



    //손님 계산완료처리
    public void CustomerBuyItem()
    {
        Customer buyCustomer = salesCustomer.GetCurrentCustomer();

        if (buyCustomer == null) return;
        //이미 계산한 손님인지 체크 필요
        if (buyCustomer.itemPayCheck) return;

        buyCustomer.itemPayCheck = true;
    }
}
