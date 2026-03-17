using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Tilemaps;

public class MobSpawner : MonoBehaviour
{
    private static readonly Collider2D[] OccupancyResults = new Collider2D[8];
    private static readonly ContactFilter2D OccupancyContactFilter = new ContactFilter2D();

    [Header("Spawn Setup")]
    [SerializeField] private List<GameObject> mobPrefabs = new();
    [SerializeField] private Transform mobContainer;
    [SerializeField] private Tilemap spawnTilemap;
    [SerializeField] private Tilemap blockedTilemap;
    [SerializeField] private bool spawnOnStart = true;
    [SerializeField] private bool clearExistingMobsBeforeSpawn = true;

    [Header("Spawn Settings")]
    [SerializeField] private int mobCount = 4;
    [SerializeField] private int startupSpawnWaitFrames = 5;

    private void Start()
    {
        if (spawnTilemap == null)
        {
            Debug.LogError("MobSpawner requires an assigned spawn Tilemap reference.", this);
            enabled = false;
            return;
        }

        if (spawnOnStart)
        {
            StartCoroutine(SpawnWhenReady());
        }
    }

    private IEnumerator SpawnWhenReady()
    {
        for (int frame = 0; frame < startupSpawnWaitFrames; frame++)
        {
            yield return null;
        }

        SpawnMobs();
    }

    [ContextMenu("Spawn Mobs")]
    public void SpawnMobs()
    {
        if (mobPrefabs.Count == 0)
        {
            Debug.LogWarning("MobSpawner has no mob prefabs assigned.", this);
            return;
        }

        Transform parent = mobContainer != null ? mobContainer : transform;
        List<GameObject> validMobPrefabs = GetValidMobPrefabs();
        if (validMobPrefabs.Count == 0)
        {
            Debug.LogWarning("MobSpawner has no valid mob prefabs assigned.", this);
            return;
        }

        if (clearExistingMobsBeforeSpawn)
        {
            ClearChildren(parent);
        }

        List<Vector2Int> availableCoordinates = GetAvailableSpawnCoordinates(parent);
        if (availableCoordinates.Count == 0)
        {
            Debug.LogWarning("MobSpawner found no valid tile cells on the assigned spawn tilemap.", this);
            return;
        }

        for (int index = 0; index < mobCount; index++)
        {
            if (availableCoordinates.Count == 0)
            {
                Debug.LogWarning("MobSpawner could not find any more valid spawn positions.", this);
                break;
            }

            int randomIndex = Random.Range(0, availableCoordinates.Count);
            Vector2Int coordinate = availableCoordinates[randomIndex];
            availableCoordinates[randomIndex] = availableCoordinates[availableCoordinates.Count - 1];
            availableCoordinates.RemoveAt(availableCoordinates.Count - 1);

            GameObject selectedPrefab = validMobPrefabs[Random.Range(0, validMobPrefabs.Count)];
            Vector3Int tilePosition = new Vector3Int(coordinate.x, coordinate.y, 0);
            Vector3 spawnPosition = spawnTilemap.GetCellCenterWorld(tilePosition);
            GameObject mobInstance = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity, parent);

            MobController mobController = mobInstance.GetComponent<MobController>();
            if (mobController != null)
            {
                mobController.ConfigureNavigation(spawnTilemap, blockedTilemap);
            }
            else
            {
                Debug.LogWarning($"Spawned mob prefab '{selectedPrefab.name}' is missing a MobController component.", selectedPrefab);
            }
        }
    }

    private List<GameObject> GetValidMobPrefabs()
    {
        List<GameObject> validPrefabs = new();
        List<int> nullPrefabIndices = new();

        for (int index = 0; index < mobPrefabs.Count; index++)
        {
            GameObject prefab = mobPrefabs[index];
            if (prefab != null)
            {
                validPrefabs.Add(prefab);
            }
            else
            {
                nullPrefabIndices.Add(index);
            }
        }

        if (nullPrefabIndices.Count > 0)
        {
            string nullSlots = string.Join(", ", nullPrefabIndices);
            Debug.LogWarning($"MobSpawner has unassigned mob prefab slots at indices: {nullSlots}. These entries will be ignored.", this);
        }

        return validPrefabs;
    }

    private static void ClearChildren(Transform parent)
    {
        for (int index = parent.childCount - 1; index >= 0; index--)
        {
            Transform child = parent.GetChild(index);
            if (Application.isPlaying)
            {
                child.SetParent(null, true);
                child.gameObject.SetActive(false);
                Destroy(child.gameObject);
            }
            else
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }

    private List<Vector2Int> GetAvailableSpawnCoordinates(Transform parent)
    {
        HashSet<Vector2Int> occupiedCoordinates = GetOccupiedMobCoordinates(parent);
        List<Vector2Int> coordinates = new();

        spawnTilemap.CompressBounds();
        BoundsInt bounds = spawnTilemap.cellBounds;

        foreach (Vector3Int cell in bounds.allPositionsWithin)
        {
            if (!spawnTilemap.HasTile(cell))
            {
                continue;
            }

            if (blockedTilemap != null && blockedTilemap.HasTile(cell))
            {
                continue;
            }

            if (IsCellOccupiedByCollider(cell, parent))
            {
                continue;
            }

            Vector2Int coordinate = new Vector2Int(cell.x, cell.y);
            if (occupiedCoordinates.Contains(coordinate))
            {
                continue;
            }

            coordinates.Add(coordinate);
        }

        return coordinates;
    }

    private HashSet<Vector2Int> GetOccupiedMobCoordinates(Transform parent)
    {
        HashSet<Vector2Int> occupied = new();
        for (int index = 0; index < parent.childCount; index++)
        {
            Vector3Int cell = spawnTilemap.WorldToCell(parent.GetChild(index).position);
            occupied.Add(new Vector2Int(cell.x, cell.y));
        }

        return occupied;
    }

    private bool IsCellOccupiedByCollider(Vector3Int cell, Transform parent)
    {
        Vector3 worldCenter = spawnTilemap.GetCellCenterWorld(cell);
        return IsWorldPositionOccupied(worldCenter, parent);
    }

    private bool IsWorldPositionOccupied(Vector3 worldPosition, Transform parent)
    {
        int count = Physics2D.OverlapPoint(worldPosition, OccupancyContactFilter, OccupancyResults);
        for (int index = 0; index < count; index++)
        {
            Collider2D collider = OccupancyResults[index];
            if (collider == null)
            {
                continue;
            }

            if (collider.isTrigger)
            {
                continue;
            }

            if (parent != null && collider.transform.IsChildOf(parent))
            {
                continue;
            }

            if (collider.transform == transform)
            {
                continue;
            }

            return true;
        }

        return false;
    }

}