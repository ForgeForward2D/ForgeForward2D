using System;

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Config
    [SerializeField] GameConfig gameConfig;

    // Block breaking mechanic
    [SerializeField] TileMapManager tileMapManager;

    // State
    private Vector2 moveInput;
    private Vector2Int moveDirection;

    private Transform playerTransform;
    private Rigidbody2D rb;
    private Animator myAnimator;
    public Transform characterModel;

    [SerializeField] private ResourceInventoryUI resourceInventoryUI;
    [SerializeField] private CraftingTableUI craftingTableUI;

    public bool IsHoldingAttack { get; private set; }

    public void Start()
    {
        playerTransform = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponentInChildren<Animator>();
    }

    public void FixedUpdate()
    {
        // Handle movement
        rb.linearVelocity = moveInput * gameConfig.player_speed;

        // Handle sprite flipping and movement direction
        float absX = Mathf.Abs(moveInput.x);
        float absY = Mathf.Abs(moveInput.y);

        if (absX > absY)
        {
            moveDirection = moveInput.x > 0 ? Vector2Int.right : Vector2Int.left;
            characterModel.localRotation = Quaternion.Euler(0f, moveInput.x > 0 ? -90f : 90f, 0f);
        }
        else if (absY > 0)
        {
            moveDirection = moveInput.y > 0 ? Vector2Int.up : Vector2Int.down;
            characterModel.localRotation = Quaternion.Euler(0f, moveInput.y < 0 ? 0f : 180f, 0f);
        }
    }

    public Vector3 GetPosition()
    {
        return playerTransform.position;
    }

    public Vector2Int GetTargettingBlock()
    {
        Vector3 position = GetPosition();
        Vector2Int cellPosition = tileMapManager.PositionToCoordinate(position);
        return cellPosition + moveDirection;
    }

    public void SetBreakingAnimation(bool isBreaking)
    {
        myAnimator.SetBool("isBreaking", isBreaking);
    }

    public void TriggerAttackAnimation()
    {
        // TODO
    }

    public void Move(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        moveInput = input;

        myAnimator.SetBool("isMoving", input.magnitude > 0.01f);
    }

    public void Attack(InputAction.CallbackContext context)
    {

        if ((resourceInventoryUI != null && resourceInventoryUI.IsOpen) || (craftingTableUI != null && craftingTableUI.IsOpen))
        {
            IsHoldingAttack = false;
            return;
        }

        // Skip started phase to avoid processing input twice (once for started and once for performed)
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                IsHoldingAttack = true;
                break;
            case InputActionPhase.Canceled:
                IsHoldingAttack = false;
                break;
            default:
                return;
        }
    }

    public static event Action<(BlockType, Vector2Int)> OnInteraction;

    public void Interact(InputAction.CallbackContext context)
    {
        if (resourceInventoryUI != null && resourceInventoryUI.IsOpen)
        {
            return;
        }

        if (context.phase != InputActionPhase.Performed) return;

        Debug.Log("Interaction triggered");

        Vector2Int targetBlockPos = GetTargettingBlock();
        BlockType targetBlock = tileMapManager.GetBlockTypeAtPosition(targetBlockPos);
        OnInteraction?.Invoke((targetBlock, targetBlockPos));
    }

    public void Inventory(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed) return;

        if (craftingTableUI != null && craftingTableUI.IsOpen)
         {
             return;
         }

        if (resourceInventoryUI != null)
        {
            resourceInventoryUI.Toggle();
            myAnimator.SetBool("isMoving", !resourceInventoryUI.IsOpen && moveInput.magnitude > 0.01f);
            Debug.Log("Resource Inventory toggled");
        }
        else
        {
            Debug.LogWarning("ResourceInventoryUI reference is missing in Player!");
        }

    }
}
