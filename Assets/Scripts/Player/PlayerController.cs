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
    [SerializeField] private Vector2 moveInput;
    [SerializeField] private Vector2Int moveDirection;

    private Transform myTransform;
    private Rigidbody2D rb;
    private Animator myAnimator;
    private SpriteRenderer mySpriteRenderer;


    public void Start()
    {
        myTransform = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
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
                mySpriteRenderer.flipX = false;
            }
            else
            {
                // Moving left
                moveDirection = Vector2Int.left;
                mySpriteRenderer.flipX = true;
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
        return myTransform.position;
    }

    public Vector2Int GetTargettingBlock()
    {
        return worldInteractionManager.PositionToCoordinate(GetPosition()) + moveDirection;
    }

    public void TriggerAttackAnimation()
    {
        Debug.Log("Attack animation triggered");
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
        // Skip started phase to avoid processing input twice (once for started and once for performed)
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                Vector2Int cellPosition = GetTargettingBlock();
                if (worldInteractionManager.StartBlockBreaking(cellPosition))
                {
                    break;
                }
                Debug.Log("No block to break, attack!");
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

        Debug.Log("Inventory toggled");
    }
}
