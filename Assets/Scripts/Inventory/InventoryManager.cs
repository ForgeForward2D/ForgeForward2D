using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private WorldInteractionManager worldInteractionManager;
    [SerializeField] private int maxSlots = 21;
    private readonly List<InventoryItem> inventory = new();

    public event Action OnInventoryChanged;

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

    private void HandleBlockBroken((BlockType type, Vector2Int pos) data)
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

        InventoryItem existingStack = inventory.Find(i => i.Item.Id == itemId && !i.IsFull);

        if (existingStack != null)
        {
            existingStack.Count++;
        }
        else if (inventory.Count < maxSlots)
        {
            inventory.Add(new InventoryItem(itemData, 1));
        }
        else
        {
            Debug.Log("Inventory Full!");
            return;
        }

        OnInventoryChanged?.Invoke();
    }

    public List<InventoryItem> GetItems() => inventory;
}