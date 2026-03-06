using System;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    public ItemType Item;
    public int Count;

    public InventoryItem(ItemType item, int count)
    {
        this.Item = item;
        this.Count = count;
    }

    public bool IsFull => Count >= Item.MaxStackSize;
}