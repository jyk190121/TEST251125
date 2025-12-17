using UnityEngine;
using UnityEngine.UI;

public class Shop_Warehouse : MonoBehaviour
{

    float keyDownTime;                       // 상호작용을 위한 시간

    bool playerIn;                           // 플레이어가 창고에 닿았는지 여부
    float keyTimer;                          // 키 입력 시간
    public Image image;                      // 키입력하는 동안 띄울 이미지
    public Image key;                        // 상호작용 키 알려줄 이미지

    CharacterController player;              // 플레이어 스크립트( 임시 )

    //public ItemSplitPopup itemSplitPopup;  //창고 UI 캔버스 열기/닫기
    public GameObject itemWarehousePanel;    //창고 UI 판넬

    GameObject inventoeyPanel;        //인벤토리 UI 판넬

    bool openWarehousePanel;                 //창고 UI 열려있는지 

    float detectRadius;                      //창고 주변 감지 범위

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        keyDownTime = 1.5f;
        keyTimer = 0f;
        image.gameObject.SetActive(false);
        key.gameObject.SetActive(false);
        image.fillAmount = 0f;
        image.color = new Color(0, 0, 150f, 50f);
        inventoeyPanel = FindAnyObjectByType<ShopManager>().inventoeyPanel;

        //itemSplitPopup = FindAnyObjectByType<ItemSplitPopup>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();

        detectRadius = 2f;
    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetKeyDown(KeySetting.keys[KeyInput.CANCLE]))
        //{
        //    itemWarehousePanel.SetActive(false);
        //    inventoeyPanel.SetActive(false);
        //    openWarehousePanel = false;
        //}


        // 플레이어가 창고 주변 detectRadius 반경 안에 있는지 검사
        playerIn = Vector3.Distance(player.transform.position, transform.position) < detectRadius;

        // 주변에 없으면 UI 초기화
        if (!playerIn && openWarehousePanel)
        {
            openWarehousePanel = false;
            inventoeyPanel.SetActive(false);
        }

        if (!playerIn)
        {
            if (image == null) return;
            if (key == null) return;
            if (itemWarehousePanel == null) return;
            playerIn = false;
            keyTimer = 0f;
            image.gameObject.SetActive(false);
            key.gameObject.SetActive(false);
            itemWarehousePanel.SetActive(false);

            return;
        }
       
        // 주변에 있으면 상호작용 키 노출
        if (!openWarehousePanel) key.gameObject.SetActive(true);

        // 상호작용 키 입력 확인
        if (Input.GetKey(KeySetting.keys[KeyInput.INTERACTIVE]))
        {
            keyTimer += Time.deltaTime;

            key.gameObject.SetActive(false);
            image.gameObject.SetActive(true);
            image.fillAmount = keyTimer;

            if (keyTimer >= keyDownTime)
            {
                image.gameObject.SetActive(false);
                //창고개방
                print("창고개방");
                //itemSplitPopup.gameObject.SetActive(true);
                itemWarehousePanel.SetActive(true);
                inventoeyPanel.SetActive(true);
                openWarehousePanel = true;
            }
        }
        else
        {
            keyTimer = 0f;
            image.gameObject.SetActive(false);
        }

        //// 플레이어가 창고 근처에 있으면 상호작용 키 누름 시간 체크
        //if (playerIn)
        //{
        //    if (Input.GetKey(KeySetting.keys[KeyInput.INTERACTIVE]))
        //    {
        //        keyTimer += Time.deltaTime;
        //        key.gameObject.SetActive(false);
        //        image.gameObject.SetActive(true);
        //        image.fillAmount = keyTimer;

        //        if (keyTimer >= keyDownTime)
        //        {
        //            image.gameObject.SetActive(false);
        //            //창고개방
        //            print("창고개방");
        //            //itemSplitPopup.gameObject.SetActive(true);
        //            itemWarehousePanel.SetActive(true);
        //            openWarehousePanel = true;
        //        }
        //    }
        //    else
        //    {
        //        keyTimer = 0f;
        //        //image.fillAmount = 0f;
        //        image.gameObject.SetActive(false);
        //        //itemSplitPopup.gameObject.SetActive(false);

        //        if (!openWarehousePanel)
        //        {
        //            itemWarehousePanel.SetActive(false);
        //        }
        //    }
        //}
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.collider.CompareTag("Player"))
    //    {
    //        playerIn = true;
    //        player = collision.collider.GetComponent<PlayerController_Shop>();
    //        key.gameObject.SetActive(true);
    //    }
    //}

    //private void OnCollisionExit(Collision collision)
    //{
    //    if (collision.collider.CompareTag("Player"))
    //    {
    //        playerIn = false;
    //        keyTimer = 0f;
    //        image.gameObject.SetActive(false);
    //        key.gameObject.SetActive(false);
    //        itemWarehousePanel.SetActive(false);
    //    }
    //}
}
