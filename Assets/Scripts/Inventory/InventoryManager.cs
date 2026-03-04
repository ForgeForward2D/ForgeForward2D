using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    [SerializeField] private List<InventoryItem> inventory = new List<InventoryItem>();

    public Action OnInventoryChanged;

    private void OnEnable()
    {
        if (playerController != null)
        {
            playerController.OnBlockBroken += AddBlockToInventory;
        }
    }

    private void OnDisable()
    {
        if (playerController != null)
        {
            playerController.OnBlockBroken -= AddBlockToInventory;
        }
    }

    private void AddBlockToInventory((BlockType block, Vector2Int position) brokenBlockInfo)
    {
        BlockType block = brokenBlockInfo.block;

        ItemType itemToAdd = ItemTypeRepository.GetItemById(block.itemID);

        if (itemToAdd == null)
        {
            Debug.LogWarning($"No Item found for ID {block.itemID}!");
            return;
        }

        InventoryItem existingStack = inventory.Find(i =>
            i.Item.id == itemToAdd.id &&
            i.Count < itemToAdd.maxStackSize
        );

        if (existingStack != null)
        {
            existingStack.Count++;
        }
        else
        {
            if (inventory.Count < 20)
            {
                inventory.Add(new InventoryItem(itemToAdd, 1));
            }
            else
            {
                Debug.Log("Inventory Full!");
                return;
            }
        }

        OnInventoryChanged?.Invoke();
    }

    public List<InventoryItem> GetInventoryItems() => inventory;
}