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
            Debug.LogWarning($"No loot table found for block {data.type.displayName} (ID: {data.type.id})");
            return;
        }

        foreach (var drop in data.type.lootDrops)
        {
            if (Random.value <= drop.chance)
                TryAddItem(drop.itemType, drop.amount);
        }
    }

    public bool TryAddItem(ItemType itemType, int amount)
    {
        if (itemType == null) return false;

        if (itemType is Tool tool) {
            Debug.Assert(amount==1, $"Attempted to {amount} tools {tool.DisplayName} (ID: {tool.Id}) to ResourceInventory. Only one can be added at a time.");
            if (toolHotbar == null) {
                Debug.LogError($"ToolHotbar reference is missing in ResourceInventory.");
                return false;
            }
            return toolHotbar.TryAddTool(tool);
        }

        int remaining = amount;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null && items[i].Item.Id == itemType.Id)
            {
                int space = items[i].Item.MaxStackSize - items[i].Count;

                if (remaining <= space) {
                    items[i].Count += remaining;
                    NotifyContentsChanged();
                    return true;
                }

                items[i].Count = items[i].Item.MaxStackSize;
                remaining -= space;
            }
        }

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                remaining -= itemType.MaxStackSize;
                if (remaining <= 0) {
                    items[i] = new InventoryItem(itemType, itemType.MaxStackSize + remaining);
                    NotifyContentsChanged();
                    return true;
                }
                items[i] = new InventoryItem(itemType, itemType.MaxStackSize);
            }
        }
        Debug.Log($"Failed to add {amount} of {itemType.DisplayName} (ID: {itemType.Id}) to ResourceInventory. Not enough space.");
        return false;
    }

    public bool TryRemoveItem(ItemType itemType, int amount)
    {
        if (CountItem(itemType) < amount)
            return false;

        if (itemType is Tool tool) {
            Debug.LogError("Attempting to remove a tool " + tool.DisplayName + " (ID: "+tool.Id+") but this should not happen");
            return false;
        }

        int remaining = amount;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null && items[i].Item.Id == itemType.Id)
            {
                if (items[i].Count <= remaining) {
                    remaining -= items[i].Count;
                    items[i] = null;
                    if (remaining == 0) {
                        NotifyContentsChanged();
                        return true;
                    }

                } else {
                    items[i].Count -= remaining;
                    remaining = 0;
                    NotifyContentsChanged();
                    return true;
                }
            }
        }
        Debug.LogError($"Failed to remove {amount} of {itemType.DisplayName} (ID: {itemType.Id}) from ResourceInventory. This should not happen since availability was checked.");
        return false;
    }

    public bool TryCraft(CraftingRecipe recipe)
    {
        // Checks if any ingredient has more required (Item2) than available (Item3)
        if (ComputeAvailability(recipe).Exists(x => x.Item3 < x.Item2)) {
            Debug.Log("Cannot craft " + recipe.result.Item.DisplayName + ": not enough ingredients.");
            return false;
        }
        // Check if there is not enough free space for the result item.
        if (CountFreeSpace(recipe.result.Item) < recipe.result.Count) {
            Debug.Log("Cannot craft " + recipe.result.Item.DisplayName + ": not enough inventory space.");
            return false;
        }

        foreach (var ingredient in recipe.ingredients)
        {
            bool result = TryRemoveItem(ingredient.Item, ingredient.Count);
            Debug.Assert(result, $"Failed to remove ingredient {ingredient.Item.DisplayName} x{ingredient.Count} for crafting {recipe.result.Item.DisplayName}. This should not happen since availability was checked.");
        }

        bool result = TryAddItem(recipe.result.Item, recipe.result.Count);
        Debug.Assert(result, $"Failed to add crafted item {recipe.result.Item.DisplayName} x{recipe.result.Count} to inventory. This should not happen since free space was checked.");
        return result;
    }

    public int CountFreeSpace(ItemType itemType)
    {
        if (itemType == null) return 0;
        if (itemType is Tool) {
            return toolHotbar.CanAdd((Tool)itemType) ? 1 : 0;
        }

        int freeSpace = 0;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                freeSpace += itemType.MaxStackSize;
            }
            else if (items[i].Item.Id == itemType.Id)
            {
                freeSpace += itemType.MaxStackSize - items[i].Count;
            }
        }
        return freeSpace;
    }

    public int CountItem(ItemType itemType)
    {
        if (itemType == null) return 0;
        if (itemType is Tool) {
            return toolHotbar.Contains((Tool)itemType) ? 1 : 0;
        }
        int totalCount = 0;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null && items[i].Item.Id == itemType.Id)
            {
                totalCount += items[i].Count;
            }
        }
        return totalCount;
    }

    public List<(ItemType, int, int)> ComputeAvailability(CraftingRecipe recipe)
    {

        List<(ItemType, int, int)> availabilityList = recipe.ingredients.Select(ingredient => {
            ItemType itemType = ingredient.Item;
            int requiredAmount = ingredient.Count;
            int availableAmount = CountItem(itemType);
            return (itemType, requiredAmount, availableAmount);
        }).ToList();

        return availabilityList;
    }


}