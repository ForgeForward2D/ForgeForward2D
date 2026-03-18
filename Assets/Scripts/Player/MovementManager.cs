using System;

using UnityEngine;
using UnityEngine.InputSystem;

public class MovementManager : MonoBehaviour
{
    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private Transform characterModel;

    [Header("Debugging")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Vector2 moveInput;
    [SerializeField] private Vector2Int moveDirection;

    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponentInChildren<Animator>();

        InputManager.OnMoveInput += HandleMoveInput;
        UIManager.OnUpdatePage += HandleUpdatePage;
        WorldManager.OnPlayerTeleport += HandleTeleport;
    }

    private void HandleMoveInput((UIPage, bool, Vector2) data)
    {
        var (uiPage, performed, input) = data;
        if (uiPage == UIPage.None)
        {
            moveInput = input;
            playerAnimator.SetBool("isMoving", input.magnitude > 0.01f);
        }
        else
        {
            moveInput = Vector2.zero;
            playerAnimator.SetBool("isMoving", false);
        }
    }

    private void HandleUpdatePage(UIPage page)
    {
        if (page != UIPage.None)
        {
            moveInput = Vector2.zero;
            playerAnimator.SetBool("isMoving", false);
        }
    }

    private void HandleTeleport(Vector3 destination)
    {
        rb.linearVelocity = Vector2.zero;
        rb.position = destination;
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
        return moveDirection;
    }
}
