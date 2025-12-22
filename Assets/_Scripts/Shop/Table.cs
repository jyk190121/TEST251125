using UnityEngine;
using UnityEngine.UI;

public class Table : MonoBehaviour
{
    RegisteredItemTemp regiItem;

    public Canvas canvas;
    public Image[] tableImage;
    public bool update;

    private void Start()
    {
        regiItem = canvas.GetComponentInChildren<RegisteredItemTemp>(true);
        update = false;
    }
    void Update()
    {
        //등록된 아이템 변경될 때 이미지 변경
        if (update)
        {
            UpdateTable();
        }
    }

    void UpdateTable()
    {
        update = false;

        for (int i = 0; i < tableImage.Length; i++)
        {
            if (regiItem.itemImages[i].sprite == null)
            {
                continue;
            }  

            tableImage[i].gameObject.SetActive(true);
            tableImage[i].sprite = regiItem.itemImages[i].sprite;
            tableImage[i].enabled = true;
            tableImage[i].preserveAspect = true;

            tableImage[i] = regiItem.itemImages[i];
        }

    }
}
