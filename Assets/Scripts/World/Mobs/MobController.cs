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

    [Header("Reference")]
    [SerializeField] private MobType mobType;

    [Header("Debugging")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Vector2 moveDirection;
    [SerializeField] private MobState currentState;
    [SerializeField] private float stateTimer;

    [Header("Settings")]
    [SerializeField] private float RepathInterval;

    private Transform playerTransform;
    private List<Vector2Int> chasePath = new();
    private float repathTimer;

    private static readonly Vector2[] WanderDirections =
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


        EnterIdleState();
    }

    private void FixedUpdate()
    {
        if (IsPlayerInRange())
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

    private void UpdateIdleState()
    {
        rb.linearVelocity = Vector2.zero;
        stateTimer -= Time.fixedDeltaTime;

        if (stateTimer <= 0f)
        {
            EnterWanderState();
        }
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

    private bool IsPlayerInRange()
    {
        float distance = Vector2.Distance(transform.position, playerTransform.position);
        return distance <= (float)mobType.aggroRange;
    }

    private void EnterChaseState()
    {
        currentState = MobState.Chasing;
        repathTimer = 0f;
        chasePath.Clear();
    }

    private void UpdateChaseState()
    {
        repathTimer -= Time.fixedDeltaTime;
        if (repathTimer <= 0f)
        {
            repathTimer = RepathInterval;
            Vector2Int from = TileMapManager.Instance.PositionToCoordinate(transform.position);
            Vector2Int to = TileMapManager.Instance.PositionToCoordinate(playerTransform.position);
            chasePath = Pathfinder.FindPath(from, to);
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
        stateTimer = Random.Range(mobType.idleDurationRange.x, mobType.idleDurationRange.y);
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
        stateTimer = Random.Range(mobType.walkDurationRange.x, mobType.walkDurationRange.y);
    }

    private Vector2 PickNextDirection()
    {
        int startIndex = Random.Range(0, WanderDirections.Length);
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
    }
}