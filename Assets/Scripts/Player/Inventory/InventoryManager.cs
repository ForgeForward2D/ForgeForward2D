using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour, InventoryComponent<ItemType>
{
    [SerializeField] public ResourceInventory resourceInventory = new ResourceInventory();
    [SerializeField] public HotBar hotBar = new HotBar();
    [SerializeField] public CraftingManager craftingManager = new CraftingManager();

    public event Action<InventoryManager> OnInventoryUpdate;

    private void Start()
    {
        resourceInventory.Start();
        hotBar.Start();
        craftingManager.Start();
    }

    public void NotifyInventoryUpdate()
    {
        OnInventoryUpdate?.Invoke(this);
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
        NotifyInventoryUpdate();
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
        NotifyInventoryUpdate();
    }
}
