using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using TMPro;

public class BlockInfoUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI actionLabel;
    [SerializeField] private Image blockIcon;

    private MovementManager movementManager;
    private Tool currentTool;
    private BlockType lastBlock;
    private Vector2Int lastDirection;
    private Tool lastTool;
    private bool isPageOpen;

    private void Awake()
    {
        movementManager = FindFirstObjectByType<MovementManager>();
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }

    private void OnEnable()
    {
        UIManager.OnUpdatePage += HandleUpdatePage;
        HotBar.OnHotBarUpdate += HandleHotBarUpdate;
    }

    private void OnDisable()
    {
        UIManager.OnUpdatePage -= HandleUpdatePage;
        HotBar.OnHotBarUpdate -= HandleHotBarUpdate;
    }

    private void HandleUpdatePage(UIPage page)
    {
        isPageOpen = page != UIPage.None;
        if (isPageOpen)
        {
            canvasGroup.alpha = 0f;
            lastBlock = null;
        }
    }

    private void HandleHotBarUpdate(HotBar hotBar)
    {
        currentTool = hotBar.GetSelectedTool();
    }

    private void Update()
    {
        if (isPageOpen) return;

        Vector2Int direction = movementManager.GetMoveDirection();
        Vector2Int playerPos = TileMapManager.Instance.PositionToCoordinate(movementManager.transform.position);
        Vector2Int targetPos = playerPos + direction;
        BlockType block = TileMapManager.Instance.GetBlockTypeAtPosition(targetPos);

        if (block == lastBlock && direction == lastDirection && currentTool == lastTool) return;
        lastBlock = block;
        lastDirection = direction;
        lastTool = currentTool;

        bool isBreakable = block != null && block.breakable;
        bool isInteractable = block != null && block.interactable;

        if (!isBreakable && !isInteractable)
        {
            canvasGroup.alpha = 0f;
            return;
        }

        canvasGroup.alpha = 1f;

        if (isBreakable)
            actionLabel.text = CanBreakWithCurrentTool(block) ? "Mine" : $"Needs {block.minimumToolTier} {block.toolType}";
        else
            actionLabel.text = "Interact";

        if (block.tile is Tile tile)
            blockIcon.sprite = tile.sprite;
    }

    private bool CanBreakWithCurrentTool(BlockType block)
    {
        if (block.toolType == ToolType.None) return true;

        ToolType toolType = currentTool == null ? ToolType.None : currentTool.type;
        ToolTier toolTier = currentTool == null ? ToolTier.None : currentTool.tier;

        if (block.toolType != toolType)
            return block.minimumToolTier == ToolTier.None;

        return toolTier >= block.minimumToolTier;
    }
}
