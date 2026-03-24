using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour, InventoryComponent<ItemType>
{
    public event Action<InventoryManager> OnInventoryUpdate;
    public static event Action<Item> OnItemCollected;

    [Header("Debugging")]
    [SerializeField] public ResourceInventory resourceInventory = new ResourceInventory();
    [SerializeField] public HotBar hotBar = new HotBar();
    [SerializeField] public CraftingManager craftingTableManager = new CraftingManager();
    [SerializeField] public CraftingManager anvilManager = new CraftingManager();

    private void Start()
    {
        resourceInventory.Start();
        hotBar.Start();
        craftingTableManager.Start();
        anvilManager.Start();

        ResourceInventory.OnResourceInventoryUpdate += NotifyInventoryUpdate;
        HotBar.OnHotBarUpdate += NotifyInventoryUpdate;
    }

    private void NotifyInventoryUpdate(object _)
    {
        OnInventoryUpdate?.Invoke(this);
    }

    public static void NotifyItemCollected(Item item)
    {
        OnItemCollected?.Invoke(item);
    }

    public int CountElements(ItemType type)
    {
        if (type is Tool tool)
        {
            return hotBar.CountElements(tool);
        }
        return resourceInventory.CountElements(type);
    }

    public int CountFreeSpace(ItemType type)
    {
        if (type is Tool tool)
        {
            return hotBar.CountFreeSpace(tool);
        }
        return resourceInventory.CountFreeSpace(type);
    }

    public void AddItemOfType(ItemType type, int count)
    {
        if (type is Tool tool)
        {
            hotBar.AddItemOfType(tool, count);
        }
        else
        {
            resourceInventory.AddItemOfType(type, count);
        }
    }

    public void RemoveItemOfType(ItemType type, int count)
    {
        if (type is Tool tool)
        {
            hotBar.RemoveItemOfType(tool, count);
        }
        else
        {
            resourceInventory.RemoveItemOfType(type, count);
        }
    }
}
