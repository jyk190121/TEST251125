using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 계산대 앞 손님 여부 파악
/// </summary>
public class SalesCustomer : MonoBehaviour
{

    // 계산대에 있는 손님들
    public List<Customer> customers = new List<Customer>();

    private void OnTriggerEnter(Collider other)
    {
        //손님이 왔을 때 확인 여부
        if (other.gameObject.layer == LayerMask.NameToLayer("Customer"))
        {
            Customer customer = other.GetComponent<Customer>();
            if (customer != null && !customers.Contains(customer))
            {
                customers.Add(customer);
            }
        }
        //print($"{customerCheck}");
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Customer"))
        {
            Customer customer = other.GetComponent<Customer>();
            if (customer != null)
            {
                customers.Remove(customer);
            }
        }
        //print($"{customerCheck}");
    }

    // 계산 가능한 손님이 있는지
    public bool HasCustomer()
    {
        return customers.Count > 0;
    }

    // 가장 앞에 있는 손님 (FIFO)
    public Customer GetCurrentCustomer()
    {
        if (customers.Count == 0) return null;
        return customers[0];
    }
}
