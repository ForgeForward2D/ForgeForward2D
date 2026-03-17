using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Tilemaps;

public class MobSpawner : MonoBehaviour
{
    private static readonly Collider2D[] OccupancyResults = new Collider2D[8];
    private static readonly ContactFilter2D OccupancyContactFilter = new ContactFilter2D();

    [Header("Spawn Setup")]
    [SerializeField] private List<MobType> mobTypes = new();
    [SerializeField] private Tilemap spawnTilemap;
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
        if (!Application.isPlaying)
        {
            Debug.LogWarning("MobSpawner: 'Spawn Mobs' can only be used in Play Mode.", this);
            return;
        }

        if (mobTypes.Count == 0)
        {
            Debug.LogWarning("MobSpawner has no mob types assigned.", this);
            return;
        }

        List<MobType> validMobTypes = GetValidMobTypes();
        if (validMobTypes.Count == 0)
        {
            Debug.LogWarning("MobSpawner has no valid mob types assigned.", this);
            return;
        }

        if (clearExistingMobsBeforeSpawn)
        {
            ClearChildren(transform);
        }

        List<Vector2Int> availableCoordinates = GetAvailableSpawnCoordinates(transform);
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

            MobType selectedMobType = validMobTypes[Random.Range(0, validMobTypes.Count)];
            GameObject selectedPrefab = selectedMobType.prefab;
            Vector3 spawnPosition = spawnTilemap.GetCellCenterWorld(new Vector3Int(coordinate.x, coordinate.y, 0));
            GameObject mobInstance = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity, transform);

            MobController mobController = mobInstance.GetComponent<MobController>();
            if (mobController != null)
            {
                mobController.SetMobType(selectedMobType);
                mobController.ConfigureNavigation();
            }
            else
            {
                Debug.LogWarning($"Spawned mob prefab '{selectedPrefab.name}' is missing a MobController component.", selectedPrefab);
            }
        }
    }

    private List<MobType> GetValidMobTypes()
    {
        List<MobType> validTypes = new();

        for (int index = 0; index < mobTypes.Count; index++)
        {
            MobType type = mobTypes[index];
            if (type == null)
            {
                continue;
            }

            if (type.prefab == null)
            {
                Debug.LogWarning($"MobSpawner mob type '{type.name}' has no prefab assigned and will be ignored.", type);
                continue;
            }

            validTypes.Add(type);
        }

        return validTypes;
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
        TileMapManager tileMapManager = TileMapManager.Instance;
        if (tileMapManager == null)
        {
            Debug.LogWarning("MobSpawner could not find TileMapManager instance for walkability checks.", this);
            return new List<Vector2Int>();
        }

        HashSet<Vector2Int> occupiedCoordinates = GetOccupiedMobCoordinates(parent);
        List<Vector2Int> coordinates = new();

        spawnTilemap.CompressBounds();
        BoundsInt bounds = spawnTilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector2Int coordinate = new Vector2Int(x, y);
                if (!spawnTilemap.HasTile(new Vector3Int(x, y, 0)))
                {
                    continue;
                }

                if (!tileMapManager.Traversable(coordinate))
                {
                    continue;
                }

                if (IsCellOccupied(coordinate, parent))
                {
                    continue;
                }

                if (occupiedCoordinates.Contains(coordinate))
                {
                    continue;
                }

                coordinates.Add(coordinate);
            }
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

    private bool IsCellOccupied(Vector2Int coordinate, Transform parent)
    {
        Vector3 worldCenter = spawnTilemap.GetCellCenterWorld(new Vector3Int(coordinate.x, coordinate.y, 0));
        int count = Physics2D.OverlapPoint(worldCenter, OccupancyContactFilter, OccupancyResults);
        for (int index = 0; index < count; index++)
        {
            Collider2D collider = OccupancyResults[index];
            if (collider == null) { continue; }
            if (collider.isTrigger) { continue; }
            if (parent != null && collider.transform.IsChildOf(parent)) { continue; }
            if (collider.transform == transform) { continue; }
            return true;
        }
        return false;
    }

}