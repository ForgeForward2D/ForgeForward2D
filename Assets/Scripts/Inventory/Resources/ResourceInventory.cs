using UnityEngine;
using System.Collections.Generic;

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
            Debug.Assert(amount==1, $"Attempted to add multiple ({amount}) of tool {tool.DisplayName} (ID: {tool.Id}) to ResourceInventory. Only one can be added at a time.");
            return toolHotbar.TryAddTool(tool);
        }
        
        if (CountFreeSpace(itemType) < amount)
        {
            Debug.LogWarning($"Not enough space to add {amount} of {itemType.DisplayName} (ID: {itemType.Id}) to ResourceInventory.");
            return false;
        }

        int remaining = amount;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null && items[i].Item.Id == itemType.Id)
            {
                int space = items[i].Item.MaxStackSize - items[i].Count;

                remaining -= space;
                if (remaining <= 0) {
                    items[i].Count += space + remaining;
                    return true; 
                }
    
                items[i].Count = items[i].Item.MaxStackSize;
            }
        }

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                remaining -= itemType.MaxStackSize;
                if (remaining <= 0) {
                    items[i] = new InventoryItem(itemType, itemType.MaxStackSize + remaining);
                    return true;
                }
                items[i] = new InventoryItem(itemType, itemType.MaxStackSize);
            }
        }
        Debug.LogError($"Failed to add {amount} of {itemType.DisplayName} (ID: {itemType.Id}) to ResourceInventory. This should not happen since free space was checked.");
        return false;
    }

    public bool TryRemoveItem(ItemType itemType, int amount)
    {
        if (CountItem(itemType) < amount)
            return false;
        
        int remaining = amount;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null && items[i].Item.Id == itemType.Id)
            {
                if (items[i].Count <= remaining) {
                    remaining -= items[i].Count;
                    items[i] = null;   
                    if (remaining == 0)
                        break;
                } else {
                    items[i].Count -= remaining;
                    remaining = 0;
                    break;
                }
            }
        }
        return true; 
    }

    public bool TryCraft(CraftingRecipe recipe)
    {
        if (ComputeAvailability(recipe).Exists(x => x.Item3 < x.Item2)) {
            Debug.Log("Cannot craft " + recipe.result.Item.DisplayName + ": not enough ingredients.");
            return false;
        }
        if (CountFreeSpace(recipe.result.Item) < recipe.result.Count) {
            Debug.Log("Cannot craft " + recipe.result.Item.DisplayName + ": not enough inventory space.");
            return false;
        }

        foreach (var ingredient in recipe.ingredients)
        {
            bool result = TryRemoveItem(ingredient.Item, ingredient.Count);
            Debug.Assert(result, $"Failed to remove ingredient {ingredient.Item.DisplayName} x{ingredient.Count} for crafting {recipe.result.Item.DisplayName}. This should not happen since availability was checked.");
        }

        return TryAddItem(recipe.result.Item, recipe.result.Count);
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
        Dictionary<ItemType, (int, int)> availability = new Dictionary<ItemType, (int, int)>();

        foreach (var ingredient in recipe.ingredients)
        {
            ItemType itemType = ingredient.Item;
            int requiredAmount = ingredient.Count;

            availability.Add(itemType, (requiredAmount, 0));
        }

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null && availability.ContainsKey(items[i].Item))
            {
                var (required, current) = availability[items[i].Item];
                availability[items[i].Item] = (required, current + items[i].Count);
            }
        }

        List<(ItemType, int, int)> availabilityList = new List<(ItemType, int, int)>();
        foreach (var kvp in availability)
        {
            availabilityList.Add((kvp.Key, kvp.Value.Item1, kvp.Value.Item2));
        }

        return availabilityList;
    }

    
}