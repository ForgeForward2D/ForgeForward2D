using UnityEngine;
using System.Collections.Generic;

public static class ToolRepository
{
    private static Dictionary<(ToolType, ToolTier), Tool> lookup;

    private static void Initialize()
    {
        var tools = Resources.LoadAll<Tool>("ToolData");

        lookup = new Dictionary<(ToolType, ToolTier), Tool>();

        foreach (var tool in tools)
            lookup[(tool.type, tool.tier)] = tool;

        Debug.Log($"Initialized ToolRepository with {tools.Length} tools.");
    }

    public static Tool GetTool(ToolType type, ToolTier tier)
    {
        if (lookup == null)
            Initialize();

        lookup.TryGetValue((type, tier), out Tool tool);
        return tool;
    }

    public static Tool GetLowestTierTool(ToolType type)
    {
        if (lookup == null)
            Initialize();

        Tool result = null;
        foreach (var kvp in lookup)
        {
            if (kvp.Key.Item1 == type && (result == null || kvp.Key.Item2 < result.tier))
                result = kvp.Value;
        }
        return result;
    }
}
