using System;
using UnityEngine;

public class ToolHotbar : ItemContainer
{
    [SerializeField] private Tool[] startingTools;

    public int SelectedIndex = 0;

    public event Action OnSelectionChanged;

    private void Start()
    {
        int count = Mathf.Min(capacity, startingTools.Length);

        for (int i = 0; i < count; i++)
        {
            Tool tool = startingTools[i];

            if (tool != null)
            {
                items[i] = new Item(tool, 1);
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

        ItemType item = items[SelectedIndex]?.itemType;

        if (item == null)
        {
            return ItemTypeRepository.GetDefaultTool(); // Return a default "empty" tool
        }

        if (item is Tool tool)
        {
            return tool;
        }
        return ItemTypeRepository.GetDefaultTool();
    }

    public bool TryAddTool(Tool tool)
    {
        if (tool == null) return false;

        int index =  tool.type switch
        {
            ToolType.Sword => 0,
            ToolType.Pickaxe => 1,
            ToolType.Axe => 2,
            ToolType.Shovel => 3,
            ToolType.Hammer => 4,
            _ => -1
        };
        if (index < 0 || index >= items.Length)
        {
            Debug.LogWarning($"Unsupported tool type {tool.type} cannot be added to ToolHotbar.");
            return false;
        }
        if (items[index] == null)
        {
            items[index] = new Item(tool, 1);
            NotifyContentsChanged();
            return true;
        }
        if (((Tool)items[index].itemType).tier < tool.tier)
        {
            items[index] = new Item(tool, 1);
            NotifyContentsChanged();
            return true;
        }
        Debug.LogWarning($"Slot {index} in ToolHotbar is already occupied by a tool of equal or higher tier. Cannot add tool {tool.displayName}.");
        return false;
    }
    public bool Contains(Tool tool)
    {
        if (tool == null) return false;
        foreach (var item in items)
        {
            if (item != null && item.itemType == tool)
            {
                return true;
            }
        }
        return false;
    }

    public bool CanAdd(Tool tool)
    {
        if (tool == null) return false;
        int index =  tool.type switch
        {
            ToolType.Sword => 0,
            ToolType.Pickaxe => 1,
            ToolType.Axe => 2,
            ToolType.Shovel => 3,
            ToolType.Hammer => 4,
            _ => -1
        };
        if (index < 0 || index >= items.Length)
        {
            Debug.LogWarning($"Unsupported tool type {tool.type} cannot be added to ToolHotbar.");
            return false;
        }
        if (items[index] == null)
        {
            return true;
        }
        if (((Tool)items[index].itemType).tier < tool.tier)
        {
            return true;
        }
        return false;

    }
}