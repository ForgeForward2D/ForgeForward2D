using UnityEngine;

public class ResourceInventory : ItemContainer
{
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
                AddItem(drop.itemType.Id, drop.amount);
        }
    }

    public void AddItem(int itemId, int amount)
    {
        ItemType itemData = ItemTypeRepository.GetItemById(itemId);
        if (itemData == null) return;

        for (int i = 0; i < amount; i++)
        {
            if (TryStackExisting(itemId))
            {
                Debug.Log($"Add item {itemData.DisplayName} (ID: {itemId}) to inventory");
            }
            else if (!TryAddNewSlot(itemData))
            {
                Debug.Log($"Inventory Full! Could not add item: {itemData.DisplayName} (ID: {itemId})");
            }
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