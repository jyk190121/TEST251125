using NUnit.Framework.Internal.Execution;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 인벤토리에 있는 아이템 팔기
///  - 인벤토리 아이템 리스트 (인벤토리 띄우기)v
///  - 현재 등록된 아이템 보여주기 v
///  - 인벤토리에서 아이템 등록 시 아이템 갯수 제거 <-> 등록된 아이템 추가
///  - 현재 등록된 아이템 제거기능
/// </summary>

[System.Serializable]
public class RegisteredItemTemp : MonoBehaviour
{
    public static RegisteredItemTemp RegiItem;

    public ItemSettingPopup dropPopup;          // 팝업창 (아이템 갯수, 판매가격 설정창)



    public Item[] itemList;                     // 등록된 아이템 리스트
    public Image[] itemImages;                  // 등록된 아이템 이미지
    int capacity = 4;                           // 아이템 등록 가능 총갯수

    //드래그 시작한 슬롯 번호 (-1: 아무것도 안 잡음)
    private int dragStartIndex = -1;

    public Transform itemParent;                 // ItemPanel

    [Header("현재 등록할 아이템 정보")]
    public Image currentImage;                   // 현재 등록할 item 이미지(아이콘)
    public TMP_InputField currentCount;          // 현재 등록할 item의 갯수
    public TMP_InputField currentPrice;          // 현재 등록할 item의 가격

    InventoryManager inventory;
    Item dropItem;                               // 현재 드롭중인 아이템

    private void Awake()
    {
        if (RegiItem == null) RegiItem = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        //현재 등록된 아이템 리스트 UI에 띄우기
        for (int i = 0; i < itemList.Length; i++)
        {
            if (itemList[i] == null) continue;

            // 아이콘 Image 생성
            GameObject images = new GameObject("Image");
            images.transform.SetParent(itemImages[i].GetComponentInChildren<Image>().transform, false);

            itemImages[i] = images.AddComponent<Image>();
            itemImages[i].sprite = itemList[i].icon;
            itemImages[i].enabled = true;
            itemImages[i].preserveAspect = true;
        }


    }

    // Update is called once per frame
    private void Update()
    {


        //마우스로 선택한 아이템 정보 가져오기
        if (Input.GetMouseButtonDown(0))
        {
            print($"{inventory.GetDragStartIndex()} 현재 인덱스?");
        }
       
    }

    //드래그 시작 시 호출
    public void OnDragStart(int index)
    {
        dragStartIndex = index;
    }

    //드래그 종료 시 호출
    public void OnDragEnd(int dropIndex)
    {
        //인벤토리에서 아이템등록으로
        if (inventory.GetDragStartIndex() != -1)
        {
            if (dropIndex == -1) return;

            //인벤토리 매니저에서 드래그한 아이템 가져오기
            Item inventoryItem = inventory.GetDraggedItem();
            int inventoryCount = inventory.GetDraggedItemCount();
            int inventoryIndex = inventory.GetDragStartIndex();

            if (inventoryItem == null) return;

            //소분 수량이 1보다 많으면 실행
            if (inventoryCount > 1)
            {
                dropPopup.OpenPopup(inventoryItem.itemName,
            //YES 눌렀을 때: 아이템 삭제
            onYes: () =>
            {
                
                dragStartIndex = -1;              // 드래그 상태 초기화
            },
            //NO 눌렀을 때: 드래그 취소 (제자리 복귀)
            onNo: () =>
            {
                CancelDrag();
            }
        );
            }
            else
            {
                //1개면 바로 이동

            }
        }


    }

    //창고 슬롯이 아닌 곳에 Drop했을 때
    public void CancelDrag()
    {
        dragStartIndex = -1;    //드래그 상태 초기화        
    }

}


