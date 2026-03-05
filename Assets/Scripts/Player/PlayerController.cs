using System;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    // Config
    [SerializeField] GameConfig gameConfig;

    // Block breaking mechanic
    [SerializeField] WorldInteractionManager worldInteractionManager;

    // State
    private Vector2 moveInput;
    private Vector2Int moveDirection;

    private Transform playerTransform;
    private Rigidbody2D rb;
    private Animator myAnimator;
    private SpriteRenderer mySpriteRenderer;

    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private InventoryUI inventoryUI;

    public void Start()
    {
        playerTransform = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void FixedUpdate()
    {
        // Handle movement
        rb.linearVelocity = moveInput * gameConfig.player_speed;

        // TODO make this work together and be more efficient
        if (moveInput.x < 0)
        {
            mySpriteRenderer.flipX = true;
        }
        else if (moveInput.x > 0)
        {
            mySpriteRenderer.flipX = false;
        }

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

    public Vector3 GetPosition()
    {
        return playerTransform.position;
    }

    public Vector2Int GetTargettingBlock()
    {
        Vector3 position = GetPosition();
        Vector2Int cellPosition = worldInteractionManager.PositionToCoordinate(position);
        return cellPosition + moveDirection;
    }

    public void TriggerAttackAnimation()
    {
        myAnimator.SetTrigger("attack");
    }

    public void Move(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        moveInput = input;

        myAnimator.SetFloat("moveX", input.x);
        myAnimator.SetFloat("moveY", input.y);
    }

    public void Attack(InputAction.CallbackContext context)
    {

        if (inventoryUI !=null && inventoryUI.IsOpen)
        {
            return;
        }

        // Skip started phase to avoid processing input twice (once for started and once for performed)
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                Vector2Int cellPosition = GetTargettingBlock();
                if (worldInteractionManager.StartBlockBreaking(cellPosition))
                {
                    break;
                }
                // TODO: No block to break: attack!
                break;
            case InputActionPhase.Canceled:
                worldInteractionManager.CancelBlockBreaking();
                break;
            default:
                return;
        }
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