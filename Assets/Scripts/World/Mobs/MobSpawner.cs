using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Tilemaps;

public class MobSpawner : MonoBehaviour
{
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

            GameObject selectedPrefab = mobPrefabs[Random.Range(0, mobPrefabs.Count)];
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

    private static void ClearChildren(Transform parent)
    {
        for (int index = parent.childCount - 1; index >= 0; index--)
        {
            Transform child = parent.GetChild(index);
            child.SetParent(null, true);
            child.gameObject.SetActive(false);
            Destroy(child.gameObject);
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

}