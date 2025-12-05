using System.Collections;
using UnityEngine;
using UnityEngine.AI;
/// <summary>
/// 손님
/// 1.가게안으로 이동
/// 2.물건구매여부 설정
/// 3.마음에 드는 물건이 있을 경우 들고 판매대로 이동
/// 4.가게밖으로 이동
/// </summary>
public class Customer : MonoBehaviour
{
    public enum CustomerState
    {
        EnteringShop,       //가게안으로 이동
        SelectItem,         //아이템 구매여부
        BuyingItem,         //아이템 구매
        LeavingShop         //가게밖으로 이동
    }

    public Transform entered;   //가게 입구 위치
    public Transform exited;    //가게 출구 위치
    public Transform itemPos;   //아이템 구매 위치
    public Transform salesPos;  //돈 계산 위치

    bool itemChek;
    bool itemBuyCheck;

    RegisteredItem items;

    NavMeshAgent agent;
    CustomerState state;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        items = GameObject.Find("DisplayStand").GetComponent<RegisteredItem>();
        itemChek = false;
        itemBuyCheck = false;
        EnterShop();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case CustomerState.EnteringShop:
                //입구로 이동
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (Vector3.Distance(transform.position, entered.position) < 1f)
                    {
                        //Warp는 순간이동
                        agent.Warp(new Vector3(-0.9f, 0.98f, -10.48f));
                    }
                    StartCoroutine(SelectItem());
                }
                break;

            case CustomerState.SelectItem:
                break;

            case CustomerState.BuyingItem:

                StartCoroutine(BuyItem());
                break;

            case CustomerState.LeavingShop:
                LeaveShop();
                //출구로 이동
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    // 가게 밖으로 나가면 삭제
                    Destroy(gameObject); 
                }
                break;
        }
    }

    void EnterShop()
    {
        print("가게로 가자");
        state = CustomerState.EnteringShop;
        agent.SetDestination(entered.position);
    }

    IEnumerator SelectItem()
    {
        if (itemChek) yield break;

        itemChek = true;

        //등록된 아이템 리스트 확인 (이동x)
        print("아이템 확인");
        agent.SetDestination(itemPos.position);
        //아이템 확인 5초대기
        yield return new WaitForSeconds(5f);
        int r = Random.Range(1, 31);

        //마음에 안드는 경우 바로 나가자
        if(r > 20)
        {
            print("마음에 드는게 없네");
            state = CustomerState.LeavingShop;
        }
        //마음에 드는 경우 판매대로 이동
        else
        {
            print("이 아이템 사야겠다");
            state = CustomerState.BuyingItem;
        }
            
    }

    IEnumerator BuyItem()
    {
        if(itemBuyCheck) yield break;

        itemBuyCheck = true;
        //아이템 가격 지불 (이동x)

        print("물건을 사자");
        agent.SetDestination(salesPos.position);

        //계산 후 밖으로 나감
        yield return new WaitForSeconds(3f);
        agent.enabled = false;
        yield return new WaitForSeconds(3f);
        state = CustomerState.LeavingShop;
    }

    void LeaveShop()
    {
        agent.enabled = true;
        agent.SetDestination(exited.position);
    }
}


