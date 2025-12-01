using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryModel
{
    //내부 저장소: 딕셔너리 (Key: 슬롯 인덱스)
    private Dictionary<int, InventorySlotModel> slots = new Dictionary<int, InventorySlotModel>();

    public int Capacity { get; private set; }

    //데이터 변경 알림 (옵저버 패턴)
    public event Action OnInventoryUpdated;

    public InventoryModel(int capacity)
    {
        this.Capacity = capacity;
    }

    //View가 그리기 쉽도록 딕셔너리를 배열로 변환하여 반환
    public InventorySlotModel[] GetSlotsForView()
    {
        InventorySlotModel[] viewArray = new InventorySlotModel[Capacity];

        for (int i = 0; i < Capacity; i++)
        {
            if (slots.ContainsKey(i))
            {
                viewArray[i] = slots[i];
            }
            else
            {
                //딕셔너리에 없으면 빈 슬롯 객체 생성
                viewArray[i] = new InventorySlotModel();
            }
        }
        return viewArray;
    }

    //아이템 보관
    public bool AddItem(Item itemData, int count = 1)
    {
        //중첩 가능한 아이템 처리
        if (itemData.isStackable)
        {
            foreach (var kvp in slots)
            {
                InventorySlotModel slot = kvp.Value;

                //같은 ID의 아이템 && maxStackSize에 도달하지 않은 아이템인 경우
                if (slot.itemData != null && slot.itemData.ID == itemData.ID && slot.quantity < itemData.maxStackSize)
                {
                    //이 슬롯에 더 들어갈 수 있는 공간 계산 (예: 99 - 90 = 9개 공간)
                    int spaceLeft = itemData.maxStackSize - slot.quantity;
                    
                    if (count <= spaceLeft)
                    {
                        //최대 스택 개수 체크
                        slot.quantity += count;
                        OnInventoryUpdated?.Invoke();
                        return true;
                    }
                    else
                    {
                        //획득한 아이템이 공간보다 많음 (넘침)
                        //일단 이 슬롯을 꽉 채우고 (99개)
                        slot.quantity = itemData.maxStackSize;

                        //넣은 만큼 count에서 뺌 (남은 개수를 들고 다음 슬롯을 찾으러 감)
                        count -= spaceLeft;
                    }
                }
            }
        }

        //남은 개수(count)를 새 슬롯들에 나눠 담기
        //(count가 0이 될 때까지 반복: 만약 200개를 한 번에 얻었다면 슬롯 3개를 써야 하니까 while문 사용)
        while (count > 0)
        {
            // 빈 슬롯 번호(Key) 찾기
            int emptySlotIndex = -1;
            for (int i = 0; i < Capacity; i++)
            {
                if (!slots.ContainsKey(i) || slots[i].IsEmpty)
                {
                    emptySlotIndex = i;
                    break;
                }
            }

            if (emptySlotIndex != -1) //빈 자리를 찾았다면
            {
                //기존 데이터가 남았을 수 있으니, 새로 덮어쓰거나 채운다.
                if (slots.ContainsKey(emptySlotIndex))
                {
                    slots[emptySlotIndex].Set(itemData, Mathf.Min(count, itemData.maxStackSize));
                }
                else
                {
                    InventorySlotModel newSlot = new InventorySlotModel();
                    newSlot.Set(itemData, Mathf.Min(count, itemData.maxStackSize));

                    //딕셔너리에 등록
                    slots.Add(emptySlotIndex, newSlot);
                }
                //이번 슬롯에 넣을 양 결정 (남은 게 99개보다 많으면 99개만, 적으면 전부)
                int amountToAdd = Mathf.Min(count, itemData.maxStackSize);
                
                //처리한 만큼 남은 개수 차감
                count -= amountToAdd; 
                
            }
            else
            {
                //빈 자리가 없는데 아직 넣을 아이템이 남음 (인벤토리 꽉 참)
                Debug.Log($"인벤토리가 가득 차서 아이템 {count}개를 줍지 못했습니다.");
                //들어간 만큼이라도 갱신
                OnInventoryUpdated?.Invoke();
                //일부 실패 (필요에 따라 true로 처리해도 됨)
                return false;                   
            }
        }

        //여기까지 왔으면 모든 아이템 처리 완료
        OnInventoryUpdated?.Invoke();
        return true;
    }

    public int GetItemCount(int targetItemID)
    {
        //총 수량 초기화
        int totalCount = 0;

        //딕셔너리 내 모든 슬롯을 전부 검사
        foreach (var slot in slots.Values)
        {
            //빈 슬롯은 넘김
            //아이템 ID가 같은지 확인
            if(slot.IsEmpty && slot.itemData.ID == targetItemID)
            {
                //같으면 토탈 카운트에 해당 슬롯 아이템의 수량을 누적
                //묶음으로 여러개 소지하고 있어도 아이템 ID가 같으면 지속적으로 누적
                totalCount += slot.quantity;
            }
        }
        //토탈 카운트 반환
        return totalCount;
    }

    //특정 슬롯의 데이터를 강제로 덮어쓰기 (교환용)
    public void AddItemToSlot(int index, InventorySlotModel slotData)
    {
        if (slots.ContainsKey(index))
        {
            slots[index] = slotData;
        }
        else
        {
            slots.Add(index, slotData);
        }
    }

    //Manager UI 갱신용 함수
    public void NotifyUpdate()
    {
        OnInventoryUpdated?.Invoke();
    }

    //아이템 제거 (인벤토리 내에서 사용)
    public void RemoveItem(int index)
    {
        if (slots.ContainsKey(index))
        {
            //딕셔너리에서 삭제
            slots.Remove(index);
            OnInventoryUpdated?.Invoke();
        }
    }

    //아이템 제거 (외부에서 사용)
    public bool RemoveItemByCount(int targetItemID, int countToRemove)
    {
        //사용하려는 아이템의 수량이 현재 가지고 있는 수량보다 적으면 실패
        if (GetItemCount(targetItemID) < countToRemove) return false;

        //딕셔너리 내 슬롯 검사
        //딕셔너리 순회 중 데이터 삭제(Clear/Remove)가 발생하면 오류가 발생
        //new List<int>(slots.Keys)를 통해 키 목록을 복사본으로 만들어서 안전하게 순회
        foreach (var key in new List<int>(slots.Keys))
        {
            //목표 수량을 다 지웠으면 종료
            if (countToRemove < 0) break;

            var slot = slots[key];

            //타겟 아이템이 맞는지 아이디 확인
            if(slot.IsEmpty && slot.itemData.ID == targetItemID)
            {
                //이 슬롯에 있는 게, 지워야 할 양보다 많음
                //(예: 슬롯에 10개 있음, 2개만 지워야 함 -> 8개 남김)
                if (slot.quantity > countToRemove)
                {
                    slot.quantity -= countToRemove;
                    countToRemove = 0; // 다 지웠음
                }
                //이 슬롯을 다 비워야 함
                //(예: 슬롯에 5개 있음, 10개 지워야 함 -> 이 슬롯 0개 만들고, 5개 더 지우러 감)
                else
                {
                    //남은 목표치 갱신
                    countToRemove -= slot.quantity;
                    //슬롯 데이터를 완전히 비움 (빈칸 만들기)
                    slot.Clear();
                }
            }
        }

        // 데이터가 변했으니 View(화면)도 갱신하라고 알림 (Observer Pattern)
        OnInventoryUpdated?.Invoke();
        return true; // 성공적으로 삭제함
    }
}