using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static DayManager;
using static RegisteredItem;
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
    public GameObject inventoeyPanel;       //인벤토리 UI 판넬

    public Result_Shop resultItem;          //판매결과 UI

    public DayManager dayManager;           //낮, 밤 체크용
    public bool isAction;                   //판매활동했는지
    bool isRegiItemOpen;
    POS_playerSalas pos_palyer;             //포스기
    SalesCustomer salesCustomer;            //손님 계산대 앞에 있는지 여부
    DisplayStand itemDisplay;               //아이템 UI 열고 닫기
    Shop_Warehouse warehouse;               //창고판넬도 취소키로 꺼주자
    CustomerManager customerManager;
    DataManager dataManager;
    SoundManager soundManager;
    Party_Shop_Night party;
    InventoryManager inventoryManager;

    bool partyPlay;

    //public GameObject light_Shop;
    Light_Shop light_Shop;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        //dayManager = FindAnyObjectByType<DayManager>();
        dayManager = _MasterManager.Instance.DayManager;
        dataManager = _MasterManager.Instance.DataManager;
        customerManager = FindAnyObjectByType<CustomerManager>();
        pos_palyer = FindAnyObjectByType<POS_playerSalas>();
        light_Shop = FindAnyObjectByType<Light_Shop>();
        salesCustomer = FindAnyObjectByType<SalesCustomer>();
        party = FindAnyObjectByType<Party_Shop_Night>();
        itemDisplay = FindAnyObjectByType<DisplayStand>();
        warehouse = FindAnyObjectByType<Shop_Warehouse>();

        inventoryManager = _MasterManager.Instance.InventoryManager;

        Inventory inventory = inventoryManager.GetComponentInChildren<Inventory>(true); ;
        inventoeyPanel = inventory.gameObject;

        if(inventoryManager.itemView == null)
        {
            inventoryManager.quickSlotView.gameObject.SetActive(true);
            inventoryManager.equipView.SetActive(true);
        }
        isRegiItemOpen = false;

        pos_palyer.image.gameObject.SetActive(false);
        pos_palyer.shopOpenCheck = false;
        partyPlay = true;
        //UI로 상호작용키 띄워주기
        pos_palyer.key.text = $"{KeySetting.GetKeyString(KeyInput.INTERACTIVE)}";
        //손님이 아이템을 가져오면 '판매' 라는 문구 로 변경
        //sales.text = "판매 시작";
        soundManager = _MasterManager.Instance.SoundManager;

        soundManager.StopBGM();

        isAction = false;
        pos_palyer.posUpdate();

        party.StopParty();
    }
    // Update is called once per frame
    void Update()
    {
        //낮인지
        if (dayManager.IsDay && !isAction)
        {
            if(!soundManager.PlayingBGM()) soundManager.PlayBGM("진영", 0);

            if (!partyPlay) party.StopParty();

            //상호작용 키로 상점 오픈하기
            if (Input.GetKeyDown(KeySetting.keys[KeyInput.INTERACTIVE]) && pos_palyer.playerIsSales)
            {

                if (!pos_palyer.shopOpenCheck)
                {
                    OpenShop();
                }

                //print($"판매대 앞에 손님 존재 : {salesCustomer.HasCustomer()}");

                //item 판매 (손님위치 - 계산대인지체크)
                if (salesCustomer.HasCustomer())
                {
                    Customer buyCustomer = salesCustomer.GetCurrentCustomer();

                    //한손님당 한번만 계산하도록
                    if (!buyCustomer.itemPayCheck)
                    {
                        //soundManager.PlaySFXIndex(0);
                        soundManager.PlaySFX("진영", 0);

                        // 손님이 선택한 아이템 가격 가져오기
                        Item boughtItem = buyCustomer.GetSelectedItem();
                        if (boughtItem != null)
                        {
                            RegisteredItem registeredItem = FindAnyObjectByType<RegisteredItem>();
                            
                            int actualPrice = registeredItem.GetCurrentPrice(boughtItem);

                            buyCustomer.DecreaseRegisteredItemCount(boughtItem);
                            SalesResultManager.Instance.AddSale(boughtItem, 1 , actualPrice);

                            //판매한 아이템에 추가

                            dataManager.EarnMoney(actualPrice);

                            Debug.Log($"[ShopManager] {boughtItem.itemName} 판매 완료! 수익: {actualPrice} gold");
                        }

                        //손님 계산완료처리
                        customerManager.CustomerBuyItem();
                    }
                }
            }

            //손님이 다 나갔을 때 밤으로 만들자
            if (customerManager.GetCustomerAllExit() && pos_palyer.shopOpenCheck)
            {
                ChangeDay();
                isAction = true;
            }

            //판매 등록 UI 열기
            if (Input.GetKeyDown(KeySetting.keys[KeyInput.INTERACTIVE]) && itemDisplay.image.gameObject.activeSelf == true)
            {
                //print("상호작용 키 입력");
                itemDisplay.image.gameObject.SetActive(false);
                //아이템 등록 열기
                inventoryManager.quickSlotView.gameObject.SetActive(false);
                inventoryManager.equipView.SetActive(false);
                itemDisplay.regiItemUI.gameObject.SetActive(true);
                inventoeyPanel.SetActive(true);
            }
            else if(itemDisplay.regiItemUI.gameObject.activeSelf == true)
            {
                inventoryManager.quickSlotView.gameObject.SetActive(false);
                inventoryManager.equipView.SetActive(false);
                isRegiItemOpen = true;

                if(Input.GetMouseButtonDown(1))
                {
                    RegiItem.UnregisterLastItemToInventory();
                    //if (RegiItem.GetSlotIndexUnderMouse(out RegisteredItemData data) != -1)
                    //{
                    //    RegiItem.UnregisterItem(data);
                    //}
                }
               
            }
            else if(isRegiItemOpen)
            {
                isRegiItemOpen = false;
                inventoryManager.quickSlotView.gameObject.SetActive(true);
                inventoryManager.equipView.SetActive(true);
            }
        }
       
        //밤인지
        else if (dayManager.IsNight)
        {
            if (isAction)
            {
                CloseShop();
                //ChangeDay();
                if (partyPlay)
                {
                    partyPlay = false;
                    StartCoroutine(party.partyToNight());
                }
                //soundManager.PlayShopBGMIndex(0);
                soundManager.StopBGM();
                soundManager.PlayBGM("진영", 2);
            }
            else
            {
                soundManager.StopBGM();
            }
            //판매 등록 UI 열기
            //if (Input.GetKeyDown(KeySetting.keys[KeyInput.INTERACTIVE]) && itemDisplay.image.gameObject.activeSelf == true)
            //{
            //    //print("상호작용 키 입력");
            //    itemDisplay.image.gameObject.SetActive(false);
            //    //itemDisplay.nightImage.gameObject.SetActive(true);
            //}

            //판매 등록 UI 열기
            if (Input.GetKeyDown(KeySetting.keys[KeyInput.INTERACTIVE]) && itemDisplay.image.gameObject.activeSelf == true)
            {
                //print("상호작용 키 입력");
                itemDisplay.image.gameObject.SetActive(false);
                //아이템 등록 열기
                inventoryManager.quickSlotView.gameObject.SetActive(false);
                inventoryManager.equipView.SetActive(false);
                itemDisplay.regiItemUI.gameObject.SetActive(true);
                inventoeyPanel.SetActive(true);
            }
            else if (itemDisplay.regiItemUI.gameObject.activeSelf == true)
            {
                inventoryManager.quickSlotView.gameObject.SetActive(false);
                inventoryManager.equipView.SetActive(false);
                isRegiItemOpen = true;

                if (Input.GetMouseButtonDown(1))
                {
                    RegiItem.UnregisterLastItemToInventory();
                    //if (RegiItem.GetSlotIndexUnderMouse(out RegisteredItemData data) != -1)
                    //{
                    //    RegiItem.UnregisterItem(data);
                    //}
                }

            }
            else if (isRegiItemOpen)
            {
                isRegiItemOpen = false;
                inventoryManager.quickSlotView.gameObject.SetActive(true);
                inventoryManager.equipView.SetActive(true);
            }
        }

        //조명 조절
        switch (dayManager.CurrentTime)
        {
            case TimeOfDay.Day:
                light_Shop.OnLight();
                break;

            case TimeOfDay.Night:
                light_Shop.OffLight();
                break;
        }
        
        //취소(닫기) 버튼
        if (Input.GetKeyDown(KeySetting.keys[KeyInput.CANCLE]))
        {
            if (itemDisplay != null)
            {
                itemDisplay.image.gameObject.SetActive(false);
                itemDisplay.regiItemUI.gameObject.SetActive(false);
                //itemDisplay.nightImage.gameObject.SetActive(false);
            }
            if (warehouse != null)
            {
                warehouse.itemWarehousePanel.gameObject.SetActive(false);
            }
            if(resultItem != null)
            {
                resultItem.CloseResultSell();
            }
            //itemDisplay.nightImage.gameObject.SetActive(false);
            inventoeyPanel.SetActive(false);
        }
    }

    void OpenShop()
    {
        pos_palyer.shopOpenCheck = true;
        pos_palyer.posUpdate();
        StartCoroutine(customerManager.CreateCustomer(10));
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
        //print("밤됫대");

        //오늘 판매한 UI 도 만들어야댐
        //SalesResultManager.Instance.GetAllResults();
        resultItem.OpenResultSell();
    }

    //낯 밤 변경
    public void ChangeDay()
    {
        if (dayManager.IsDay)
        {
            dayManager.ChangeTimeOfDay(TimeOfDay.Night);
        }
        else if (dayManager.IsNight)
        {
            //판매결과 초기화
            SalesResultManager.Instance.ClearResults();
            dayManager.ChangeTimeOfDay(TimeOfDay.Day);
        }
    }
}
