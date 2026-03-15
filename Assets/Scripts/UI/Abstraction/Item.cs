using System;
using UnityEngine;

[Serializable]
public class Item
{
    public ItemType itemType;
    public int count;

    public Item(ItemType itemType, int count)
    {
        this.itemType = itemType;
        this.count = count;
    }

    public bool IsFull => count >= itemType.maxStackSize;
}