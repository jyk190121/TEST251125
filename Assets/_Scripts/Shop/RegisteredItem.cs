using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
/// <summary>
/// 인벤토리에 있는 아이템 팔기
///  - 인벤토리 아이템 리스트
///  - 현재 등록된 아이템 보여주기 v
///  - 현재 등록된 아이템 제거기능
///  - 인벤토리에서 아이템 등록 시 아이템 갯수 제거 <-> 등록된 아이템 추가
/// </summary>

[System.Serializable]
public class RegisteredItem : MonoBehaviour
{
    //등록된 아이템 리스트
    public Item[] itemList;

    public Transform itemParent;     // ItemPanel
    public GameObject imagePrefab;   // Image가 달린 프리팹

    private void Start()
    {
        GameObject[] slot = new GameObject[itemList.Length];

        for (int i = 0; i < itemList.Length; i++)
        {
            if (itemList[i] == null) continue;

            // 슬롯 생성
            slot[i] = Instantiate(imagePrefab, itemParent);

            // 아이콘 Image 생성
            GameObject images = new GameObject("Icon");
            images.transform.SetParent(slot[i].transform, false);

            Image itemImage = images.AddComponent<Image>();
            itemImage.sprite = itemList[i].icon;
            itemImage.enabled = true;
            itemImage.preserveAspect = true;

        }
    }
}
