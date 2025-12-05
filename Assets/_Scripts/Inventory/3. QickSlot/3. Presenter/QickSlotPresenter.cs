using UnityEngine;

public class QickSlotPresenter : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //숫자 키 입력으로 퀵슬롯 아이템 사용
        if (Input.GetKeyDown(KeyCode.Alpha1)) UseItem(0);
    }

    public void RegisterItem(Item newItem)
    {
        if(newItem.type != ItemType.Potion)
        {
            Debug.Log("소비 아이템만 퀵슬롯에 등록할 수 있습니다.");
            return;
        }
    }

    public void UseItem(int slotIndex)
    {

    }
}
