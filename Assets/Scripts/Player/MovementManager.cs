using System;

using UnityEngine;
using UnityEngine.InputSystem;

public class MovementManager : MonoBehaviour
{
    public static event Action<Vector2Int> OnTargetPositionChanged;

    [SerializeField] private GameConfig gameConfig;
    [SerializeField] private Transform characterModel;

    [Header("Debugging")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Vector2 moveInput;
    [SerializeField] private Vector2Int moveDirection;

    private Vector2Int previousTargetPosition;

    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponentInChildren<Animator>();

        InputManager.OnMoveInput += HandleMoveInput;
        UIManager.OnUpdatePage += HandleUpdatePage;
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

    public void FixedUpdate()
    {
        // Handle movement
        rb.linearVelocity = moveInput * gameConfig.player_speed;

        // Handle sprite flipping and movement direction
        float absX = Mathf.Abs(moveInput.x);
        float absY = Mathf.Abs(moveInput.y);

        Vector2Int previousDirection = moveDirection;

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

        Vector2Int targetPosition = GetTargetPosition();
        if (targetPosition != previousTargetPosition)
        {
            previousTargetPosition = targetPosition;
            OnTargetPositionChanged?.Invoke(targetPosition);
        }
    }

    public Vector2Int GetMoveDirection()
    {
        return moveDirection;
    }

    public Vector2Int GetTargetPosition()
    {
        Vector2Int playerPos = TileMapManager.Instance.PositionToCoordinate(transform.position);
        return playerPos + moveDirection;
    }
}
