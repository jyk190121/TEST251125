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
                //파밍한 아이템 정보도 데이터 매니저에 저장 -> 안에 있는 BattelRecord로 전송
                _MasterManager.Instance.DataManager.GetItem(item);
                Destroy(this.gameObject);
            }
        }
    }

}
