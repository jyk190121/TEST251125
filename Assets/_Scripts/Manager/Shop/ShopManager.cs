using UnityEngine;
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
///  - 손님이 없을 때 : UI 안띄움? 현재는 그렇게..
///  
/// </summary>
/// 
[RequireComponent(typeof(ShopManager))]
public class ShopManager : MonoBehaviour
{
    public DayManager dayManager;           //낮, 밤 체크용
    public bool isAction;                   //판매활동했는지
    POS_playerSalas pos_palyer;             //포스기
    SalesCustomer salesCustomer;            //손님 계산대 앞에 있는지 여부
    CustomerManager customerManager;
    DataManager dataManager;
    SoundManager soundManager;

    //public GameObject light_Shop;
    Light_Shop light_Shop;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        //dayManager = FindAnyObjectByType<DayManager>();
        dayManager = _MasterManager.Instance.DayManager;
        dataManager = _MasterManager.Instance.DataManager;
        customerManager = FindAnyObjectByType<CustomerManager>();
        soundManager = FindAnyObjectByType<SoundManager>();
        pos_palyer = FindAnyObjectByType<POS_playerSalas>();
        light_Shop = FindAnyObjectByType<Light_Shop>();
        salesCustomer = FindAnyObjectByType<SalesCustomer>();

        pos_palyer.image.gameObject.SetActive(false);
        pos_palyer.shopOpenCheck = false;
        //UI로 상호작용키 띄워주기
        pos_palyer.key.text = $"{KeySetting.GetKeyString(KeyInput.INTERACTIVE)}";
        //손님이 아이템을 가져오면 '판매' 라는 문구 로 변경
        //sales.text = "판매 시작";

        soundManager.StopBGM();
        pos_palyer.posUpdate();

    }

    // Update is called once per frame
    void Update()
    {
        //print($"플레이어 판매대에 있는 상태 {pos_palyer.playerIsSales}");

        //낮인지
        if (dayManager.IsDay && !isAction)
        {
            if(!soundManager.PlayingBGM())
            {
                //soundManager.PlayShopBGMIndex(0);
                soundManager.PlayBGM("진영", 0);
            }

            //상호작용 키로 상점 오픈하기
            if (Input.GetKeyDown(KeySetting.keys[KeyInput.INTERACTIVE]) && pos_palyer.playerIsSales)
            {
                if(!pos_palyer.shopOpenCheck)
                {
                    OpenShop();
                }

                //print($"판매대 앞에 손님 존재 : {salesCustomer.HasCustomer()}");

                //item 판매 (손님위치 - 계산대인지체크)
                if (salesCustomer.HasCustomer())
                {
                    //soundManager.PlaySFXIndex(0);
                    soundManager.PlaySFX("진영", 0);

                    //골드 100 획득 (임시)
                    dataManager.EarnMoney(100);

                    //손님 계산완료처리
                    customerManager.CustomerBuyItem();
                }
            }

            //손님이 다 나갔을 때 밤으로 만들자
            if (customerManager.GetCustomerAllExit() && pos_palyer.shopOpenCheck)
            {
                isAction = true;
            }

        }

        else if (isAction)
        {
            CloseShop();
            ChangeDay();
        }
        //밤인지 (밤엔 음악끄기)
        else if(dayManager.IsNight)
        {
            soundManager.StopBGM();
        }

        switch (dayManager.CurrentTime)
        {
            case TimeOfDay.Day:
                light_Shop.OnLight();
                break;

            case TimeOfDay.Night:
                light_Shop.OffLight();
                break;
        }
    }

    void OpenShop()
    {
        pos_palyer.shopOpenCheck = true;
        pos_palyer.posUpdate();
        StartCoroutine( customerManager.CreateCustomer(10));
        soundManager.StopBGM();
        soundManager.PlayBGM("진영", 1);
    }

    void CloseShop()
    {
        isAction = false;
        pos_palyer.shopOpenCheck = false;
        //pos_palyer.playerIsSales = false;
        pos_palyer.posUpdate();
        pos_palyer.image.gameObject.SetActive(false);
        print("밤됫대");
    }

    //낯 밤 변경 및 조명 변경
    public void ChangeDay()
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
