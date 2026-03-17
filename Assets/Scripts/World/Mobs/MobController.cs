using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MobController : MonoBehaviour
{
    private enum MobState
    {
        Idle,
        Wandering,
    }

    [Header("Settings")]
    [SerializeField] private MobType mobType;
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float directionCheckDistance = 0.45f;
    [SerializeField] private Vector2 idleDurationRange = new Vector2(0.75f, 2f);
    [SerializeField] private Vector2 walkDurationRange = new Vector2(1.5f, 4f);

    [Header("Debugging")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Vector2 moveDirection;
    [SerializeField] private MobState currentState = MobState.Idle;
    [SerializeField] private float stateTimer;
    [SerializeField] private TileMapManager tileMapManager;

    private static readonly Vector2[] WanderDirections =
    {
        Vector2.up,
        Vector2.right,
        Vector2.down,
        Vector2.left,
    };

    public Vector2 MoveDirection => moveDirection;
    public bool IsMoving => currentState == MobState.Wandering;

    private float CurrentMoveSpeed => mobType != null ? mobType.moveSpeed : moveSpeed;
    private float CurrentDirectionCheckDistance => mobType != null ? mobType.directionCheckDistance : directionCheckDistance;
    private Vector2 CurrentIdleDurationRange => mobType != null ? mobType.idleDurationRange : idleDurationRange;
    private Vector2 CurrentWalkDurationRange => mobType != null ? mobType.walkDurationRange : walkDurationRange;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        tileMapManager = TileMapManager.Instance;
        if (tileMapManager == null)
        {
            Debug.LogWarning("MobController has no TileMapManager available at Start; movement will stay idle until configured.", this);
            return;
        }

        EnterIdleState();
    }

    private void FixedUpdate()
    {
        if (tileMapManager == null)
        {
            tileMapManager = TileMapManager.Instance;
            rb.linearVelocity = Vector2.zero;
            return;
        }

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

        rb.linearVelocity = moveDirection * CurrentMoveSpeed;
    }

    private void EnterIdleState()
    {
        currentState = MobState.Idle;
        stateTimer = Random.Range(CurrentIdleDurationRange.x, CurrentIdleDurationRange.y);
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
        stateTimer = Random.Range(CurrentWalkDurationRange.x, CurrentWalkDurationRange.y);
    }

    protected virtual Vector2 PickNextDirection()
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

    protected virtual bool CanMoveInDirection(Vector2 direction)
    {
        if (direction == Vector2.zero || tileMapManager == null)
        {
            return false;
        }

        Vector3 lookAheadPosition = transform.position + (Vector3)(direction.normalized * CurrentDirectionCheckDistance);
        return IsWalkable(lookAheadPosition);
    }

    private bool IsWalkable(Vector3 worldPosition)
    {
        Vector2Int cellPosition = tileMapManager.PositionToCoordinate(worldPosition);
        return tileMapManager.Traversable(cellPosition);
    }

    public void ConfigureNavigation()
    {
        tileMapManager = TileMapManager.Instance;

        if (tileMapManager != null)
        {
            EnterIdleState();
        }
    }

    public void SetMobType(MobType type)
    {
        mobType = type;
    }
}