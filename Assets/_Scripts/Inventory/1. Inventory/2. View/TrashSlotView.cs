using UnityEngine;
using UnityEngine.EventSystems;

public class TrashSlotView : MonoBehaviour, IDropHandler
{
    //누군가 쓰레기통에 아이템을 드롭했을 때 실행됨
    public void OnDrop(PointerEventData eventData)
    {
        //매니저에게 "지금 드래그 중인 아이템 삭제해!"라고 명령
        InventoryManager.Instance.OnDropToTrash();
    }
}
