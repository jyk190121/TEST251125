using UnityEngine;
using UnityEngine.InputSystem;
using static DayManager;
/// <summary>
/// 1. 밤/낮을 구분해주는 기능 (DayManager)
///  - 낮 : 플레이어가 계산대 앞에서 상호작용 키로 판매시작 / 아이템 들고 온 손님 존재할 땐 : 판매
///  - 밤 : 상점 닫힘, 낮에 상점 열었을 경우 - 판매종료 UI 띄우기
/// 
/// 2. 플레이어가 계산대 위치로 이동했는지 파악
///  - POS_playerSalas로부터 받아오면 댐
/// 
/// 3. 손님이 모두 나갈경우 밤으로 바꾸기
/// 
/// 4. 손님 존재여부 파악
///  - 손님이 있을 때 : 플레이어가 계산대 앞에서 상호작용 버튼으로 판매
///  - 손님이 없을 때 : UI 안띄움?
///  
/// </summary>
public class ShopManager : MonoBehaviour
{
    DayManager dayManager;                 //낮, 밤 체크용
    public bool isAction;           
    POS_playerSalas pos_palyer;            //포스기
    CustomerManager customerManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
        dayManager = FindAnyObjectByType<DayManager>();
        pos_palyer = FindAnyObjectByType<POS_playerSalas>();
        customerManager = FindAnyObjectByType<CustomerManager>();

        pos_palyer.image.gameObject.SetActive(false);
        pos_palyer.shopOpenCheck = false;
        //UI로 상호작용키 띄워주기
        pos_palyer.key.text = $"{KeySetting.GetKeyString(KeyInput.INTERACTIVE)}";
        //손님이 아이템을 가져오면 '판매' 라는 문구 로 변경
        //sales.text = "판매 시작";

        pos_palyer.posUpdate();
    }

    // Update is called once per frame
    void Update()
    {
        //낮인지
        if(dayManager.IsDay && !isAction)
        {
            //상호작용 키로 상점 오픈하기
            if (Input.GetKeyDown(KeySetting.keys[KeyInput.INTERACTIVE]) && pos_palyer.playerIsSales)
            {
                if(!pos_palyer.shopOpenCheck)
                {
                    OpenShop();
                    //item 판매 (손님위치 - 계산대인지체크)
                }
            }

            //손님이 다 나갔을 때 밤으로 만들자
            if (customerManager.GetCustomerAllExit())
            {
                isAction = true;
            }

        }

        //밤인지
        else if (dayManager.IsNight && isAction)
        {
            CloseShop();
        }

        //낮, 밤 변경
        if (isAction)
        {
            ChangeDay();
        }

    }

    void OpenShop()
    {
        pos_palyer.shopOpenCheck = true;
        pos_palyer.posUpdate();

        StartCoroutine( customerManager.CreateCustomer(5));
    }

    void CloseShop()
    {
        isAction = false;
        pos_palyer.shopOpenCheck = false;
        pos_palyer.posUpdate();
        pos_palyer.image.gameObject.SetActive(false);
        
        dayManager.ChangeTimeOfDay(TimeOfDay.Night);
        print("밤됫대");
    }

    void ChangeDay()
    {
        if (dayManager.IsDay)
        {
            dayManager.ChangeTimeOfDay(TimeOfDay.Night);
        }
        else if (dayManager.IsNight)
        {
            dayManager.ChangeTimeOfDay(TimeOfDay.Day);
        }
    }
}
