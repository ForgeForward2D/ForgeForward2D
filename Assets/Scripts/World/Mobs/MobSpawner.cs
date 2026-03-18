using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class MobSpawner : MonoBehaviour
{
    private static readonly Collider2D[] OccupancyResults = new Collider2D[8];
    private static readonly ContactFilter2D OccupancyContactFilter = new ContactFilter2D();

    [Header("Spawn Setup")]
    [SerializeField] private List<MobType> mobTypes = new();
    [SerializeField] private bool spawnOnStart = true;
    [SerializeField] private bool clearExistingMobsBeforeSpawn = true;

    [Header("Spawn Settings")]
    [SerializeField] private int mobCount = 4;
    [SerializeField] private int startupSpawnWaitFrames = 5;

    private void Start()
    {
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
            ClearChildren();
        }

        List<Vector2Int> availableCoordinates = GetAvailableSpawnCoordinates(transform);
        if (availableCoordinates.Count == 0)
        {
            Debug.LogWarning("MobSpawner found no valid spawn positions in the world.", this);
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
            Vector3 spawnPosition = TileMapManager.Instance.CoordinateToPosition(coordinate);
            GameObject mobInstance = Instantiate(selectedPrefab, spawnPosition, Quaternion.identity, transform);

            MobController mobController = mobInstance.GetComponent<MobController>();
            if (mobController != null)
            {
                mobController.SetMobType(selectedMobType);
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

    private void ClearChildren()
    {
        for (int index = transform.childCount - 1; index >= 0; index--)
        {
            Transform child = transform.GetChild(index);
            child.SetParent(null, true);
            child.gameObject.SetActive(false);
            Destroy(child.gameObject);
        }
    }

    private List<Vector2Int> GetAvailableSpawnCoordinates(Transform parent)
    {
        HashSet<Vector2Int> occupiedCoordinates = GetOccupiedMobCoordinates(parent);
        List<Vector2Int> coordinates = new();

        (int xMin, int xMax, int yMin, int yMax) = TileMapManager.Instance.GetBounds();

        for (int x = xMin; x < xMax; x++)
        {
            for (int y = yMin; y < yMax; y++)
            {
                Vector2Int coordinate = new Vector2Int(x, y);

                if (!TileMapManager.Instance.Walkable(coordinate))
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
            Vector2Int coordinate = TileMapManager.Instance.PositionToCoordinate(parent.GetChild(index).position);
            occupied.Add(coordinate);
        }

        return occupied;
    }

    private bool IsCellOccupied(Vector2Int coordinate, Transform parent)
    {
        Vector3 worldCenter = TileMapManager.Instance.CoordinateToPosition(coordinate);
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