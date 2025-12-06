using System.Collections;
using UnityEngine;
/// <summary>
/// ¼Õ´Ô¸¸µé¾îÁÖ±â
/// ¸î¸í ? ·£´ý?
/// </summary>

public class CustomerManager : MonoBehaviour
{
    public GameObject customer;     //¼Õ´Ô ÇÁ¸®ÆÕ
    GameObject[] customers;         //¼Õ´Ôµé
    public Transform createPos;     //¼Õ´Ôµé »ý¼º À§Ä¡

    //ÀÓ½Ã
    void Start()
    {
        StartCoroutine(CreateCustomer(7));
    }

    //¼Õ´Ô »ý¼º
    public IEnumerator CreateCustomer(int r)
    {
        //¼Õ´ÔÀº ³·¿¡¸¸ µîÀå
        //if(Daily? == ³·)
        customers = new GameObject[r];

        for (int i = 0; i < r; i++)
        {
            customers[i] = Instantiate(customer, createPos.position , Quaternion.identity);

            yield return new WaitForSeconds(5f);
        }
    }
    
}
