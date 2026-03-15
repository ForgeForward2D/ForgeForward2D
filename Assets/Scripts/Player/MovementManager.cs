using System;

using UnityEngine;
using UnityEngine.InputSystem;

public class MovementManager : MonoBehaviour
{
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private Transform characterModel;

    [Header("Debug")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Vector2 moveInput;
    [SerializeField] private Vector2Int moveDirection;

    public bool IsHoldingAttack { get; private set; }

    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponentInChildren<Animator>();

        InputManager.OnMoveInput += HandleMoveInput;
    }

    private void HandleMoveInput((UIPage, Vector2) data) 
    {
        var (uiPage, input) = data;
        if (uiPage == UIPage.None)
        {
            moveInput = input;
            playerAnimator.SetBool("isMoving", input.magnitude > 0.01f);
        }
        else
        {
            moveInput = Vector2.zero;
        }
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

    public Vector2Int GetMoveDirection() 
    {
        return moveDirection ;
    }

    // public void Achievements(InputAction.CallbackContext context)
    // {
    //     if (context.phase != InputActionPhase.Performed) return;

    //     if (resourceInventoryUI != null && resourceInventoryUI.IsOpen)
    //     {
    //         Debug.Log("Cannot open Achievements: Inventory is currently open!");
    //         return;
    //     }

    //     if (achievementUI != null)
    //     {
    //         achievementUI.Toggle();
    //         Debug.Log("Achievement UI toggled");
    //     }
    //     else
    //     {
    //         Debug.LogWarning("AchievementUI reference is missing in Player!");
    //     }
    // }

    // public void OnScrollAchievements(InputAction.CallbackContext context)
    // {
    //     if (achievementUI != null && achievementUI.IsOpen)
    //     {
    //         float value = context.ReadValue<float>();
    //         achievementUI.SetScrollInput(value);
    //     }
    // }
}
