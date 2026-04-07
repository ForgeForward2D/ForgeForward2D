using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
public class PortalManager : MonoBehaviour
{
    public static event Action<(Level, Vector3)> OnPlayerTeleport;

    private WorldGeneration worldGeneration;
    private Tilemap foregroundTilemap;
    private Level[] levels;

    private Dictionary<Vector2Int, Level> portalLevels = new Dictionary<Vector2Int, Level>();
    private Dictionary<Vector2Int, Vector2Int> portalDestinations = new Dictionary<Vector2Int, Vector2Int>();
    private Vector2Int currentPortalTile;
    private Vector2Int portalEntered;

    void Start()
    {
        Debug.Log("PortalManager: Start called");

        worldGeneration = FindAnyObjectByType<WorldGeneration>();
        if (worldGeneration == null)
        {
            Debug.LogError("PortalManager: worldGeneration is not assigned!");
            return;
        }

        levels = worldGeneration.Levels;
        foregroundTilemap = TileMapManager.Instance.ForegroundTilemap;

        // Link portals: levels sharing the same portalBlock are paired
        var portalBlocks = new List<BlockType>();
        var portalsByBlock = new Dictionary<BlockType, List<Vector2Int>>();
        var colliderPositions = new HashSet<Vector2Int>();

        foreach (var level in levels)
        {
            if (level.portalBlock == null)
            {
                Debug.LogWarning($"Portal: No portal block set for {level.levelName}");
                continue;
            }
            portalBlocks.Add(level.portalBlock);

            if (!portalsByBlock.ContainsKey(level.portalBlock))
            {
                portalsByBlock[level.portalBlock] = new List<Vector2Int>();
            }
            portalsByBlock[level.portalBlock].Add(level.startingPoint);
            portalLevels[level.startingPoint] = level;


            if (colliderPositions.Add(level.startingPoint))
            {
                CreateColliderAroundPortal(level.startingPoint);
            }
        }
        // search the portal map for portalBlocks and add the portals respectively to the portalsByBlock dict
        foregroundTilemap.CompressBounds();
        BoundsInt bounds = foregroundTilemap.cellBounds;
        foreach (Vector3Int cellPos in bounds.allPositionsWithin)
        {
            TileBase tile = foregroundTilemap.GetTile(cellPos);
            if (tile == null) continue;

            BlockType blockType = BlockTypeRepository.GetBlockByTile(tile);
            if (blockType == null || !portalsByBlock.ContainsKey(blockType)) continue;

            Vector2Int pos = new Vector2Int(cellPos.x, cellPos.y);
            if (!portalsByBlock[blockType].Contains(pos))
            {
                portalsByBlock[blockType].Add(pos);
                portalLevels[pos] = worldGeneration.BaseLevel;
            }

            if (colliderPositions.Add(pos))
            {
                CreateColliderAroundPortal(pos);
            }
        }

        foreach (var pair in portalsByBlock)
        {
            var positions = pair.Value;
            if (positions.Count < 2)
            {
                Debug.LogWarning($"Portal: every portal-network should have at least 2 portals (for portal: {pair.Key.displayName}");
                continue;
            }
            for (int i = 0; i < positions.Count; i++)
            {
                Vector2Int destination = positions[(i + 1) % positions.Count];
                portalDestinations[positions[i]] = destination;
                Debug.Log($"Portal: Mapped {positions[i]} -> {destination}");
            }
        }
    }

    void CreateColliderAroundPortal(Vector2Int portalPos)
    {
        Vector3Int cellPos = new Vector3Int(portalPos.x, portalPos.y, 0);
        Vector3 worldCenter = foregroundTilemap.GetCellCenterWorld(cellPos);
        Vector3 localCenter = transform.InverseTransformPoint(worldCenter);
        BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
        collider.offset = new Vector2(localCenter.x, localCenter.y);
        collider.size = foregroundTilemap.cellSize;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Vector2Int currentTile = TileMapManager.Instance.PositionToCoordinate(other.transform.position);
        TileBase tile = foregroundTilemap.GetTile(new Vector3Int(currentTile.x, currentTile.y, 0));
        if (tile != null)
        {
            // player is already on a portal tile (probably just teleported here)
            return;
        }
        portalEntered = currentTile;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        Vector2Int tile = TileMapManager.Instance.PositionToCoordinate(other.transform.position);
        if (!IsAdjacent(portalEntered, tile) || (portalEntered.x == tile.x && portalEntered.y == tile.y))
        {
            return;
        }
        TileBase portalTile = foregroundTilemap.GetTile(new Vector3Int(tile.x, tile.y, 0));

        if (portalTile == null)
        {
            Debug.LogWarning($"Portal: stopped — no portal tile at {tile}");
            return;
        }

        currentPortalTile = tile;
        if (portalDestinations.TryGetValue(currentPortalTile, out Vector2Int destination))
        {
            Vector3 worldPos = foregroundTilemap.GetCellCenterWorld(new Vector3Int(destination.x, destination.y, 0));
            if (portalLevels.TryGetValue(destination, out Level destinationLevel))
            {
                OnPlayerTeleport?.Invoke((destinationLevel, worldPos));
                string levelName = destinationLevel.levelName;
                Debug.Log($"Portal: teleported player to {destination} in level {levelName}");
            }
            else
            {
                Debug.LogWarning($"Portal: teleported player to {destination} (no level found)");
            }
            portalEntered = new Vector2Int(Int32.MinValue, Int32.MinValue);
        }
        else
        {
            Debug.LogWarning($"Portal: stopped — no destination mapped for tile {currentPortalTile}");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;
        portalEntered = new Vector2Int(Int32.MinValue, Int32.MinValue);
    }

    bool IsAdjacent(Vector2Int a, Vector2Int b)
    {
        return b.x - 1 <= a.x && a.x <= b.x + 1 && b.y - 1 <= a.y && a.y <= b.y + 1;
    }
}
