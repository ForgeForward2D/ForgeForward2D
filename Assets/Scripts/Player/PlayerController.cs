using System;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    // Config
    [SerializeField] GameConfig gameConfig;

    // Movement
    [SerializeField] Rigidbody2D rb;

    // Block breaking mechanic
    [SerializeField] TileMapManager tileMapManager;

    // State
    [SerializeField] private Vector2 moveInput;
    [SerializeField] private Vector2Int moveDirection;

    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private InventoryUI inventoryUI;

    public event Action<(BlockType, Vector2Int)> OnBlockBroken;

    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void FixedUpdate()
    {
        // Handle movement
        rb.linearVelocity = moveInput * gameConfig.player_speed;

        // Handle direction for sprite and recording latest moveDirection 
        if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
        {
            if (moveInput.x > 0)
            {
                // Moving right
                moveDirection = Vector2Int.right;
            }
            else
            {
                // Moving left
                moveDirection = Vector2Int.left;
            }
        }
        else
        {
            if (moveInput.y > 0)
            {
                // Moving up
                moveDirection = Vector2Int.up;
            }
            else if (moveInput.y < 0)
            {
                // Moving down
                moveDirection = Vector2Int.down;
            }
        }

    }

    public void Move(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        moveInput = input;
    }

    public void Attack(InputAction.CallbackContext context)
    {

        if (inventoryUI !=null && inventoryUI.IsOpen)
        {
            return;
        }

        if (context.phase != InputActionPhase.Performed) return;

        // Todo: Refactor
        // Maybe push more into resource generation?
        Vector2Int cellPosition = tileMapManager.PositionToCoordinate(transform.position) + Vector2Int.RoundToInt(moveDirection);

        BlockType brokenBlockType = tileMapManager.GetBlockTypeAtPosition(cellPosition);

        if (brokenBlockType == null)
        {
            Debug.Log("Trying to break null block at " + cellPosition);
            return;
        }

        if (!brokenBlockType.breakable)
        {
            Debug.Log("Trying to break unbreakable block (" + brokenBlockType.displayName + ") at " + cellPosition);
            return;
        }

        BlockType replacementBlockType = BlockTypeRepository.GetBlockById(brokenBlockType.replacementBlockId);

        tileMapManager.DrawBlock(replacementBlockType, cellPosition);

        inventoryManager?.AddItem(brokenBlockType.itemID);

        Vector2Int cellPosition2D = new Vector2Int(cellPosition.x, cellPosition.y);
        Debug.Log("[EVENT] Broke " + brokenBlockType?.displayName + " at " + cellPosition2D);
        OnBlockBroken?.Invoke((brokenBlockType, cellPosition2D));
    }
    public void Inventory(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed) return;

        if (inventoryUI != null)
        {
            inventoryUI.Toggle();
            Debug.Log("Inventory toggled");
        }
        else
        {
            Debug.LogWarning("InventoryUI reference is missing in Player!");
        }
    }
}
