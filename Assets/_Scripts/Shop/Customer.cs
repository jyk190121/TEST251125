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

    bool itemCheck;
    bool itemBuyCheck;
    public bool itemPayCheck;   //돈 지불여부

    RegisteredItem items;

    NavMeshAgent agent;
    CustomerState state;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.avoidancePriority = Random.Range(0, 100);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        items = GameObject.Find("DisplayStand").GetComponent<RegisteredItem>();
        itemCheck = false;
        itemBuyCheck = false;
        itemPayCheck = false;
        entered = GameObject.Find("EnterPos").GetComponent<Transform>();
        exited = GameObject.Find("ExitPos").GetComponent<Transform>();
        itemPos = GameObject.Find("ItemPos").GetComponent<Transform>();
        salesPos = GameObject.Find("SalesPos").GetComponent<Transform>();
        EnterShop();
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameObject) return;

        switch (state)
        {
            case CustomerState.EnteringShop:
                //입구로 이동
                if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    //agent.Warp(transform.position += new Vector3(0, 0, 2));
                    agent.Warp(new Vector3(-0.9f, 0.98f, -10.48f));
                    state = CustomerState.SelectItem;
                }
                break;

            case CustomerState.SelectItem:
                StartCoroutine(SelectItem());
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
        if (itemCheck) yield break;

        itemCheck = true;

        //등록된 아이템 리스트 확인 (이동x)
        print("아이템 확인");
        agent.SetDestination(itemPos.position);
        //아이템 확인 5초대기
        yield return new WaitForSeconds(5f);
        int r = Random.Range(1, 31);

        //마음에 안드는 경우 바로 나가자
        if (r > 20)
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
        if (itemBuyCheck) yield break;

        itemBuyCheck = true;

        //print("물건을 사자");
        agent.SetDestination(salesPos.position);

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            //플레이어가 POS기 앞에 서서 해당아이템 판매 확인 (계산 중) 후 이동
            agent.enabled = false;
            
            while(true)
            {
                print($"{gameObject.name} 돈 지불 대기");
                if(itemPayCheck)
                {
                    print($"{gameObject.name} 돈 지불 완료");
                    break;
                }
            }

            yield return new WaitForSeconds(3f);
            state = CustomerState.LeavingShop;
          
        }
        else
        {
            //print("다시 판매대로");
            itemBuyCheck = false;
            agent.SetDestination(salesPos.position);
        }
    }

    void LeaveShop()
    {
        agent.enabled = true;
        agent.SetDestination(exited.position);
    }


    //플레이어한테 부딫힐 때 충돌 미적용 (임시)
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), collision.collider);
        }
    }
}


