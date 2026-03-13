using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ResourceInventory : ItemContainer
{
    [SerializeField] private ToolHotbar toolHotbar;

    private void OnEnable()
    {
        BlockBreakingManager.OnBlockBroken += HandleBlockBroken;
    }

    private void OnDisable()
    {
        BlockBreakingManager.OnBlockBroken -= HandleBlockBroken;
    }

    private void HandleBlockBroken((BlockType type, Vector2Int position) data)
    {
        if (data.type.lootDrops == null || data.type.lootDrops.Length == 0)
        {
            Debug.LogWarning($"No loot table found for block {data.type.displayName}");
            return;
        }

        foreach (var drop in data.type.lootDrops)
        {
            if (Random.value <= drop.chance)
            {
                TryAddItem(drop.itemType, drop.amount);
            }
        }
    }

    public bool TryAddItem(ItemType itemType, int amount)
    {
        if (itemType == null) return false;

        if (itemType is Tool tool)
        {
            Debug.Assert(amount == 1, $"Attempted to {amount} tools {tool.displayName} to ResourceInventory. Only one can be added at a time.");
            if (toolHotbar == null)
            {
                Debug.LogError($"ToolHotbar reference is missing in ResourceInventory.");
                return false;
            }
            return toolHotbar.TryAddTool(tool);
        }

        int remaining = amount;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null && items[i].itemType == itemType)
            {
                int space = items[i].itemType.maxStackSize - items[i].count;

                if (remaining <= space)
                {
                    items[i].count += remaining;
                    NotifyContentsChanged();
                    return true;
                }

                items[i].count = items[i].itemType.maxStackSize;
                remaining -= space;
            }
        }

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                remaining -= itemType.maxStackSize;
                if (remaining <= 0)
                {
                    items[i] = new Item(itemType, itemType.maxStackSize + remaining);
                    NotifyContentsChanged();
                    return true;
                }
                items[i] = new Item(itemType, itemType.maxStackSize);
            }
        }
        Debug.Log($"Failed to add {amount} of {itemType.displayName} to ResourceInventory. Not enough space.");
        return false;
    }

    public bool TryRemoveItem(ItemType itemType, int amount)
    {
        if (CountItem(itemType) < amount)
            return false;

        if (itemType is Tool tool)
        {
            Debug.LogError($"Attempting to remove a tool {tool.displayName} but this should not happen");
            return false;
        }

        int remaining = amount;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null && items[i].itemType == itemType)
            {
                if (items[i].count <= remaining)
                {
                    remaining -= items[i].count;
                    items[i] = null;
                    if (remaining == 0)
                    {
                        NotifyContentsChanged();
                        return true;
                    }

                }
                else
                {
                    items[i].count -= remaining;
                    remaining = 0;
                    NotifyContentsChanged();
                    return true;
                }
            }
        }
        Debug.LogError($"Failed to remove {amount} of {itemType.displayName} from ResourceInventory. This should not happen since availability was checked.");
        return false;
    }

    public bool TryCraft(CraftingRecipe recipe)
    {
        // Checks if any ingredient has more required (Item2) than available (Item3)
        if (ComputeAvailability(recipe).Exists(x => x.Item3 < x.Item2))
        {
            Debug.Log($"Cannot craft {recipe.result.itemType.displayName}: not enough ingredients.");
            return false;
        }
        // Check if there is not enough free space for the result item.
        if (CountFreeSpace(recipe.result.itemType) < recipe.result.count)
        {
            Debug.Log($"Cannot craft {recipe.result.itemType.displayName}: not enough inventory space.");
            return false;
        }

        foreach (var ingredient in recipe.ingredients)
        {
            bool removeSuccess = TryRemoveItem(ingredient.itemType, ingredient.count);
            Debug.Assert(removeSuccess, $"Failed to remove ingredient {ingredient.itemType.displayName} x{ingredient.count} for crafting {recipe.result.itemType.displayName}. This should not happen since availability was checked.");
        }

        bool addSuccess = TryAddItem(recipe.result.itemType, recipe.result.count);
        Debug.Assert(addSuccess, $"Failed to add crafted item {recipe.result.itemType.displayName} x{recipe.result.count} to inventory. This should not happen since free space was checked.");
        return addSuccess;
    }

    public int CountFreeSpace(ItemType itemType)
    {
        if (itemType == null) return 0;
        if (itemType is Tool)
        {
            return toolHotbar.CanAdd((Tool)itemType) ? 1 : 0;
        }

        int freeSpace = 0;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                freeSpace += itemType.maxStackSize;
            }
            else if (items[i].itemType == itemType)
            {
                freeSpace += itemType.maxStackSize - items[i].count;
            }
        }
        return freeSpace;
    }

    public int CountItem(ItemType itemType)
    {
        if (itemType == null) return 0;
        if (itemType is Tool)
        {
            return toolHotbar.Contains((Tool)itemType) ? 1 : 0;
        }
        int totalCount = 0;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null && items[i].itemType == itemType)
            {
                totalCount += items[i].count;
            }
        }
        return totalCount;
    }

    public List<(ItemType, int, int)> ComputeAvailability(CraftingRecipe recipe)
    {

        List<(ItemType, int, int)> availabilityList = recipe.ingredients.Select(ingredient =>
        {
            ItemType itemType = ingredient.itemType;
            int requiredAmount = ingredient.count;
            int availableAmount = CountItem(itemType);
            return (itemType, requiredAmount, availableAmount);
        }).ToList();

        return availabilityList;
    }


}