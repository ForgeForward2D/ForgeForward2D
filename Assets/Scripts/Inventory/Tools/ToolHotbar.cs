using System;
using UnityEngine;

public class ToolHotbar : ItemContainer
{
    [SerializeField] private int[] startingItemIds;

    public int SelectedIndex = 0;

    public event Action OnSelectionChanged;

    private void Start()
    {
        int count = Mathf.Min(capacity, startingItemIds.Length);

        for (int i = 0; i < count; i++)
        {
            int itemId = startingItemIds[i];

            if (itemId != 0)
            {
                ItemType data = ItemTypeRepository.GetItemById(itemId);
                if (data != null)
                {
                    items[i] = new InventoryItem(data, 1);
                }
                else
                {
                    Debug.LogWarning($"No item found for ID {itemId} in ToolHotbar starting items.");
                }
            }
        }
        OnSelectionChanged?.Invoke();
        NotifyContentsChanged();
    }

    public void ChangeSelectedSlot(int index)
    {
        if (index >= 0 && index < capacity)
        {
            SelectedIndex = index;
            OnSelectionChanged?.Invoke();
        }
    }

    public Tool GetSelectedTool()
    {

        ItemType item = items[SelectedIndex]?.Item;

        if (item == null)
        {
            return ItemTypeRepository.GetDefaultTool(); // Return a default "empty" tool
        }

        if (item is Tool tool)
        {
            return tool;
        }
        return ItemTypeRepository.GetToolById(item.Id);
    }

    public bool TryAddTool(Tool tool)
    {
        int index =  tool.type switch
        {
            ToolType.Sword => 0,
            ToolType.Pickaxe => 1,
            ToolType.Axe => 2,
            ToolType.Shovel => 3,
            ToolType.Hammer => 4,
            _ => -1
        };
        if (index == -1)
        {
            Debug.LogWarning("Unsupported tool type " + tool.type.ToString() + " cannot be added to ToolHotbar.");
            return false;
        }
        if (items[index] == null)
        {
            items[index] = new InventoryItem(tool, 1);
            NotifyContentsChanged();
            return true;
        }
        if (((Tool)items[index].Item).tier < tool.tier)
        {
            items[index] = new InventoryItem(tool, 1);
            NotifyContentsChanged();
            return true;
        }
        Debug.LogWarning("Slot "+index+" in ToolHotbar is already occupied by a tool of equal or higher tier. Cannot add tool "+tool.DisplayName+" (ID: "+tool.Id+").");
        return false;
    }
    public bool Contains(Tool tool)
    {
        foreach (var item in items)
        {
            if (item != null && item.Item.Id == tool.Id)
            {
                return true;
            }
        }
        return false;
    }

    public bool CanAdd(Tool tool)
    {
        int index =  tool.type switch
        {
            ToolType.Sword => 0,
            ToolType.Pickaxe => 1,
            ToolType.Axe => 2,
            ToolType.Shovel => 3,
            ToolType.Hammer => 4,
            _ => -1
        };
        if (index == -1)
        {
            Debug.LogWarning("Unsupported tool type " + tool.type.ToString() + " cannot be added to ToolHotbar.");
            return false;
        }
        if (items[index] == null)
        {
            return true;
        }
        if (((Tool)items[index].Item).tier < tool.tier)
        {
            return true;
        }
        return false;

    }
}