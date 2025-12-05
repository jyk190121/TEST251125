using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;


public class QickSlotModel
{
    //내부 저장소 배열
    private Item[] slots;

    //퀵슬롯 수
    public int Capacity { get; private set; }

    //데이터 변경 알림 (옵저버 패턴)
    public event Action OnQickSlotUpdated;

    //생성자
    public QickSlotModel(int capacity)
    {
        this.Capacity = capacity;
        slots = new Item[capacity];
    }

    //QickSlotView에게 데이터 반환
    public Item[] GetSlotsForView()
    {
        return slots;
    }

    //퀵슬롯에 아이템 등록
    public void SetQickSlot(int index, Item item)
    {
        if(index >= 0 && index < Capacity)
        {
            slots[index] = item;
            OnQickSlotUpdated?.Invoke(); //변경 알림
        }
    }

    //퀵슬롯 아이템 비우기
    public void ClearQickSlot(int index)
    {
        if (index >= 0 && index < Capacity)
        {
            slots[index] = null;
            OnQickSlotUpdated?.Invoke(); //변경 알림
        }
    }
    
    //특정 슬롯의 아이템 가져오기 (매니저용)
    public Item GetItem(int index)
    {
        if (index >= 0 && index < Capacity)
        {
            return slots[index];
        }
        return null;
    }
}
