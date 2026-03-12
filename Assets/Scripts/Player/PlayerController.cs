using System;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    // Config
    [SerializeField] GameConfig gameConfig;

    // Block breaking mechanic
    [SerializeField] TileMapManager tileMapManager;

    [SerializeField] private ResourceInventoryUI resourceInventoryUI;
    [SerializeField] private AchievementUI achievementUI;

    // State
    private Vector2 moveInput;
    private Vector2Int moveDirection;

    private Transform playerTransform;
    private Rigidbody2D rb;
    private Animator myAnimator;
    private SpriteRenderer mySpriteRenderer;

    public bool IsHoldingAttack { get; private set; }

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
            mySpriteRenderer.flipX = moveInput.x > 0;
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

        if ((resourceInventoryUI != null && resourceInventoryUI.IsOpen) || (achievementUI != null && achievementUI.IsOpen))
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

    public void Inventory(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed) return;

        if (achievementUI != null && achievementUI.IsOpen)
        {
            Debug.Log("Cannot open Inventory: Achievements are currently open!");
            return;
        }

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

    public void Achievements(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed) return;

        if (resourceInventoryUI != null && resourceInventoryUI.IsOpen)
        {
            Debug.Log("Cannot open Achievements: Inventory is currently open!");
            return;
        }

        if (achievementUI != null)
        {
            achievementUI.Toggle();
            Debug.Log("Achievement UI toggled");
        }
        else
        {
            Debug.LogWarning("AchievementUI reference is missing in Player!");
        }
    }
}
