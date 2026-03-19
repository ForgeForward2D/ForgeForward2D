using UnityEngine;
using System;

public class TargetBlockInfoManager : MonoBehaviour
{
    public static event Action<TargetBlockInfoManager> OnBlockInfoUpdate;

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
        MovementManager.OnTargetPositionChanged += HandleTargetPositionChanged;
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

    private void HandleTargetPositionChanged(Vector2Int _)
    {
        Refresh();
    }

    private void HandleBlockChanged((BlockType blockType, Vector2Int position) blockInfo)
    {
        if (blockInfo.position == movementManager.GetTargetPosition())
            Refresh();
    }

    private void Refresh()
    {
        if (isPageOpen) return;

        currentBlock = TileMapManager.Instance.GetBlockTypeAtPosition(movementManager.GetTargetPosition());
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
