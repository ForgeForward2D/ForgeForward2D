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
    private BlockType lastBlock;
    private Vector2Int lastDirection;
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
    }

    private void OnDisable()
    {
        UIManager.OnUpdatePage -= HandleUpdatePage;
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

    private void Update()
    {
        if (isPageOpen) return;

        Vector2Int direction = movementManager.GetMoveDirection();
        Vector2Int playerPos = TileMapManager.Instance.PositionToCoordinate(movementManager.transform.position);
        Vector2Int targetPos = playerPos + direction;
        BlockType block = TileMapManager.Instance.GetBlockTypeAtPosition(targetPos);

        if (block == lastBlock && direction == lastDirection) return;
        lastBlock = block;
        lastDirection = direction;

        bool isBreakable = block != null && block.breakable;
        bool isInteractable = block != null && block.interactable;

        if (!isBreakable && !isInteractable)
        {
            canvasGroup.alpha = 0f;
            return;
        }

        canvasGroup.alpha = 1f;
        actionLabel.text = isBreakable ? "Mine" : "Interact";

        if (block.tile is Tile tile)
            blockIcon.sprite = tile.sprite;
    }
}
