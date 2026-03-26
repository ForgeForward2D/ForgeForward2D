using System;

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionManager : MonoBehaviour
{
    public static event Action<(UIPage, BlockType, Vector2Int, bool)> OnAttackUpdate;
    public static event Action<(UIPage, BlockType, Vector2Int)> OnBlockInteraction;

    private static int npcLayerMask;
    private float lastDialogueNavY;

    [Header("Debugging")]
    [SerializeField] private MovementManager movementManager;
    [SerializeField] private Transform playerTransform;

    public void Awake()
    {
        movementManager = GetComponent<MovementManager>();
        playerTransform = GetComponent<Transform>();
        npcLayerMask = LayerMask.GetMask("NPC");

        InputManager.OnAttackUpdate += HandleAttackUpdate;
        InputManager.OnInteractionInput += HandleInteractionInput;
        InputManager.OnMoveInput += HandleMoveInput;
    }

    private void HandleMoveInput((UIPage uiPage, bool performed, Vector2 input) data)
    {
        if (data.uiPage != UIPage.Dialogue) return;

        if (!data.performed)
        {
            lastDialogueNavY = 0f;
            return;
        }

        float y = data.input.y;
        if (y == lastDialogueNavY) return;
        lastDialogueNavY = y;

        if (y > 0f) HandleDialogueNavigate(1);
        else if (y < 0f) HandleDialogueNavigate(-1);
    }

    public void HandleAttackUpdate((UIPage, bool) data)
    {
        var (uiPage, isAttacking) = data;

        if (uiPage == UIPage.Dialogue)
        {
            if (isAttacking) HandleDialogueNavigate(-1);
            return;
        }

        Vector3 player3DPosition = playerTransform.position;
        Vector2Int playerPosition = TileMapManager.Instance.PositionToCoordinate(player3DPosition);
        Vector2Int targetPos = playerPosition + movementManager.GetMoveDirection();
        BlockType blockType = TileMapManager.Instance.GetBlockTypeAtPosition(targetPos);

        OnAttackUpdate?.Invoke((uiPage, blockType, targetPos, isAttacking));
    }


    public void HandleInteractionInput(UIPage uiPage)
    {
        Vector3 player3DPosition = playerTransform.position;
        Vector2Int playerPosition = TileMapManager.Instance.PositionToCoordinate(player3DPosition);
        Vector2Int targetPos = playerPosition + movementManager.GetMoveDirection();

        Vector3 targetWorldPos = TileMapManager.Instance.CoordinateToPosition(targetPos);
        Collider2D npcHit = Physics2D.OverlapPoint(targetWorldPos, npcLayerMask);
        if (npcHit != null)
        {
            NpcController npc = npcHit.GetComponent<NpcController>();
            if (npc == null)
            {
                Debug.LogWarning("Hit an object on the NPC layer that doesn't have an NpcController component.");
            }
            else
            {
                if (uiPage == UIPage.None)
                    Debug.Log($"Triggering NPC interaction with {npc.GetDisplayName()}");
                npc.HandleInteraction(uiPage);
                return;
            }
        }

        BlockType blockType = TileMapManager.Instance.GetBlockTypeAtPosition(targetPos);

        Debug.Log($"Triggering interaction of {(blockType == null ? "Air" : blockType.displayName)}");

        OnBlockInteraction?.Invoke((uiPage, blockType, targetPos));
    }

    private void HandleDialogueNavigate(int direction)
    {
        Vector2Int targetPos = TileMapManager.Instance.PositionToCoordinate(playerTransform.position) + movementManager.GetMoveDirection();
        Vector3 targetWorldPos = TileMapManager.Instance.CoordinateToPosition(targetPos);

        Collider2D npcHit = Physics2D.OverlapPoint(targetWorldPos, npcLayerMask);
        if (npcHit == null) return;

        NpcController npc = npcHit.GetComponent<NpcController>();
        npc?.HandleDialogueNavigate(direction);
    }
}
