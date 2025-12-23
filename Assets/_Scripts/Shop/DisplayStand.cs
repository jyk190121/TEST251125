using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 등록하기 버튼 UI 띄우고 지우기
/// </summary>
public class DisplayStand : MonoBehaviour
{
    public Image image;
    public Image regiItemUI;
    public Image nightImage;
    public TextMeshProUGUI key;

    [Header("슬롯 UI")]
    public Image[] slotImages;           // 아이템 이미지
    public TextMeshProUGUI[] countTexts; // 수량 표시
    public TextMeshProUGUI[] priceTexts; // 가격 표시

    [Header("아이템 표시")]
    public Transform[] displayShelfPositions; // 각 슬롯의 프리팹 배치 위치

    private RegisteredItem registeredItem;
    private int[] lastCounts = new int[4];
    private int[] lastPrices = new int[4];
    private Item[] lastItems = new Item[4];

    private void Start()
    {
        //registeredItem = FindAnyObjectByType<RegisteredItem>();

        // 초기화
        for (int i = 0; i < 4; i++)
        {
            lastCounts[i] = -1;
            lastPrices[i] = -1;
            lastItems[i] = null;
        }
    }

    private void Update()
    {
        //RefreshDisplayUI();
    }

    /// <summary>
    /// 진열대 UI 갱신
    /// </summary>
    public void RefreshDisplayUI()
    {
        if (registeredItem == null) return;

        RegisteredItem.RegisteredItemData[] itemsData = registeredItem.GetAllRegisteredItemsData();

        for (int i = 0; i < itemsData.Length; i++)
        {
            if (itemsData[i] != null && itemsData[i].item != null && itemsData[i].count > 0)
            {
                if (lastItems[i] != itemsData[i].item ||
                    lastCounts[i] != itemsData[i].count ||
                    lastPrices[i] != itemsData[i].price)
                {
                    // UI 업데이트
                    if (slotImages != null && i < slotImages.Length)
                    {
                        slotImages[i].sprite = itemsData[i].item.icon;
                        slotImages[i].enabled = true;
                    }

                    if (countTexts != null && i < countTexts.Length)
                    {
                        countTexts[i].text = itemsData[i].count.ToString();
                        countTexts[i].enabled = true;
                    }

                    if (priceTexts != null && i < priceTexts.Length)
                    {
                        priceTexts[i].text = itemsData[i].price.ToString();
                        priceTexts[i].enabled = true;
                    }

                    // 프리팹 재배치
                    if (lastItems[i] != itemsData[i].item)
                    {
                        SpawnItemPrefabOnShelf(i, itemsData[i].item);
                    }

                    // 이전 값 저장
                    lastItems[i] = itemsData[i].item;
                    lastCounts[i] = itemsData[i].count;
                    lastPrices[i] = itemsData[i].price;
                }
            }
            else
            {
                // 빈 슬롯 - UI 비활성화
                if (lastItems[i] != null)
                {
                    if (slotImages != null && i < slotImages.Length)
                        slotImages[i].enabled = false;

                    if (countTexts != null && i < countTexts.Length)
                        countTexts[i].enabled = false;

                    if (priceTexts != null && i < priceTexts.Length)
                        priceTexts[i].enabled = false;

                    ClearShelfItem(i);

                    lastItems[i] = null;
                    lastCounts[i] = -1;
                    lastPrices[i] = -1;
                }
            }
        }
    }

    /// <summary>
    /// 진열장 위에 아이템 프리팹 배치
    /// </summary>
    void SpawnItemPrefabOnShelf(int slotIndex, Item item)
    {
        if (displayShelfPositions[slotIndex] == null)
        {
            Debug.LogWarning($"[DisplayStand] {slotIndex}번 진열장 위치가 없습니다");
            return;
        }

        Transform shelfPosition = displayShelfPositions[slotIndex];

        // 기존 프리팹 먼저 제거
        if (shelfPosition.childCount > 0)
        {
            foreach (Transform child in shelfPosition)
            {
                Destroy(child.gameObject);
            }
        }

        // 새 프리팹 생성
        if (item.itemPrefab != null)
        {
            GameObject prefabInstance = Instantiate(
                item.itemPrefab,
                shelfPosition
            );

            prefabInstance.name = item.itemName;
            prefabInstance.transform.localPosition = new Vector3(0, 0.75f, 0);
            prefabInstance.transform.localRotation = Quaternion.Euler(90f, 0, 0);
            prefabInstance.transform.localScale = Vector3.one * 0.3f; // 0.3배 크기

            GraphicRaycaster raycaster = prefabInstance.GetComponent<GraphicRaycaster>();
            if (raycaster != null) Destroy(raycaster);

            // Collider 비활성화
            Collider[] colliders = prefabInstance.GetComponentsInChildren<Collider>();
            foreach (var collider in colliders)
            {
                collider.enabled = false;
            }

            Debug.Log($"[DisplayStand] 진열장 {slotIndex}에 {item.itemName} 배치");
        }
        else
        {
            Debug.LogWarning($"[DisplayStand] {item.itemName}에 프리팹이 없습니다");
        }
    }

    /// <summary>
    /// 진열장에서 아이템 제거
    /// </summary>
    void ClearShelfItem(int slotIndex)
    {
        if (displayShelfPositions[slotIndex] != null && displayShelfPositions[slotIndex].childCount > 0)
        {
            foreach (Transform child in displayShelfPositions[slotIndex])
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            image.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            GameObject inventory = FindAnyObjectByType<ShopManager>().inventoeyPanel;

            image.gameObject.SetActive(false);
            regiItemUI.gameObject.SetActive(false);
            inventory.SetActive(false);
        }
    }
}
