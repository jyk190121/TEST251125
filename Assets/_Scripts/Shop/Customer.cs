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
        BuyingItem,         //아이템 구매여부
        LeavingShop         //가게밖으로 이동
    }

    public Transform entered;   //가게 입구 위치
    public Transform exited;    //가게 출구 위치
    public Transform itemPos;   //아이템 구매 위치
    public Transform salesPos;  //돈 계산 위치

    NavMeshAgent agent;
    CustomerState state;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        EnterShop();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case CustomerState.EnteringShop:
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    StartCoroutine(BuyItem());
                }
                break;

            case CustomerState.BuyingItem:
                // 구매 중에는 이동하지 않음
                break;

            case CustomerState.LeavingShop:
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    Destroy(gameObject); // 가게 밖으로 나가면 삭제
                }
                break;
        }
    }

    void EnterShop()
    {
        print("가게로 가자");
        state = CustomerState.EnteringShop;
        agent.SetDestination(entered.position);
        if (Vector3.Distance(transform.position, entered.position) < 1f)
        {
            transform.position = exited.position;
        }
    }

    IEnumerator BuyItem()
    {
        print("물건을 사자");
        state = CustomerState.BuyingItem;

        // 구매 행동 예시: 2초 기다림
        yield return new WaitForSeconds(2f);

        LeaveShop();
    }

    void LeaveShop()
    {
        print("가게를 나가자");
        state = CustomerState.LeavingShop;
        agent.SetDestination(exited.position);
    }
}


