using UnityEngine;
using System;

public class BlockBreakingManager : MonoBehaviour
{
    public static event Action<(BlockType, Vector2Int)> OnBlockBroken;

    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private TileMapManager tileMapManager;
    [SerializeField] private ToolHotbar toolHotbar;

    public Tool currentTool;

    private bool isBreaking = false;
    private float breakProgress = 0f;
    private Vector2Int currentTargetPos;
    private BlockType currentTargetBlock;
    private DateTime lastProgressUpdateTime;

    void OnEnable()
    {
        lastProgressUpdateTime = DateTime.Now;
        toolHotbar.OnSelectionChanged += HandleSelectionChanged;
        TileMapManager.OnBlockChanged += HandleBlockChanged;
    }

    void OnDisable()
    {
        toolHotbar.OnSelectionChanged -= HandleSelectionChanged;
        TileMapManager.OnBlockChanged -= HandleBlockChanged;
    }

    void Update()
    {
        if (playerController.IsHoldingAttack)
        {
            HandleBreaking();
        }
        else if (isBreaking)
        {
            CancelBreaking();
        }
    }

    private void HandleSelectionChanged()
    {
        currentTool = toolHotbar.GetSelectedTool();
    }

    private void HandleBlockChanged((BlockType, Vector2Int) blockInfo)
    {
        // If the block that changed is the one we're currently breaking, update our reference
        if (blockInfo.Item2 == currentTargetPos)
        {
            currentTargetBlock = blockInfo.Item1;
        }
    }

    private void HandleBreaking()
    {
        Vector2Int targetPos = playerController.GetTargettingBlock();
        // If we moved to a new block, reset progress
        if (targetPos != currentTargetPos)
        {
            tileMapManager.UpdateBlockBreakingProgress(currentTargetPos, 0);
            currentTargetPos = targetPos;
            currentTargetBlock = tileMapManager.GetBlockTypeAtPosition(currentTargetPos);
            breakProgress = 0f;
        }


        float efficiency = CalculateEfficiency();

        // Block not breakable with current tool
        if (efficiency <= 0f)
        {
            if (isBreaking) CancelBreaking();
            return;
        }

        isBreaking = true;
        playerController.SetBreakingAnimation(true);


        // Progress formula: 
        float deltaProgress = Time.deltaTime * gameConfig.player_breaking_speed * efficiency / currentTargetBlock.hardness;
        breakProgress += deltaProgress;

        if (breakProgress >= 1f)
        {
            TriggerBreak();
            return;
        }

        int previousStage = Mathf.CeilToInt((breakProgress - deltaProgress) * 10f);
        int stage = Mathf.CeilToInt(breakProgress * 10f);

        float timeSinceLastUpdate = (float)(DateTime.Now - lastProgressUpdateTime).TotalSeconds;        

        if (stage != previousStage && timeSinceLastUpdate >= gameConfig.block_breaking_animation_min_update_interval) {

            lastProgressUpdateTime = DateTime.Now;
            tileMapManager.UpdateBlockBreakingProgress(currentTargetPos, stage);
        }
    }

    public float CalculateEfficiency()
    {
        if (currentTargetBlock == null || !currentTargetBlock.breakable) return 0.0f;

        // If the block doesn't require a specific tool, every tool is efficient
        if (currentTargetBlock.toolType == ToolType.None)
        {
            return currentTool.efficiency;
        }

        // Wrong tool type: Same as no tool
        if (currentTargetBlock.toolType != currentTool.type)
        {
            return currentTargetBlock.minimumToolTier == ToolTier.None ? 1.0f : 0.0f;
        }

        if (currentTool.tier < currentTargetBlock.minimumToolTier)
        {
            // Tool is not good enough to break the block
            return 0.0f;
        }

        return currentTool.efficiency;
    }

    private void TriggerBreak( )
    {
        BlockType replacementBlock = BlockTypeRepository.GetBlockById(currentTargetBlock.replacementBlockId);
        OnBlockBroken?.Invoke((currentTargetBlock, currentTargetPos));
        tileMapManager.DrawBlock(replacementBlock, currentTargetPos);
        CancelBreaking();
    }

    private void CancelBreaking()
    {
        tileMapManager.UpdateBlockBreakingProgress(currentTargetPos, 0);
        isBreaking = false;
        playerController.SetBreakingAnimation(false);
        breakProgress = 0f;
    }
}