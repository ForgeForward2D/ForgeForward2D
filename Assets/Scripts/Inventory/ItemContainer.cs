using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemContainer : MonoBehaviour
{
    [SerializeField] protected int capacity;
    protected Item[] items;

    public event Action OnContentChanged;

    protected virtual void Awake()
    {
        items = new Item[capacity];
    }

    public Item[] GetItems() => items;

    protected void NotifyContentsChanged() => OnContentChanged?.Invoke();
}