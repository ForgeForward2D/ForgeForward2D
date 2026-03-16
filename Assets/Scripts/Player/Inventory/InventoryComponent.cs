using System;
using UnityEngine;

public interface InventoryComponent<Type>
{
    public void NotifyInventoryUpdate();

    public int CountElements(Type type);
    public int CountFreeSpace(Type type);

    public bool TryAdd(Item item)
    {
        if (item == null || item.itemType == null)
        {
            Debug.LogWarning("Try add invalid item");
            return false;
        }
        if (item.itemType is Type type)
        {
            // add as many items as possible stopping when space is full
            bool success = CountFreeSpace(type) >= item.count;
            AddItemOfType(type, item.count);
            NotifyInventoryUpdate();
            return success;
        }
        Debug.LogWarning("Trying to add item of wrong type to inventory.");
        return false;
    }
    void AddItemOfType(Type type, int count);


    public bool TryRemove(Item item)
    {
        if (item == null || item.itemType == null)
        {
            Debug.LogWarning("Try removing invalid item");
            return false;
        }
        if (item.itemType is Type type)
        {
            // only remove, if enough exists
            if (CountElements(type) >= item.count)
            {
                RemoveItemOfType(type, item.count);
                NotifyInventoryUpdate();
                return true;
            }
            Debug.LogWarning($"Failed to remove {item.count} {item.itemType.displayName}: Not enough in inventory");
            return false;
        }
        Debug.LogWarning("Trying to remove item of wrong type from inventory.");
        return false;
    }
    void RemoveItemOfType(Type type, int count);
}