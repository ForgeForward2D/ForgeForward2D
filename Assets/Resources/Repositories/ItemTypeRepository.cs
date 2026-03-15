using UnityEngine;
using System.Collections.Generic;

public static class ItemTypeRepository
{
    private static Dictionary<string, ItemType> nameLookup;
    private static Dictionary<string, Tool> toolNameLookup;

    private static Tool defaultTool;

    private static void Initialize()
    {
        var itemTypes = Resources.LoadAll<ItemType>("ItemData");
        var toolTypes = Resources.LoadAll<Tool>("ToolData");

        nameLookup = new Dictionary<string, ItemType>();
        toolNameLookup = new Dictionary<string, Tool>();

        foreach (var item in itemTypes)
        {
            nameLookup[item.displayName] = item;
        }

        foreach (var tool in toolTypes)
        {
            nameLookup[tool.displayName] = (ItemType) tool;
            toolNameLookup[tool.displayName] = tool;
        }


        int totalItems = itemTypes.Length + toolTypes.Length;

        Debug.Log($"Initialized ItemTypeRepository with {totalItems} items ({itemTypes.Length} items, {toolTypes.Length} tools).");
    }
    
    public static ItemType GetItemByName(string name)
    {
        if (nameLookup == null)
            Initialize();
        return nameLookup.GetValueOrDefault(name);
    }

    public static Tool GetToolByName(string name)
    {
        if (toolNameLookup == null)
            Initialize();

        if (!toolNameLookup.TryGetValue(name, out Tool tool))
        {
            Debug.LogWarning($"No tool found for name {name} in HotBar.");
            return GetDefaultTool(); // Return a default "empty" tool
        }
        return tool;
    }

    public static Tool GetDefaultTool()
    {
        if (defaultTool == null)
        {
            defaultTool = ScriptableObject.CreateInstance<Tool>();
        }
        return defaultTool;
    }
}