using System;
using System.Collections.Generic;
using UnityEngine;

public class Tracker : MonoBehaviour
{
    public static Action<Tracker> OnTrackerUpdate;

    [Header("Debugging")]
    [SerializeField] private SerializableDictionary<BlockType, int> blockAttacks;
    [SerializeField] private SerializableDictionary<BlockType, int> blocksBroken;
    [SerializeField] private SerializableDictionary<BlockWithTool, int> blocksBrokenWithTool;
    [SerializeField] private SerializableDictionary<BlockType, int> blockInteractions;
    [SerializeField] private SerializableDictionary<ItemType, int> itemsCollected;
    [SerializeField] private SerializableDictionary<CraftingRecipe, int> recipesCrafted;
    [SerializeField] private SerializableDictionary<Level, int> visitedLevels;
    [SerializeField] private InventoryManager lastInventoryState;

    private void Awake()
    {
        blockAttacks = new SerializableDictionary<BlockType, int>();
        blocksBroken = new SerializableDictionary<BlockType, int>();
        blocksBrokenWithTool = new SerializableDictionary<BlockWithTool, int>();
        blockInteractions = new SerializableDictionary<BlockType, int>();
        itemsCollected = new SerializableDictionary<ItemType, int>();
        recipesCrafted = new SerializableDictionary<CraftingRecipe, int>();
        visitedLevels = new SerializableDictionary<Level, int>();

        PlayerInteractionManager.OnAttackUpdate += HandleAttackUpdate;
        PlayerInteractionManager.OnInteraction += HandleInteraction;
        BlockBreakingManager.OnBlockBroken += HandleBlockBroken;

        InventoryManager.OnItemCollected += HandleItemCollected;
        CraftingManager.OnRecipeCrafted += HandleRecipeCrafted;
    }
       
    private void HandleAttackUpdate((UIPage, BlockType, Vector2Int, bool) data)
    {
        var (uiPage, blockType, targetPos, attackStatus) = data;

        if (!attackStatus)
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