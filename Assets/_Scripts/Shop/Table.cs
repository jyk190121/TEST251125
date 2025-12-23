using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof (Canvas))]
public class Table : MonoBehaviour
{
    RegisteredItem regiItem;

    public Canvas canvas;
    public Image[] tableImage;

    private void Start()
    {
        regiItem = canvas.GetComponentInChildren<RegisteredItem>(true);
    }
    //void Update()
    //{
    //    //등록된 아이템 변경될 때 이미지 변경
    //    if (update)
    //    {
    //        UpdateTable();
    //    }
    //}

    //void UpdateTable()
    //{
    //    update = false;

    //    for (int i = 0; i < tableImage.Length; i++)
    //    {
    //        if (regiItem.itemImages[i].sprite == null)
    //        {
    //            continue;
    //        }  

    //        tableImage[i].gameObject.SetActive(true);
    //        tableImage[i].sprite = regiItem.itemImages[i].sprite;
    //        tableImage[i].enabled = true;
    //        tableImage[i].preserveAspect = true;

    //        tableImage[i] = regiItem.itemImages[i];
    //    }

    //}
    public void UpdateTable()
    {
        for (int i = 0; i < tableImage.Length; i++)
        {
            if (regiItem.itemImages[i].sprite == null) continue;

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
