using UnityEngine;
using UnityEngine.UI;
using static KJY_Camera;
using static RegisteredItem;

[RequireComponent (typeof (Table))]
public class Table : MonoBehaviour
{
    //RegisteredItem regiItem;

    public Image[] tableImage;

    void OnEnable()
    {
        OnCameraArrived += HandleCameraArrived;
    }

    void HandleCameraArrived(CameraArea area)
    {

        if (this == null) return;

        if (area == CameraArea.Shop)
        {
            Invoke("OpenTable", 0.15f);
            return;
        }
        else
        {
            CloseTable();
            return;
        }
    }

    private void OnDestroy()
    {
        OnCameraArrived -= HandleCameraArrived;
    }

    public void UpdateTable()
    {
        // DataManager에서 직접 진열대 데이터 가져오기
        var dataManager = _MasterManager.Instance.DataManager;

        if (dataManager == null)
        {
            Debug.LogError("[Table] DataManager를 찾을 수 없습니다");
            return;
        }

        var registeredItemsData = dataManager.GetRegisteredItems();

        if (registeredItemsData == null || registeredItemsData.Length == 0)
        {
            Debug.LogWarning("[Table] 진열대 데이터가 없습니다");
            return;
        }

        // RegisteredItem에서 itemImages를 가져와야 한다면
        RegisteredItem regiItem = FindAnyObjectByType<RegisteredItem>();

        if (regiItem == null)
        {
            Debug.LogWarning("[Table] RegisteredItem을 찾을 수 없습니다 (아이콘 업데이트 스킵)");
            return;
        }

        for (int i = 0; i < tableImage.Length; i++)
        {
            //// itemImages 확인
            //if (regiItem.itemImages == null || i >= regiItem.itemImages.Length)
            //{
            //    tableImage[i].gameObject.SetActive(false);
            //    continue;
            //}

            //if (regiItem.itemImages[i].sprite == null)
            //    continue;

            //// DataManager의 데이터 사용
            //if (registeredItemsData[i] == null || registeredItemsData[i].count < 1)
            //{
            //    tableImage[i].sprite = null;
            //    tableImage[i].gameObject.SetActive(false);
            //    tableImage[i].preserveAspect = false;
            //    continue;
            //}

            //tableImage[i].sprite = regiItem.itemImages[i].sprite;
            //tableImage[i].gameObject.SetActive(true);
            //tableImage[i].preserveAspect = true;

            if (i >= registeredItemsData.Length)
            {
                tableImage[i].gameObject.SetActive(false);
                continue;
            }

            RegisteredItemData data = registeredItemsData[i];

            // 데이터가 없거나 수량이 0이면 무조건 제거
            if (data == null || data.count <= 0)
            {
                tableImage[i].sprite = null;
                tableImage[i].gameObject.SetActive(false);
                tableImage[i].preserveAspect = false;
                continue;
            }

            // sprite가 없으면 표시 불가
            if (regiItem.itemImages == null ||
                i >= regiItem.itemImages.Length ||
                regiItem.itemImages[i].sprite == null)
            {
                tableImage[i].gameObject.SetActive(false);
                continue;
            }

            // 정상 표시
            tableImage[i].sprite = regiItem.itemImages[i].sprite;
            tableImage[i].gameObject.SetActive(true);
            tableImage[i].preserveAspect = true;

        }
    }



    public void OpenTable()
    {
        gameObject.SetActive(true);
    }

    public void CloseTable()
    {
        gameObject.SetActive(false);
    }
}
