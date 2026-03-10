using UnityEngine;

public class ResourceInventory : ItemContainer
{
    [SerializeField] private WorldInteractionManager worldInteractionManager;

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
        if(data.type.itemID != 0) 
        {
            AddItem(data.type.itemID);
        }
    }
    
    public void AddItem(int itemId)
    {
        ItemType itemData = ItemTypeRepository.GetItemById(itemId);
        if (itemData == null) return;

        if (TryStackExisting(itemId)) return;

        if (!TryAddNewSlot(itemData))
        {
            Debug.LogWarning($"Inventory Full! Could not add item: {itemData.DisplayName} (ID: {itemId})");
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