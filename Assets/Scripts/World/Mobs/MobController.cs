using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
public class MobController : MonoBehaviour
{
    private enum MobState
    {
        Idle,
        Wandering,
    }

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform visualRoot;
    [SerializeField] private Tilemap walkableTilemap;
    [SerializeField] private Tilemap blockedTilemap;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float directionCheckDistance = 0.45f;
    [SerializeField] private Vector2 idleDurationRange = new Vector2(0.75f, 2f);
    [SerializeField] private Vector2 walkDurationRange = new Vector2(1.5f, 4f);

    [Header("Debugging")]
    [SerializeField] private Vector2 moveDirection;
    [SerializeField] private MobState currentState = MobState.Idle;
    [SerializeField] private float stateTimer;

    private bool isNavigationConfigured;

    private static readonly Vector2[] WanderDirections =
    {
        Vector2.up,
        Vector2.right,
        Vector2.down,
        Vector2.left,
    };

    public Vector2 MoveDirection => moveDirection;
    public bool IsMoving => currentState == MobState.Wandering;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (walkableTilemap == null)
        {
            Debug.LogWarning("MobController has no walkable Tilemap assigned at Start; movement will stay idle until configured.", this);
            return;
        }

        isNavigationConfigured = true;
        EnterIdleState();
    }

    private void FixedUpdate()
    {
        if (walkableTilemap == null)
        {
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

    private void OnDisable()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    protected virtual void UpdateIdleState()
    {
        rb.linearVelocity = Vector2.zero;
        stateTimer -= Time.fixedDeltaTime;

        if (stateTimer <= 0f)
        {
            EnterWanderState();
        }
    }

    protected virtual void UpdateWanderState()
    {
        stateTimer -= Time.fixedDeltaTime;

        if (stateTimer <= 0f || !CanMoveInDirection(moveDirection))
        {
            EnterIdleState();
            return;
        }

        rb.linearVelocity = moveDirection * moveSpeed;
        UpdateFacing();
    }

    protected virtual void EnterIdleState()
    {
        currentState = MobState.Idle;
        stateTimer = Random.Range(idleDurationRange.x, idleDurationRange.y);
        rb.linearVelocity = Vector2.zero;
    }

    protected virtual void EnterWanderState()
    {
        moveDirection = PickNextDirection();
        if (moveDirection == Vector2.zero)
        {
            EnterIdleState();
            return;
        }

        currentState = MobState.Wandering;
        stateTimer = Random.Range(walkDurationRange.x, walkDurationRange.y);
        UpdateFacing();
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
        if (direction == Vector2.zero || walkableTilemap == null)
        {
            return false;
        }

        Vector3 lookAheadPosition = transform.position + (Vector3)(direction.normalized * directionCheckDistance);
        return IsWalkable(lookAheadPosition);
    }

    protected bool IsWalkable(Vector3 worldPosition)
    {
        Vector3Int cell = walkableTilemap.WorldToCell(worldPosition);
        if (!walkableTilemap.HasTile(cell))
        {
            return false;
        }

        return blockedTilemap == null || !blockedTilemap.HasTile(cell);
    }

    protected virtual void UpdateFacing()
    {
        if (visualRoot == null || Mathf.Abs(moveDirection.x) < 0.01f)
        {
            return;
        }

        Vector3 localScale = visualRoot.localScale;
        localScale.x = Mathf.Abs(localScale.x) * (moveDirection.x < 0f ? -1f : 1f);
        visualRoot.localScale = localScale;
    }

    public virtual void Interact(GameObject interactor)
    {
    }

    public void ConfigureNavigation(Tilemap walkable, Tilemap blocked)
    {
        walkableTilemap = walkable;
        blockedTilemap = blocked;

        if (!isNavigationConfigured && walkableTilemap != null)
        {
            isNavigationConfigured = true;
            EnterIdleState();
        }
    }
}