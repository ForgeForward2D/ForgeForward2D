using System.Collections.Generic;
using UnityEngine;

public class ResourceInventory : ItemContainer
{
    [SerializeField] private WorldInteractionManager worldInteractionManager;
    [SerializeField] private BlockLootTable[] lootTables;

    private Dictionary<BlockType, LootDrop[]> lootLookup;

    private void Start()
    {
        lootLookup = new Dictionary<BlockType, LootDrop[]>();
        foreach (var entry in lootTables)
        {
            if (entry.blockType != null)
                lootLookup[entry.blockType] = entry.drops;
        }
    }

    private void OnEnable()
    {
        if (worldInteractionManager != null)
            worldInteractionManager.OnBlockBroken += HandleBlockBroken;
    }

    private void OnDisable()
    {
        if (worldInteractionManager != null)
            worldInteractionManager.OnBlockBroken -= HandleBlockBroken;
    }

    private void HandleBlockBroken((BlockType type, Vector2Int position) data)
    {
        if (lootLookup.TryGetValue(data.type, out var drops))
        {
            foreach (var drop in drops)
            {
                if (Random.value <= drop.chance)
                    AddItem(drop.itemType.Id);
            }
        }
        else
        {
            Debug.LogWarning($"No loot table found for block {data.type.displayName} (ID: {data.type.id})");
        }
        // if (data.type.itemID != 0)
        // {
        //     AddItem(data.type.itemID);
        // }
    }

    public void AddItem(int itemId)
    {
        ItemType itemData = ItemTypeRepository.GetItemById(itemId);
        if (itemData == null) return;

        if (TryStackExisting(itemId))
        {
            Debug.Log($"Add item {itemData.DisplayName} (ID: {itemId}) to inventory");
            return;
        }

        if (!TryAddNewSlot(itemData))
        {
            Debug.Log($"Inventory Full! Could not add item: {itemData.DisplayName} (ID: {itemId})");
        }
    }

    private bool TryStackExisting(int itemId)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] != null && items[i].Item.Id == itemId && !items[i].IsFull)
            {
                items[i].Count++;
                NotifyContentsChanged();
                return true;
            }
        }
        return false;
    }

    private bool TryAddNewSlot(ItemType itemData)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                items[i] = new InventoryItem(itemData, 1);
                NotifyContentsChanged();
                return true;
            }
        }
        return false;
    }
}