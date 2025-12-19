using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
/// <summary>
/// 손님
/// 1.가게안으로 이동
/// 2.물건구매여부 설정
/// 3.마음에 드는 물건이 있을 경우 들고 판매대로 이동
/// 4.가게밖으로 이동
/// </summary>

public enum CustomerType
{
    Normal,      // 일반형
    Generous,    // 후한형
    Picky        // 까다로운형
}

public enum PriceEvaluation
{
    VeryCheap,    // 굉장히 쌈 (현재가 < 기본가 × 0.9)
    Appropriate,  // 적정가 (기본가 × 0.9 ~ 1.1)
    Expensive,    // 비쌈 (기본가 × 1.1 ~ 1.2)
    VeryExpensive // 너무 비쌈 (기본가 × 1.2 초과)
}


public class Customer : MonoBehaviour
{
    public enum CustomerState
    {
        EnteringShop, //가게안으로 이동
        SelectItem, //아이템 구매여부
        BuyingItem, //아이템 구매
        LeavingShop //가게밖으로 이동
    }

    [Header("위치 설정")]
    public Transform entered; //가게 입구 위치
    public Transform exited; //가게 출구 위치
    public Transform itemPos; //아이템 구매 위치
    public Transform salesPos; //돈 계산 위치

    [Header("손님 설정")]
    public CustomerType customerType;

    bool itemCheck;
    bool itemBuyCheck;
    public bool itemPayCheck; //돈 지불여부

    RegisteredItem registeredItems;

    NavMeshAgent agent;
    CustomerState state;

    Animator anim; //손님 애니메이션

