using System;
using UnityEngine;

public class QuickSlotModel
{
    private Item[] slots;
    public int Capacity { get; private set; }

    public QuickSlotModel(int capacity)
    {
        this.Capacity = capacity;
        slots = new Item[capacity];
    }

    public void SetQickSlot(int index, Item item)
    {
        if (index >= 0 && index < Capacity)
        {
            slots[index] = item;
        }
    }

    public void ClearQickSlot(int index)
    {
        if (index >= 0 && index < Capacity)
        {
            slots[index] = null;
        }
    }

    public Item GetItem(int index)
    {
        if (index >= 0 && index < Capacity)
        {
            return slots[index];
        }
        return null;
    }
}