using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MobController : MonoBehaviour
{
    private enum MobState
    {
        Idle,
        Wandering,
    }

    [Header("Reference")]
    [SerializeField] private MobType mobType;

    [Header("Debugging")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Vector2 moveDirection;
    [SerializeField] private MobState currentState;
    [SerializeField] private float stateTimer;

    private static readonly Vector2[] WanderDirections =
    {
        Vector2.up,
        Vector2.right,
        Vector2.down,
        Vector2.left,
    };

    public Vector2 MoveDirection => moveDirection;
    public bool IsMoving => currentState == MobState.Wandering;

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

        EnterIdleState();
    }

    private void FixedUpdate()
    {
        switch (currentState)
        {
            case MobState.Idle:
                UpdateIdleState();
                break;
            case MobState.Wandering:
                UpdateWanderState();
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
        TileMapManager tileMapManager = TileMapManager.Instance;
        Vector2Int cellPosition = tileMapManager.PositionToCoordinate(lookAheadPosition);
        return tileMapManager.Traversable(cellPosition);
    }

    public void SetMobType(MobType type)
    {
        mobType = type;
    }
}