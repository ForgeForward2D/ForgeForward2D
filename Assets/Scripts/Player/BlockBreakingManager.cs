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
    private Vector2Int currentTargetPos;

    private bool isBreaking = false;
    private float breakProgress = 0f;
    private DateTime lastProgressUpdateTime;

    void OnEnable()
    {
        lastProgressUpdateTime = DateTime.Now;
        toolHotbar.OnSelectionChanged += HandleSelectionChanged;
    }

    void OnDisable()
    {
        toolHotbar.OnSelectionChanged -= HandleSelectionChanged;
    }

    void Update()
    {
        Vector2Int targetBlock = playerController.GetTargettingBlock();

        if (playerController.IsHoldingAttack)
        {
            HandleBreaking(targetBlock);
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

    private void HandleBreaking(Vector2Int targetPos)
    {
        BlockType block = tileMapManager.GetBlockTypeAtPosition(targetPos);

        float efficiency = CalculateEfficiency(block);

        // Block not breakable with current tool
        if (efficiency <= 0f)
        {
            if (isBreaking) CancelBreaking();
            return;
        }

        // If we moved to a new block, reset progress
        if (targetPos != currentTargetPos)
        {
            tileMapManager.UpdateBlockBreakingProgress(currentTargetPos, 0);
            currentTargetPos = targetPos;
            breakProgress = 0f;
        }
        isBreaking = true;
        playerController.SetBreakingAnimation(true);


        // Progress formula: 
        float deltaProgress = Time.deltaTime * gameConfig.player_breaking_speed * efficiency / block.hardness;
        breakProgress += deltaProgress;

        if (breakProgress >= 1f)
        {
            TriggerBreak(block);
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

    public float CalculateEfficiency(BlockType block)
    {
        if (block == null || !block.breakable) return 0.0f;

        // If the block doesn't require a specific tool, every tool is efficient
        if (block.toolType == ToolType.None)
        {
            return currentTool.efficiency;
        }

        // Wrong tool type: Same as no tool
        if (block.toolType != currentTool.type)
        {
            return block.minimumToolTier == ToolTier.None ? 1.0f : 0.0f;
        }

        if (currentTool.tier < block.minimumToolTier)
        {
            // Tool is not good enough to break the block
            return 0.0f;
        }

        return currentTool.efficiency;
    }

    private void TriggerBreak(BlockType blockType)
    {
        BlockType replacementBlock = BlockTypeRepository.GetBlockById(blockType.replacementBlockId);
        tileMapManager.DrawBlock(replacementBlock, currentTargetPos);
        OnBlockBroken?.Invoke((blockType, currentTargetPos));
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