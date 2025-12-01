using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item Data")]
public class Item : ScriptableObject
{
    [Header("기본 정보")]
    public int ID;                  //아이템 고유 ID
    public string itemName;         //아이템 이름
    public Sprite icon;             //아이콘 이미지
    [TextArea(3, 5)]
    public string description;      //아이템 설명

    [Header("스택(겹치기) 설정")]    
    public bool isStackable;        //스택화 가능 유무 (포션, 재료 등)
    public int maxStackSize = 99;   //최대 수량

    [Header("데이터")]
    public int buyPrice;            //구매가
    public int sellPrice;           //판매가

    // (필요하다면) 아이템 사용 시 효과 등을 위한 메서드도 추가 가능
}