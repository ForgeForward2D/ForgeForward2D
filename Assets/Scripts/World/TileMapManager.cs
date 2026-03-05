
using System;

using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapManager : MonoBehaviour
{
    [SerializeField] GameConfig gameConfig;

    [SerializeField] Tilemap wallTilemap;
    [SerializeField] Tilemap walkableTilemap;

    public void DrawBlock(BlockType blockType, Vector2Int position)
    {
        if (blockType == null)
        {
            blockType = BlockTypeRepository.GetBlockById(0); // Default to air block if null
        }

        Vector3Int tilePosition = new Vector3Int(position.x, position.y, 0);

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
        Debug.Assert(walkableTilemap.WorldToCell(worldPosition) == cellPosition, "Expected both tilemaps to return the same cell position for the same world position, but got " + walkableTilemap.WorldToCell(worldPosition) + " and " + cellPosition);

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
                Debug.Log("No block found for wall tile at " + position);
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
                Debug.Log("No block found for walkable tile at " + position);
            }
        }
        return null;
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
}