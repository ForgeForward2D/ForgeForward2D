using UnityEngine;
using System;

public class BlockBreakingManager : MonoBehaviour
{
    public static event Action<(BlockType, Vector2Int)> OnBlockBroken;

    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private Animator playerAnimator;

    [Header("Debugging")]
    [SerializeField] private MovementManager movementManager;
    [SerializeField] private Transform playerTransform;

    public Tool currentTool;

    private bool isPlayerHoldingAttack=false;
    private bool isBreaking = false;
    private float breakProgress = 0f;
    private Vector2Int currentTargetPos;
    private BlockType currentTargetBlock;
    private DateTime lastProgressUpdateTime;

    private void Awake()
    {
        movementManager = GetComponent<MovementManager>();
        playerTransform = GetComponent<Transform>();

        lastProgressUpdateTime = DateTime.Now;
        currentTool = ItemTypeRepository.GetDefaultTool();

        InputManager.OnAttackUpdate += HandleAttackInput;
        HotBar.OnHotBarUpdate += HandleHotBarUpdate;
        TileMapManager.OnBlockChanged += HandleBlockChanged;
    }


    void Update()
    {
        if (isPlayerHoldingAttack)
        {
            HandleBreaking();
        }
        else if (isBreaking)
        {
            CancelBreaking();
        }
    }

    private void HandleAttackInput((UIPage, bool) data)
    {
        var (uiPage, attackStatus) = data;

        if (uiPage == UIPage.None) 
        {
            isPlayerHoldingAttack = attackStatus;
        }
        else 
        {
            isPlayerHoldingAttack = false;
        }
    }

    private void HandleHotBarUpdate(HotBar hotBar)
    {
        currentTool = hotBar.GetSelectedTool();
    }

    private void HandleBlockChanged((BlockType blockType, Vector2Int position) blockInfo)
    {
        // If the block that changed is the one we're currently breaking, update our reference
        if (blockInfo.position == currentTargetPos)
        {
            currentTargetBlock = blockInfo.blockType;
        }
    }

    private void HandleBreaking()
    {
        Vector3 player3DPosition = playerTransform.position;
        Vector2Int playerPosition = TileMapManager.Instance.PositionToCoordinate(player3DPosition);
        Vector2Int targetPos = playerPosition + movementManager.GetMoveDirection();
        // If we moved to a new block, reset progress
        if (targetPos != currentTargetPos)
        {
            TileMapManager.Instance.UpdateBlockBreakingProgress(currentTargetPos, 0);
            currentTargetPos = targetPos;
            currentTargetBlock = TileMapManager.Instance.GetBlockTypeAtPosition(currentTargetPos);
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
        playerAnimator.SetBool("isBreaking", isBreaking);


        // Progress formula:
        float hardness = currentTargetBlock == null ? 1f : currentTargetBlock.hardness;
        float deltaProgress = Time.deltaTime * gameConfig.player_breaking_speed * efficiency / hardness;
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
            TileMapManager.Instance.UpdateBlockBreakingProgress(currentTargetPos, stage);
        }
    }

    public float CalculateEfficiency()
    {
        if (currentTool == null) currentTool = ItemTypeRepository.GetDefaultTool();
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
        BlockType replacementBlock = currentTargetBlock.replacementBlock; 
        OnBlockBroken?.Invoke((currentTargetBlock, currentTargetPos));
        TileMapManager.Instance.DrawBlock(replacementBlock, currentTargetPos);
        CancelBreaking();
    }

    private void CancelBreaking()
    {
        TileMapManager.Instance.UpdateBlockBreakingProgress(currentTargetPos, 0);
        isBreaking = false;
        playerAnimator.SetBool("isBreaking", isBreaking);
        breakProgress = 0f;
    }
}