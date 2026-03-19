using UnityEngine;
using System;

public class BlockInfoManager : MonoBehaviour
{
    public static event Action<BlockInfoManager> OnBlockInfoUpdate;

    [Header("Debugging")]
    [SerializeField] private MovementManager movementManager;
    [SerializeField] private Tool currentTool;
    [SerializeField] private BlockType currentBlock;

    private bool isPageOpen;

    private void Awake()
    {
        movementManager = GetComponent<MovementManager>();
        HotBar.OnHotBarUpdate += HandleHotBarUpdate;
        UIManager.OnUpdatePage += HandleUpdatePage;
        MovementManager.OnMoveDirectionChanged += HandleMoveDirectionChanged;
        MovementManager.OnTilePositionChanged += HandleTilePositionChanged;
        TileMapManager.OnBlockChanged += HandleBlockChanged;
    }

    private void HandleHotBarUpdate(HotBar hotBar)
    {
        currentTool = hotBar.GetSelectedTool();
        Refresh();
    }

    private void HandleUpdatePage(UIPage page)
    {
        isPageOpen = page != UIPage.None;
        if (isPageOpen)
        {
            OnBlockInfoUpdate?.Invoke(null);
            currentBlock = null;
        }
    }

    private void HandleMoveDirectionChanged(Vector2Int _)
    {
        Refresh();
    }

    private void HandleTilePositionChanged(Vector2Int _)
    {
        Refresh();
    }

    private void HandleBlockChanged((BlockType blockType, Vector2Int position) blockInfo)
    {
        Vector2Int playerPos = TileMapManager.Instance.PositionToCoordinate(movementManager.transform.position);
        if (blockInfo.position == playerPos + movementManager.GetMoveDirection())
            Refresh();
    }

    private void Refresh()
    {
        if (isPageOpen) return;

        currentBlock = movementManager.GetTargetBlock();
        bool hasAction = currentBlock != null && (currentBlock.breakable || currentBlock.interactable);
        OnBlockInfoUpdate?.Invoke(hasAction ? this : null);
    }

    public BlockType GetTargetBlock()
    {
        return currentBlock;
    }

    public bool CanBreakWithCurrentTool(BlockType block)
    {
        return BlockBreakingManager.CalculateEfficiency(block, currentTool) > 0f;
    }

    public Tool GetRelevantTool(BlockType block)
    {
        if (block.toolType == ToolType.None) return null;
        if (currentTool != null && currentTool.type == block.toolType && CanBreakWithCurrentTool(block)) return currentTool;
        if (block.minimumToolTier == ToolTier.None) return ToolRepository.GetLowestTierTool(block.toolType);
        return ToolRepository.GetTool(block.toolType, block.minimumToolTier);
    }
}
