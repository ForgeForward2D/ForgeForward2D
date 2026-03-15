using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapManager : MonoBehaviour
{
    public static TileMapManager Instance { get; private set; }

    [SerializeField] private Tilemap wallTilemap;
    [SerializeField] private Tilemap walkableTilemap;
    [SerializeField] private Tilemap animationTilemap;

    public static Action<(BlockType, Vector2Int)> OnBlockChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple TileMapManager instances detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void DrawBlock(BlockType blockType, Vector2Int position)
    {
        Vector3Int tilePosition = new Vector3Int(position.x, position.y, 0);

        if (blockType == null)
        {
            wallTilemap.SetTile(tilePosition, null);
            walkableTilemap.SetTile(tilePosition, null);
            OnBlockChanged?.Invoke((null, position));
            return;
        }

        if (blockType.walkable)
        {
            wallTilemap.SetTile(tilePosition, null);
            walkableTilemap.SetTile(tilePosition, blockType.tile);
        }
        else
        {
            wallTilemap.SetTile(tilePosition, blockType.tile);
            walkableTilemap.SetTile(tilePosition, null);
        }

        OnBlockChanged?.Invoke((blockType, position));
    }

    public void ClearTile(Vector2Int position)
    {
        Vector3Int tilePosition = new Vector3Int(position.x, position.y, 0);
        wallTilemap.SetTile(tilePosition, null);
        walkableTilemap.SetTile(tilePosition, null);
    }

    public Vector2Int PositionToCoordinate(Vector3 worldPosition)
    {
        Vector3Int cellPosition = wallTilemap.WorldToCell(worldPosition);

        Debug.Assert(cellPosition.z == 0, "Expected cell position z to be 0 but got " + cellPosition.z);
        Debug.Assert(
            walkableTilemap.WorldToCell(worldPosition) == cellPosition,
            "Expected both tilemaps to return the same cell position but got "
            + walkableTilemap.WorldToCell(worldPosition) + " and " + cellPosition
        );

        return new Vector2Int(cellPosition.x, cellPosition.y);
    }

    public BlockType GetBlockTypeAtPosition(Vector2Int position)
    {
        Vector3Int tilePosition = new Vector3Int(position.x, position.y, 0);

        TileBase wallTile = wallTilemap.GetTile(tilePosition);
        if (wallTile != null)
        {
            BlockType blockType = BlockTypeRepository.GetBlockByTile(wallTile);
            if (blockType != null)
            {
                Debug.Assert(!blockType.walkable);
                return blockType;
            }
            else
            {
                Debug.Log($"No block found for wall tile at {position}");
            }
        }

        TileBase walkableTile = walkableTilemap.GetTile(tilePosition);
        if (walkableTile != null)
        {
            BlockType blockType = BlockTypeRepository.GetBlockByTile(walkableTile);
            if (blockType != null)
            {
                Debug.Assert(blockType.walkable);
                return blockType;
            }
            else
            {
                Debug.Log($"No block found for walkable tile at {position}");
            }
        }

        return null;
    }

    public bool IsOccupied(Vector2Int pos)
    {
        Vector3Int cell = new Vector3Int(pos.x, pos.y, 0);
        Vector3 center = wallTilemap.GetCellCenterWorld(cell);

        Vector2 size = wallTilemap.cellSize * 0.9f;

        LayerMask mask = ~(1 << wallTilemap.gameObject.layer);

        return Physics2D.OverlapBox(center, size, 0f, mask) != null;
    }

    public (int, int, int, int) GetBounds()
    {
        walkableTilemap.CompressBounds();
        wallTilemap.CompressBounds();

        BoundsInt walkableTileBounds = walkableTilemap.cellBounds;
        BoundsInt wallTileBounds = wallTilemap.cellBounds;

        int yMin = Mathf.Min(walkableTileBounds.yMin, wallTileBounds.yMin);
        int yMax = Mathf.Max(walkableTileBounds.yMax, wallTileBounds.yMax);
        int xMin = Mathf.Min(walkableTileBounds.xMin, wallTileBounds.xMin);
        int xMax = Mathf.Max(walkableTileBounds.xMax, wallTileBounds.xMax);

        return (xMin, xMax, yMin, yMax);
    }

    public void UpdateBlockBreakingProgress(Vector2Int position, int stage)
    {
        Vector3Int tilePosition = new Vector3Int(position.x, position.y, 0);
        TileBase destroyTile = DestroyTileRepository.GetDestroyTile(stage);
        animationTilemap.SetTile(tilePosition, destroyTile);
    }
}