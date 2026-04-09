using System;

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionManager : MonoBehaviour
{
    public static event Action<(UIPage, BlockType, Vector2Int, bool)> OnAttackUpdate;
    public static event Action<(UIPage, BlockType, Vector2Int)> OnBlockInteraction;

    private int npcLayerMask;

    [Header("Debugging")]
    [SerializeField] private MovementManager movementManager;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private InventoryManager inventoryManager;


    public void Awake()
    {
        movementManager = GetComponent<MovementManager>();
        playerTransform = GetComponent<Transform>();
        inventoryManager = GetComponent<InventoryManager>();

        npcLayerMask = LayerMask.GetMask("NPC");

        InputManager.OnAttackInputUpdate += HandleAttackUpdate;
        InputManager.OnInteractionInput += HandleInteractionInput;
    }

    public void HandleAttackUpdate((UIPage, bool) data)
    {
        var (uiPage, isAttacking) = data;

        Vector3 player3DPosition = playerTransform.position;
        Vector2Int playerPosition = TileMapManager.Instance.PositionToCoordinate(player3DPosition);
        Vector2Int targetPos = playerPosition + movementManager.GetMoveDirection();
        BlockType blockType = TileMapManager.Instance.GetBlockTypeAtPosition(targetPos);

        OnAttackUpdate?.Invoke((uiPage, blockType, targetPos, isAttacking));
    }


    public void HandleInteractionInput(UIPage uiPage)
    {
        Vector3 player3DPosition = playerTransform.position;
        Vector2Int playerPosition = TileMapManager.Instance.PositionToCoordinate(player3DPosition);
        Vector2Int targetPos = playerPosition + movementManager.GetMoveDirection();

        Vector3 targetWorldPos = TileMapManager.Instance.CoordinateToPosition(targetPos);
        Collider2D npcHit = Physics2D.OverlapPoint(targetWorldPos, npcLayerMask);
        if (npcHit != null)
        {
            NpcController npc = npcHit.GetComponent<NpcController>();
            if (npc == null)
            {
                Debug.LogWarning("Hit an object on the NPC layer that doesn't have an NpcController component.");
            }
            else
            {
                if (uiPage == UIPage.None)
                {
                    Debug.Log($"Triggering NPC interaction with {npc.GetDisplayName()}");
                    if (npc.acceptsSword && inventoryManager != null)
                    {
                        SwordItem bestSword = null;
                        foreach (var item in inventoryManager.resourceInventory.GetItems())
                        {
                            if (item != null && item.itemType is SwordItem sword && sword.swordLevel > npc.swordLevel)
                            {
                                if (bestSword == null || sword.swordLevel > bestSword.swordLevel)
                                    bestSword = sword;
                            }
                        }

                        if (bestSword != null)
                        {
                            npc.GiveSword(bestSword);
                            inventoryManager.RemoveItemOfType(bestSword, 1);
                            MobSpawner spawner = FindAnyObjectByType<MobSpawner>();
                            if (spawner != null)
                            {
                                spawner.SpawnMobs();
                            }
                            else
                            {
                                Debug.LogError($"No MobSpawner found in the scene. Make sure there is a MobSpawner component in the scene for mob spawn reduction to work.");
                            }
                        }
                        else
                        {
                            SwordItem anySword = null;
                            foreach (var item in inventoryManager.resourceInventory.GetItems())
                            {
                                if (item != null && item.itemType is SwordItem sword)
                                {
                                    if (anySword == null || sword.swordLevel > anySword.swordLevel)
                                        anySword = sword;
                                }
                            }
                            if (anySword != null)
                                npc.RejectSword(anySword);
                        }
                    }
                    npc.HandleInteraction(uiPage);
                }
                return;
            }
        }

        BlockType blockType = TileMapManager.Instance.GetBlockTypeAtPosition(targetPos);

        Debug.Log($"Triggering interaction of {(blockType == null ? "Air" : blockType.displayName)}");

        OnBlockInteraction?.Invoke((uiPage, blockType, targetPos));
    }
}
