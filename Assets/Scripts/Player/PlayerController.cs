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

    [SerializeField] private ResourceInventoryUI resourceInventoryUI;

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

        // Handle sprite flipping and movement direction
        float absX = Mathf.Abs(moveInput.x);
        float absY = Mathf.Abs(moveInput.y);

        // Update sprite flip whenever there's horizontal input
        if (moveInput.x != 0)
        {
            mySpriteRenderer.flipX = moveInput.x < 0;
        }

        // Determine movement direction based on dominant axis
        if (absX > absY)
        {
            moveDirection = moveInput.x > 0 ? Vector2Int.right : Vector2Int.left;
        }
        else if (absY > 0)
        {
            moveDirection = moveInput.y > 0 ? Vector2Int.up : Vector2Int.down;
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

        if (resourceInventoryUI !=null && resourceInventoryUI.IsOpen)
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

        if (resourceInventoryUI != null)
        {
            resourceInventoryUI.Toggle();
            Debug.Log("Resource Inventory toggled");
        }
        else
        {
            Debug.LogWarning("ResourceInventoryUI reference is missing in Player!");
        }
    }
}
