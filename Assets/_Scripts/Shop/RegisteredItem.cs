using UnityEngine;
/// <summary>
/// 인벤토리에 있는 아이템 팔기
///  - 인벤토리 아이템 리스트
///  - 등록 시 아이템 갯수 제거 <-> 등록된 아이템 추가
///  
/// </summary>
public class RegisteredItem : MonoBehaviour
{
    //등록된 아이템 리스트
    public Item[] itemList;
    
    //인벤토리 아이템 리스트
    InventoryManager inventory;

    private void Start()
    {
        inventory = _MasterManager.Instance.InventoryManager;
    }
    
    public void InventoryList()
    {
        //inventory.
    }

    

}
