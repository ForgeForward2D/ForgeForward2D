using System;

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionManager : MonoBehaviour
{
    public static event Action<(UIPage, BlockType, Vector2Int, bool)> OnAttackUpdate;
    public static event Action<(UIPage, BlockType, Vector2Int)> OnInteraction;

    [Header("Debugging")]
    [SerializeField] private MovementManager movementManager;
    [SerializeField] private Transform playerTransform;

    public void Awake()
    {
        movementManager = GetComponent<MovementManager>();
        playerTransform = GetComponent<Transform>();

        InputManager.OnAttackUpdate += HandleAttackUpdate;
        InputManager.OnInteractionInput += HandleInteractionInput;
    }

    public void HandleAttackUpdate((UIPage, bool) data)
    {
        var (uiPage, attackStatus) = data;

        Vector3 player3DPosition = playerTransform.position;
        Vector2Int playerPosition = TileMapManager.Instance.PositionToCoordinate(player3DPosition);
        Vector2Int targetPos = playerPosition + movementManager.GetMoveDirection();
        BlockType blockType = TileMapManager.Instance.GetBlockTypeAtPosition(targetPos);

        OnAttackUpdate?.Invoke((uiPage, blockType, targetPos, attackStatus));
    }


    public void HandleInteractionInput(UIPage uiPage)
    {
        Vector3 player3DPosition = playerTransform.position;
        Vector2Int playerPosition = TileMapManager.Instance.PositionToCoordinate(player3DPosition);
        Vector2Int targetPos = playerPosition + movementManager.GetMoveDirection();
        BlockType blockType = TileMapManager.Instance.GetBlockTypeAtPosition(targetPos);

        Debug.Log($"Triggering interaction of {(blockType == null ? "Air" : blockType.displayName)}");

        OnInteraction?.Invoke((uiPage, blockType, targetPos));
    }

}
