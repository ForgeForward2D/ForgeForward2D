using System;
using System.Collections.Generic;

using UnityEngine;

[Serializable]
public class HotBar : InventoryComponent<Tool>
{
    public static event Action<HotBar> OnHotBarUpdate;

    [SerializeField] private List<Tool> tools;

    [Header("Debugging")]
    [SerializeField] private List<ToolType> toolTypes;
    [SerializeField] private int selectedIndex = 0;

    // Initialize hot bar
    public void Start()
    {
        HotBarUI.RequestRefresh += HandleRequestRefresh;
        InputManager.OnHotBarSelected += SetSelectedSlot;
        InputManager.OnHotBarScroll += ChangeSelectedSlot;

        toolTypes = new List<ToolType>((ToolType[])Enum.GetValues(typeof(ToolType)));
        toolTypes.Remove(ToolType.None);

        int startingToolCount = tools == null ? 0 : tools.Count;
        List<Tool> newTools = new List<Tool>();
        for (int i = 0; i < toolTypes.Count; i++)
        {
            if (i < startingToolCount)
            {
                newTools.Add(tools[i]);
            }
            else
            {
                newTools.Add(null);
            }
        }
        tools = newTools;
        Debug.Log($"Set {tools.Count} hot bar tools");
    }

    private void HandleRequestRefresh()
    {
        OnHotBarUpdate?.Invoke(this);
    }

    public void SetSelectedSlot((UIPage, int) data)
    {
        var (uiPage, index) = data;
        if (index >= 0 && index < tools.Count)
        {
            selectedIndex = index;
            OnHotBarUpdate?.Invoke(this);
        }
    }
    public void ChangeSelectedSlot((UIPage, int) data)
    {
        var (uiPage, delta) = data;
        int index = selectedIndex + delta;
        index = (index % tools.Count + tools.Count) % tools.Count;

        SetSelectedSlot((uiPage, index));
    }

    public List<Tool> GetCurrentTools()
    {
        return tools;
    }

    public int GetSelectedIndex()
    {
        return selectedIndex;
    }

    public Tool GetSelectedTool()
    {
        if (tools == null || selectedIndex < 0 || selectedIndex >= tools.Count)
            return null;
        return tools[selectedIndex];
    }

    private int ToolTypeToIndex(ToolType toolType)
    {
        for (int i = 0; i < toolTypes.Count; i++)
        {
            if (toolTypes[i] == toolType)
            {
                return i;
            }
        }
        return -1;
    }

    public int CountElements(Tool tool)
    {
        if (tool == null) return 0;
        int index = ToolTypeToIndex(tool.type);
        if (index < 0 || index >= tools.Count) return 0;
        bool contains = tools[index] == tool;
        return contains ? 1 : 0;
    }

    public int CountFreeSpace(Tool tool)
    {
        if (tool == null) return 0;
        int index = ToolTypeToIndex(tool.type);
        if (index < 0 || index >= tools.Count) return 0;
        bool replaceable = tools[index] == null || tools[index].tier < tool.tier;
        return replaceable ? 1 : 0;
    }

    public void AddItemOfType(Tool tool, int count)
    {
        Debug.Assert(tool != null, "Try adding null tool. This was checked previously");
        Debug.Assert(count == 1, "Tools should always have amount of 1");

        int index = ToolTypeToIndex(tool.type);
        if (index < 0 || index >= tools.Count)
        {
            Debug.LogWarning($"Can not add {tool.displayName}, because its index is invalid (0<={index}<{tools.Count})");
            return;
        }

        if (tools[index] != null && tools[index].tier >= tool.tier)
        {
            Debug.LogWarning($"Can not add {tool.displayName}, because there is no free space ({tools[index].displayName} is there)");
            return;
        }

        tools[index] = tool;
        OnHotBarUpdate?.Invoke(this);
        InventoryManager.NotifyItemCollected(new Item(tool, 1));
    }

    public void RemoveItemOfType(Tool tool, int count)
    {
        Debug.Assert(tool != null, "Try removing null tool. This was checked previously");
        Debug.Assert(count == 1, "Tools should always have amount of 1");

        int index = ToolTypeToIndex(tool.type);
        if (index < 0 || index >= tools.Count)
        {
            Debug.LogError($"Can not remove {tool.displayName}, because its index is invalid (0<={index}<{tools.Count})");
            return;
        }

        if (tools[index] != tool)
        {
            Debug.LogError($"Can not remove {tool.displayName}, because its not in the hot bar");
            return;
        }

        tools[index] = null;
        OnHotBarUpdate?.Invoke(this);
    }

}