    // 선택된 아이템 정보
    Item selectedItem;
    PriceEvaluation priceEvaluation;
    bool willBuy;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent == null)
        {
            Debug.LogError("[Customer] NavMeshAgent 컴포넌트를 찾을 수 없습니다");
            return;
        }

        agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //필수 참조 초기화
        registeredItems = FindAnyObjectByType<RegisteredItem>();
        if (registeredItems == null)
        {
            Debug.LogError("[Customer] RegisteredItem을 찾을 수 없습니다");
            Destroy(gameObject);
            return;
        }

        itemCheck = false;
        itemBuyCheck = false;
        itemPayCheck = false;

        entered = GameObject.Find("EnterPos").GetComponent<Transform>();
        exited = GameObject.Find("ExitPos").GetComponent<Transform>();
        itemPos = GameObject.Find("ItemPos").GetComponent<Transform>();
        salesPos = GameObject.Find("SalesPos").GetComponent<Transform>();

        if (entered == null || exited == null || itemPos == null || salesPos == null)
        {
            Debug.LogError("[Customer] 위치 참조가 부족합니다");
            Destroy(gameObject);
            return;
        }

        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogWarning("[Customer] Animator가 없습니다");
        }

        customerType = GetWeightedRandomCustomerType();
        Debug.Log($"[Customer] {gameObject.name}이 들어왔습니다. 타입: {customerType}");

        EnterShop();
    }

    // Update is called once per frame
    void Update()
    {
        CustomerMove();
    }

    /// <summary>
    /// 손님 타입 랜덤 선택
    /// </summary>
    CustomerType GetWeightedRandomCustomerType()
    {
        float rand = Random.Range(0f, 1f);

        if (rand < 0.1f)        // 0.0 ~ 0.1 = 10%
            return CustomerType.Generous;
        else if (rand < 0.5f)   // 0.1 ~ 0.5 = 40%
            return CustomerType.Normal;
        else                    // 0.5 ~ 1.0 = 50%
            return CustomerType.Picky;
    }


    void CustomerMove()
    {
        if (!gameObject) return;

        if (anim != null)
            anim.SetFloat("Speed", agent.velocity.sqrMagnitude);

        switch (state)
        {
            case CustomerState.EnteringShop:
                HandleEnteringShop();
                break;

            case CustomerState.SelectItem:
                HandleSelectItem();
                break;

            case CustomerState.BuyingItem:
                HandleBuyingItem();
                break;

            case CustomerState.LeavingShop:
                HandleLeavingShop();
                break;
        }
    }

    void HandleEnteringShop()
    {
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.Warp(new Vector3(-0.9f, 0.98f, -10.48f));
            state = CustomerState.SelectItem;
            itemCheck = false;

            Debug.Log($"[Customer] {gameObject.name}이 가게에 들어왔습니다");
        }
    }

    void HandleSelectItem()
    {
        if (!itemCheck)
        {
            StartCoroutine(SelectItem());
        }

        if (Vector3.Distance(transform.position, itemPos.position) < 1f)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    void HandleBuyingItem()
    {
        if (!itemBuyCheck)
        {
            StartCoroutine(BuyItem());
        }
    }

    void HandleLeavingShop()
    {
        LeaveShop();

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            Debug.Log($"[Customer] {gameObject.name}이 가게를 나갔습니다");
            Destroy(gameObject);
        }
    }

    void EnterShop()
    {
        //print("가게로 가자");
        state = CustomerState.EnteringShop;
        agent.SetDestination(entered.position);
    }
    IEnumerator SelectItem()
    {
        itemCheck = true;
        agent.SetDestination(itemPos.position);

        //등록된 아이템 확인 및 이동 5초대기
        print("아이템 확인");

        yield return new WaitForSeconds(5f);

        // 1단계: 가게에 마음에 드는 게 있는가?
        int r = Random.Range(1, 31);
        if (r > 20)
        {
            Debug.Log($"[{customerType}손님] 마음에 드는 게 없네");
            state = CustomerState.LeavingShop;
            yield break;
        }

        // 2단계: 진열된 아이템 중 하나 랜덤 선택
        selectedItem = SelectRandomItem();
        if (selectedItem == null)
        {
            Debug.Log($"[{customerType}손님] 판매 가능한 아이템이 없습니다");
            state = CustomerState.LeavingShop;
            yield break;
        }

        // 3단계: 선택한 아이템의 현재 가격 평가
        priceEvaluation = EvaluatePrice(selectedItem);
        Debug.Log($"[{customerType}손님] {selectedItem.itemName}을(를) 봤습니다. 평가: {priceEvaluation}");

        // 4단계: 손님 타입에 따른 구매 여부 결정
        willBuy = DecideToBuy(priceEvaluation);

        if (willBuy)
        {
            Debug.Log($"[{customerType}손님] 이 아이템 사야겠다!");
            state = CustomerState.BuyingItem;
        }
        else
        {
            Debug.Log($"[{customerType}손님] 이 가격은 좀 비싼데...");
            state = CustomerState.LeavingShop;
        }
    }

    /// <summary>
    /// 진열대에 등록된 아이템 중 랜덤으로 하나 선택
    /// </summary>
    Item SelectRandomItem()
    {
        if (registeredItems == null)
        {
            Debug.LogError("[Customer] registeredItems가 null입니다");
            return null;
        }

        if (registeredItems.itemList == null)
        {
            Debug.LogError("[Customer] itemList가 null입니다");
            return null;
        }

        List<Item> availableItems = new List<Item>();

        // 진열대에 등록된 null이 아닌 아이템만 수집
        foreach (var item in registeredItems.itemList)
        {
            if (item != null)
            {
                availableItems.Add(item);
            }
        }

        if (availableItems.Count == 0)
        {
            Debug.LogWarning("[Customer] 진열대에 아이템이 없습니다");
            return null;
        }

        int randomIndex = Random.Range(0, availableItems.Count);
        Item selected = availableItems[randomIndex];

        Debug.Log($"[{customerType}손님] 아이템 선택: {selected.itemName}");
        return selected;
    }

    /// <summary>
    /// 아이템의 현재 설정 가격을 평가
    /// </summary>
    PriceEvaluation EvaluatePrice(Item item)
    {
        if (item == null)
        {
            Debug.LogError("[Customer] EvaluatePrice - item이 null입니다");
            return PriceEvaluation.Appropriate;
        }

        if (registeredItems == null)
        {
            Debug.LogError("[Customer] EvaluatePrice - registeredItems가 null입니다");
            return PriceEvaluation.Appropriate;
        }

        int basePrice = item.sellPrice;
        int currentPrice = registeredItems.GetCurrentPrice(item);

        // Division by Zero 방지
        if (basePrice == 0)
        {
            Debug.LogWarning($"[Customer] {item.itemName}의 sellPrice가 0입니다");
            return PriceEvaluation.Appropriate;
        }

        float ratio = (float)currentPrice / basePrice;

        // 평가 기준에 따른 분류
        if (ratio < 0.9f)
        {
            Debug.Log($"[평가] {item.itemName}: {ratio:F2} → 굉장히 쌈");
            return PriceEvaluation.VeryCheap;
        }
        else if (ratio <= 1.1f)
        {
            Debug.Log($"[평가] {item.itemName}: {ratio:F2} → 적정가");
            return PriceEvaluation.Appropriate;
        }
        else if (ratio <= 1.2f)
        {
            Debug.Log($"[평가] {item.itemName}: {ratio:F2} → 비쌈");
            return PriceEvaluation.Expensive;
        }
        else
        {
            Debug.Log($"[평가] {item.itemName}: {ratio:F2} → 너무 비쌈");
            return PriceEvaluation.VeryExpensive;
        }
    }

    /// <summary>
    /// 손님 타입과 가격 평가에 따라 구매 여부 결정
    /// </summary>
    bool DecideToBuy(PriceEvaluation eval)
    {
        float buyChance = 0f;

        switch (customerType)
        {
            case CustomerType.Normal:
                if (eval == PriceEvaluation.VeryCheap || eval == PriceEvaluation.Appropriate)
                    buyChance = 1.0f; // 100%
                else if (eval == PriceEvaluation.Expensive)
                    buyChance = 0.1f; // 10%
                else if (eval == PriceEvaluation.VeryExpensive)
                    buyChance = 0.0f; // 0%
                break;

            case CustomerType.Generous:
                if (eval == PriceEvaluation.VeryCheap || eval == PriceEvaluation.Appropriate)
                    buyChance = 1.0f; // 100%
                else if (eval == PriceEvaluation.Expensive)
                    buyChance = 0.5f; // 50%
                else if (eval == PriceEvaluation.VeryExpensive)
                    buyChance = 0.05f; // 5%
                break;

            case CustomerType.Picky:
                if (eval == PriceEvaluation.VeryCheap || eval == PriceEvaluation.Appropriate)
                    buyChance = 1.0f; // 100%
                else if (eval == PriceEvaluation.Expensive || eval == PriceEvaluation.VeryExpensive)
                    buyChance = 0.0f; // 0%
                break;
        }

        // 확률 판정
        float randomValue = Random.Range(0f, 1f);
        bool decision = randomValue < buyChance;

        return decision;
    }

    IEnumerator BuyItem()
    {
        itemBuyCheck = true;
        agent.SetDestination(salesPos.position);

        Debug.Log($"[{customerType}손님] 판매대로 이동 중...");

        // 판매대 도착 대기
        while (agent.pathPending || agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null;
        }

        // 판매대 도착 - 거래 대기
        Debug.Log($"[{customerType}손님] 판매대에 도착. 거래 대기 중...");

        agent.enabled = false;

        float waitTime = 5f;
        float elapsed = 0f;

        // 5초 동안 대기
        while (elapsed < waitTime)
        {
            transform.rotation = Quaternion.identity;

            // 플레이어가 판매했으면 즉시 거래 완료
            if (itemPayCheck)
            {
                Debug.Log($"[{customerType}손님 {gameObject.name}] 돈 지불 완료!");
                agent.enabled = true;
                state = CustomerState.LeavingShop;
                yield break;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 5초 경과
        Debug.Log($"[{customerType}손님 {gameObject.name}] 너무 오래 걸리네. 나가겠습니다");
        agent.enabled = true;
        state = CustomerState.LeavingShop;
    }

    void LeaveShop()
    {
        agent.enabled = true;
        agent.SetDestination(exited.position);
    }


    // 플레이어와의 충돌 무시
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Physics.IgnoreCollision(GetComponent<Collider>(), collision.collider);
        }
    }

    // 선택된 아이템 조회
    public Item GetSelectedItem()
    {
        return selectedItem;
    }

    // 평가 결과 조회
    public PriceEvaluation GetPriceEvaluation()
    {
        return priceEvaluation;
    }
}
