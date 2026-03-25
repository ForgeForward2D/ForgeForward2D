using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ResourceInventory : InventoryComponent<ItemType>
{
    public static event Action<ResourceInventory> OnResourceInventoryUpdate;

    [SerializeField] int capacity = 21;

    [Header("Debugging")]
    [SerializeField] private List<Item> items;

    public void Start()
    {
        items = Enumerable.Repeat<Item>(null, capacity).ToList();

        ResourceInventoryUI.RequestRefresh += HandleRequestRefresh;
        BlockBreakingManager.OnBlockBroken += HandleBlockBroken;
        MobController.OnMobStealItems += HandleMobSteal;
    }

    private void HandleRequestRefresh()
    {
        OnResourceInventoryUpdate?.Invoke(this);
    }

    private List<Item> HandleMobSteal(int count, MobType mob)
    {
        List<int> occupiedSlots = new();
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] != null && items[i].itemType != null)
                occupiedSlots.Add(i);
        }

        List<Item> stolenItems = new();
        if (occupiedSlots.Count == 0) return stolenItems;

        int removed = 0;
        while (removed < count && occupiedSlots.Count > 0)
        {
            int pick = UnityEngine.Random.Range(0, occupiedSlots.Count);
            int slotIndex = occupiedSlots[pick];

            ItemType stolenType = items[slotIndex].itemType;
            Debug.Assert(items[slotIndex].count > 0, $"Trying to steal from slot {slotIndex} which has no items. This should not happen since we check for occupied slots.");
            items[slotIndex].count--;
            if (items[slotIndex].count == 0)
            {
                items[slotIndex] = null;
                occupiedSlots.RemoveAt(pick);
            }

            Item existing = stolenItems.Find(item => item.itemType == stolenType);
            if (existing != null)
                existing.count++;
            else
                stolenItems.Add(new Item(stolenType, 1));

            removed++;
        }

        Debug.Log($"{mob.displayName} stole {removed} items from the player: {string.Join(", ", stolenItems.Select(item => $"{item.count} {item.itemType.displayName}"))}");

        return stolenItems;
    }

    private void HandleBlockBroken((BlockType type, Vector2Int position, Tool tool) data)
    {
        if (data.type == null)
        {
            Debug.LogWarning("HandleBlockBroken invoked with null block");
        }
        if (data.type.lootDrops == null || data.type.lootDrops.Length == 0)
        {
            Debug.LogWarning($"No loot table found for block {data.type.displayName}");
            return;
        }

        foreach (var drop in data.type.lootDrops)
        {
            if (drop.chance == 1f || UnityEngine.Random.value <= drop.chance)
            {
                AddItemOfType(drop.itemType, drop.amount);
            }
        }
    }

    public List<Item> GetItems()
    {
        return items;
    }

    public int CountElements(ItemType itemType)
    {
        if (itemType == null) return 0;
        int count = 0;
        foreach (var item in items)
        {
            if (item != null && item.itemType == itemType)
            {
                count += item.count;
            }
        }
        return count;
    }

    public int CountFreeSpace(ItemType itemType)
    {
        if (itemType == null) return 0;

        int freeSpace = 0;
        foreach (var item in items)
        {
            if (item == null)
            {
                freeSpace += itemType.maxStackSize;
            }
            else if (item.itemType == itemType)
            {
                freeSpace += itemType.maxStackSize - item.count;
            }
        }
        return freeSpace;
    }

    public void AddItemOfType(ItemType itemType, int amount)
    {
        Debug.Assert(itemType != null, "Try adding null item. This was checked previously");

        int remaining = amount;
        foreach (var item in items)
        {
            if (item != null && item.itemType == itemType)
            {
                int space = item.itemType.maxStackSize - item.count;

                if (remaining <= space)
                {
                    item.count += remaining;
                    OnResourceInventoryUpdate?.Invoke(this);
                    InventoryManager.NotifyItemCollected(new Item(itemType, amount));
                    return;
                }

                item.count = item.itemType.maxStackSize;
                remaining -= space;
            }
        }

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null || items[i].itemType == null)
            {
                Debug.Log($"Found empty spot to add {remaining} {itemType.displayName}");
                if (remaining <= itemType.maxStackSize)
                {
                    items[i] = new Item(itemType, remaining);
                    OnResourceInventoryUpdate?.Invoke(this);
                    InventoryManager.NotifyItemCollected(new Item(itemType, amount));
                    return;
                }
                items[i] = new Item(itemType, itemType.maxStackSize);
                remaining -= itemType.maxStackSize;
            }
        }
        Debug.LogWarning($"Failed to add {amount} of {itemType.displayName} to ResourceInventory. Not enough space.");
        OnResourceInventoryUpdate?.Invoke(this);
        InventoryManager.NotifyItemCollected(new Item(itemType, amount - remaining));
    }

    public void RemoveItemOfType(ItemType itemType, int amount)
    {
        Debug.Assert(itemType != null, "Try adding null item. This was checked previously");

        int remaining = amount;
        // Loop in reverse to avoid fragmentation
        for (int i = items.Count - 1; i >= 0; i--)
        {
            if (items[i] != null && items[i].itemType == itemType)
            {
                if (items[i].count <= remaining)
                {
                    remaining -= items[i].count;
                    items[i] = null;
                    if (remaining == 0)
                    {
                        OnResourceInventoryUpdate?.Invoke(this);
                        return;
                    }

                }
                else
                {
                    items[i].count -= remaining;
                    remaining = 0;
                    OnResourceInventoryUpdate?.Invoke(this);
                    return;
                }
            }
        }
        Debug.LogError($"Failed to remove {amount} of {itemType.displayName} from ResourceInventory. This should not happen since availability was checked.");
        OnResourceInventoryUpdate?.Invoke(this);
    }


}