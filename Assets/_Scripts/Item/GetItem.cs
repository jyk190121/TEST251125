using UnityEngine;

//아이템 자체에 붙을 스크립트
//아이템의 정보를 가지고 있음
//플레이어가 아이템이랑 부딪히면 자동으로 먹어짐
//꽉차면 안먹어짐
public class GetItem : MonoBehaviour
{
    public Item item;
    public Transform itemImageposition;
    SpriteRenderer SR;

    private void Start()
    {
        SR = itemImageposition.GetComponent<SpriteRenderer>();
        SR.sprite = item.icon;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 29)
        {
            if (_MasterManager.Instance.InventoryManager.AddItem(item))
            {
                Destroy(this.gameObject);
            }
        }
    }

}
