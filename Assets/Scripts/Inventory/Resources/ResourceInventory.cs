using UnityEngine;

public class ResourceInventory : ItemContainer
{
    [SerializeField] private WorldInteractionManager worldInteractionManager;

    private void OnEnable()
    {
        if (worldInteractionManager != null)
        {
            worldInteractionManager.OnBlockBroken += HandleBlockBroken;
        }
    }

    private void OnDisable()
    {
        if (worldInteractionManager != null)
        {
            worldInteractionManager.OnBlockBroken -= HandleBlockBroken;
        }
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
        if(itemData == null) return;

        for (int i = 0; i < capacity; i++)
        {
            if (items[i] != null && items[i].Item.Id == itemId && !items[i].IsFull)
            {
                items[i].Count++;
                NotifyContentsChanged();
                return;
            }
        }

        for (int i = 0; i < capacity; i++)
        {
            if (items[i] == null)
            {
                items[i] = new InventoryItem(itemData, 1);
                NotifyContentsChanged();
                return;
            }
        }
    }
}