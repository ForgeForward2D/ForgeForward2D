using UnityEngine;
using System.Collections.Generic;

public static class ItemTypeRepository
{
    private static Dictionary<int, ItemType> idLookup;
    private static Dictionary<int, Tool> toolIdLookup;

    private static Tool defaultTool;

    private static void Initialize()
    {
        var itemTypes = Resources.LoadAll<ItemType>("ItemData");
        var toolTypes = Resources.LoadAll<Tool>("ToolData");

        idLookup = new Dictionary<int, ItemType>();
        toolIdLookup = new Dictionary<int, Tool>();

        foreach (var item in itemTypes)
        {
            idLookup[item.Id] = item;
        }

        foreach (var tool in toolTypes)
        {
            idLookup[tool.Id] = (ItemType)tool;
            toolIdLookup[tool.Id] = tool;
        }


        int totalItems = itemTypes.Length + toolTypes.Length;

        Debug.Log("Initialized ItemTypeRepository with " + totalItems + " items (" + itemTypes.Length + " items, " + toolTypes.Length + " tools).");
    }

    public static ItemType GetItemById(int id)
    {
        if (idLookup == null)
            Initialize();
        // return idLookup[id];
        return idLookup.GetValueOrDefault(id);
    }

    public static Tool GetToolById(int id)
    {
        if (toolIdLookup == null)
            Initialize();

        if (!toolIdLookup.TryGetValue(id, out Tool tool))
        {
            Debug.LogWarning($"No tool found for ID {id} in ToolHotbar.");
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