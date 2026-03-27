using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class Tracker : MonoBehaviour
{
    public static event Action<Tracker> OnTrackerUpdate;

    [Header("Debugging")]
    [SerializeField] private SerializableDictionary<BlockType, int> blockAttacks;
    [SerializeField] private SerializableDictionary<BlockType, int> blocksBroken;
    [SerializeField] private SerializableDictionary<BlockWithTool, int> blocksBrokenWithTool;
    [SerializeField] private SerializableDictionary<BlockType, int> blockInteractions;
    [SerializeField] private SerializableDictionary<ItemType, int> itemsCollected;
    [SerializeField] private SerializableDictionary<CraftingRecipe, int> recipesCrafted;
    [SerializeField] private SerializableDictionary<Level, int> visitedLevels;
    [SerializeField] private SerializableDictionary<UIPage, int> visitedUIs;

    private void Awake()
    {
        blockAttacks = new SerializableDictionary<BlockType, int>();
        blocksBroken = new SerializableDictionary<BlockType, int>();
        blocksBrokenWithTool = new SerializableDictionary<BlockWithTool, int>();
        blockInteractions = new SerializableDictionary<BlockType, int>();
        itemsCollected = new SerializableDictionary<ItemType, int>();
        recipesCrafted = new SerializableDictionary<CraftingRecipe, int>();
        visitedLevels = new SerializableDictionary<Level, int>();
        visitedUIs = new SerializableDictionary<UIPage, int>();

        PlayerInteractionManager.OnAttackUpdate += HandleAttackUpdate;
        PlayerInteractionManager.OnBlockInteraction += HandleInteraction;
        BlockBreakingManager.OnBlockBroken += HandleBlockBroken;

        InventoryManager.OnItemCollected += HandleItemCollected;
        CraftingManager.OnRecipeCrafted += HandleRecipeCrafted;
        PortalManager.OnPlayerTeleport += HandlePlayerTeleport;
        UIManager.OnUpdatePage += HandlePageUpdate;
    }

    private void HandleAttackUpdate((UIPage, BlockType, Vector2Int, bool) data)
    {
        var (uiPage, blockType, targetPos, isAttacking) = data;

        if (!isAttacking)
            return;

        if (blockType == null)
            blockType = BlockTypeRepository.GetBlockByName("Air");

        if (blockAttacks.ContainsKey(blockType))
            blockAttacks[blockType]++;
        else
            blockAttacks[blockType] = 1;
        OnTrackerUpdate?.Invoke(this);
    }

    private void HandleInteraction((UIPage, BlockType, Vector2Int) data)
    {
        var (uiPage, blockType, targetPos) = data;

        if (blockType == null)
            blockType = BlockTypeRepository.GetBlockByName("Air");

        if (blockInteractions.ContainsKey(blockType))
            blockInteractions[blockType]++;
        else
            blockInteractions[blockType] = 1;
        OnTrackerUpdate?.Invoke(this);
    }

    private void HandleBlockBroken((BlockType, Vector2Int, Tool) brokenBlockInfo)
    {
        var (blockType, targetPos, tool) = brokenBlockInfo;

        if (blockType == null)
            blockType = BlockTypeRepository.GetBlockByName("Air");

        if (blocksBroken.ContainsKey(blockType))
            blocksBroken[blockType]++;
        else
            blocksBroken[blockType] = 1;

        var key = new BlockWithTool(blockType, tool);
        if (blocksBrokenWithTool.ContainsKey(key))
            blocksBrokenWithTool[key]++;
        else
            blocksBrokenWithTool[key] = 1;
        OnTrackerUpdate?.Invoke(this);
    }

    private void HandleItemCollected(Item item)
    {
        var itemType = item.itemType;
        if (itemsCollected.ContainsKey(itemType))
            itemsCollected[itemType] += item.count;
        else
            itemsCollected[itemType] = item.count;
        OnTrackerUpdate?.Invoke(this);
    }

    private void HandleRecipeCrafted(CraftingRecipe recipe)
    {
        if (recipesCrafted.ContainsKey(recipe))
            recipesCrafted[recipe]++;
        else
            recipesCrafted[recipe] = 1;
        OnTrackerUpdate?.Invoke(this);
    }

    private void HandlePlayerTeleport((Level, Vector3) data)
    {
        var (level, destination) = data;

        if (level == null)
            return;

        if (visitedLevels.ContainsKey(level))
            visitedLevels[level]++;
        else
            visitedLevels[level] = 1;
        OnTrackerUpdate?.Invoke(this);
    }

    private void HandlePageUpdate(UIPage page)
    {
        if (page == UIPage.None)
            return;

        if (visitedUIs.ContainsKey(page))
            visitedUIs[page]++;
        else
            visitedUIs[page] = 1;
        OnTrackerUpdate?.Invoke(this);
    }

    public Dictionary<BlockType, int> GetBlockAttacks()
    {
        return blockAttacks;
    }

    public Dictionary<BlockType, int> GetBlocksBroken()
    {
        return blocksBroken;
    }

    public Dictionary<BlockWithTool, int> GetBlocksBrokenWithTool()
    {
        return blocksBrokenWithTool;
    }

    public Dictionary<BlockType, int> GetBlockInteractions()
    {
        return blockInteractions;
    }

    public Dictionary<ItemType, int> GetItemsCollected()
    {
        return itemsCollected;
    }

    public Dictionary<CraftingRecipe, int> GetRecipesCrafted()
    {
        return recipesCrafted;
    }

    public Dictionary<Level, int> GetVisitedLevels()
    {
        return visitedLevels;
    }

    public string Dump()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("TrackingType,KeyName,Count");

        foreach (var kvp in blockAttacks)
            sb.AppendLine($"BlockAttack,{kvp.Key.displayName},{kvp.Value}");
        foreach (var kvp in blockInteractions)
            sb.AppendLine($"BlockInteraction,{kvp.Key.displayName},{kvp.Value}");
        foreach (var kvp in blocksBroken)
            sb.AppendLine($"BlockBroken,{kvp.Key.displayName},{kvp.Value}");
        foreach (var kvp in blocksBrokenWithTool)
        {
            string blockName = kvp.Key.blockType.displayName;
            string toolName = kvp.Key.tool == null ? "Hand" : kvp.Key.tool.displayName;
            string keyName = $"{blockName} With {toolName}";
            sb.AppendLine($"BlockBrokenWithTool,{keyName},{kvp.Value}");
        }

        foreach (var kvp in itemsCollected)
            sb.AppendLine($"ItemCollected,{kvp.Key.displayName},{kvp.Value}");
        foreach (var kvp in recipesCrafted)
            sb.AppendLine($"RecipesCrafted,{kvp.Key.result.itemType.displayName},{kvp.Value}");
        foreach (var kvp in visitedLevels)
            sb.AppendLine($"VisitedLevel,{kvp.Key.levelName},{kvp.Value}");
        foreach (var kvp in visitedUIs)
            sb.AppendLine($"VisitedUI,{kvp.Key},{kvp.Value}");

        return sb.ToString();
    }
}

[Serializable]
public struct BlockWithTool
{
    public BlockType blockType;
    public Tool tool;

    public BlockWithTool(BlockType blockType, Tool tool)
    {
        this.blockType = blockType;
        this.tool = tool;
    }
}