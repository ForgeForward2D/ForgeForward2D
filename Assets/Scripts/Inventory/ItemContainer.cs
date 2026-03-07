using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemContainer : MonoBehaviour
{
    [SerializeField] protected int capacity;
    protected InventoryItem[] items;

    public event Action OnContentChanged;

    protected virtual void Awake()
    {
        items = new InventoryItem[capacity];
    }

    public InventoryItem[] GetItems()
    {
        return items;
    }

    protected void NotifyContentsChanged()
    {
        OnContentChanged?.Invoke();
    }
}