using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MobController : MonoBehaviour
{
    private enum MobState
    {
        Idle,
        Wandering,
        Chasing,
    }

    public static event Func<int, MobType, List<Item>> OnMobStealItems;

    [Header("Reference")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Debugging")]
    [SerializeField] private MobType mobType;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Vector2 moveDirection;
    [SerializeField] private MobState currentState;
    [SerializeField] private float stateTimer;

    [Header("Settings")]
    [SerializeField] private float repathInterval = 0.3f;


    private Transform playerTransform;
    private List<Vector2Int> chasePath = new();
    private float repathTimer;
    private float stealTimer;
    private float consumeTimer;
    private List<Item> inventory = new();

    private static Vector2[] WanderDirections =
    {
        Vector2.up,
        Vector2.right,
        Vector2.down,
        Vector2.left,
    };

    public Vector2 MoveDirection => moveDirection;
    public bool IsMoving => currentState == MobState.Wandering || currentState == MobState.Chasing;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentState = MobState.Idle;

        if (mobType == null)
        {
            Debug.LogError("MobController requires a MobType to be assigned.", this);
            enabled = false;
            return;
        }

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            playerTransform = playerObj.transform;
        else
            Debug.LogError($"Mob Controller: No player object found");

        if (repathInterval < 0.01f)
        {
            Debug.LogWarning($"repath interval is set to {repathInterval} which may cause performance issues. Consider using a value >= 0.3");
        }

        EnterIdleState();
    }

    private void FixedUpdate()
    {
        if (!IsHungry())
        {
            consumeTimer += Time.fixedDeltaTime;
            float consumeInterval = 60f / mobType.itemsConsumedPerMin;
            if (consumeTimer >= consumeInterval)
            {
                Consume();
                consumeTimer = 0f;
            }
        }

        if (IsHungry() && IsPlayerInRange())
        {
            if (currentState != MobState.Chasing)
                EnterChaseState();
        }
        else if (currentState == MobState.Chasing)
        {
            EnterIdleState();
        }

        switch (currentState)
        {
            case MobState.Idle:
                UpdateIdleState();
                break;
            case MobState.Wandering:
                UpdateWanderState();
                break;
            case MobState.Chasing:
                UpdateChaseState();
                break;
        }
    }

    private bool IsPlayerInRange()
    {
        float distance = Vector2.Distance(transform.position, playerTransform.position);
        return distance <= mobType.aggroRange;
    }

    private void EnterChaseState()
    {
        currentState = MobState.Chasing;
        repathTimer = 0f;
        stealTimer = 0f;
        chasePath.Clear();
    }

    private void UpdateChaseState()
    {
        repathTimer -= Time.fixedDeltaTime;
        if (repathTimer <= 0f)
        {
            repathTimer = repathInterval;
            Vector2Int source = TileMapManager.Instance.PositionToCoordinate(transform.position);
            Vector2Int destination = TileMapManager.Instance.PositionToCoordinate(playerTransform.position);
            chasePath = Pathfinder.FindPath(source, destination);
        }

        if (mobType.stealsResources)
        {
            float dist = Vector2.Distance(transform.position, playerTransform.position);
            if (dist <= mobType.stealRange)
            {
                stealTimer += Time.fixedDeltaTime;
                if (stealTimer >= mobType.stealInterval)
                {
                    int count = UnityEngine.Random.Range(1, mobType.maxItemsStolen + 1);
                    List<Item> stolen = OnMobStealItems?.Invoke(count, mobType);
                    if (stolen != null)
                    {
                        foreach (Item item in stolen)
                            AddToInventory(item);
                    }
                    stealTimer = 0f;
                }
            }
            else
            {
                stealTimer = 0f;
            }
        }

        if (chasePath.Count == 0)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector3 nextWorldPos = TileMapManager.Instance.CoordinateToPosition(chasePath[0]);
        Vector2 toNext = (Vector2)nextWorldPos - (Vector2)transform.position;

        if (toNext.magnitude < 0.1f)
        {
            chasePath.RemoveAt(0);
            if (chasePath.Count == 0)
            {
                rb.linearVelocity = Vector2.zero;
                return;
            }
            nextWorldPos = TileMapManager.Instance.CoordinateToPosition(chasePath[0]);
            toNext = (Vector2)nextWorldPos - (Vector2)transform.position;
        }

        moveDirection = toNext.normalized;
        rb.linearVelocity = moveDirection * mobType.chaseSpeed;
    }

    private void EnterIdleState()
    {
        currentState = MobState.Idle;
        stateTimer = UnityEngine.Random.Range(mobType.idleDurationRange.x, mobType.idleDurationRange.y);
    }

    private void UpdateIdleState()
    {
        rb.linearVelocity = Vector2.zero;
        stateTimer -= Time.fixedDeltaTime;

        if (stateTimer <= 0f)
        {
            EnterWanderState();
        }
    }

    private void EnterWanderState()
    {
        moveDirection = PickNextDirection();
        if (moveDirection == Vector2.zero)
        {
            EnterIdleState();
            return;
        }

        currentState = MobState.Wandering;
        stateTimer = UnityEngine.Random.Range(mobType.walkDurationRange.x, mobType.walkDurationRange.y);
    }

    private void UpdateWanderState()
    {
        stateTimer -= Time.fixedDeltaTime;

        if (stateTimer <= 0f || !CanMoveInDirection(moveDirection))
        {
            EnterIdleState();
            return;
        }

        rb.linearVelocity = moveDirection * mobType.moveSpeed;
    }

    private Vector2 PickNextDirection()
    {
        int startIndex = UnityEngine.Random.Range(0, WanderDirections.Length);
        for (int index = 0; index < WanderDirections.Length; index++)
        {
            Vector2 candidate = WanderDirections[(startIndex + index) % WanderDirections.Length];
            if (CanMoveInDirection(candidate))
            {
                return candidate;
            }
        }

        return Vector2.zero;
    }

    private bool CanMoveInDirection(Vector2 direction)
    {
        if (direction == Vector2.zero)
        {
            return false;
        }

        Vector3 lookAheadPosition = transform.position + (Vector3)(direction.normalized * mobType.directionCheckDistance);
        Vector2Int cellPosition = TileMapManager.Instance.PositionToCoordinate(lookAheadPosition);
        return TileMapManager.Instance.Walkable(cellPosition);
    }

    public void SetMobType(MobType type)
    {
        mobType = type;
        if (spriteRenderer != null) 
        {
            spriteRenderer.sprite = mobType.sprite;
        }
        else 
        {
            Debug.LogWarning($"MobController has no SpriteRenderer assigned, cannot set sprite for mob type {mobType.displayName}");
        }
    }

    private void Consume()
    {
        if (inventory.Count == 0) return;

        Item last = inventory[inventory.Count - 1];
        last.count--;
        if (last.count <= 0)
        {
            inventory.RemoveAt(inventory.Count - 1);
        }
        Debug.Log($"{mobType.displayName} consumed 1 {last.itemType.displayName}");
    }

    private void AddToInventory(Item item)
    {
        foreach (Item existing in inventory)
        {
            if (existing.itemType == item.itemType)
            {
                existing.count += item.count;
                return;
            }
        }
        inventory.Add(new Item(item.itemType, item.count));
    }

    private bool IsHungry()
    {
        return inventory.Count == 0;
    }
}