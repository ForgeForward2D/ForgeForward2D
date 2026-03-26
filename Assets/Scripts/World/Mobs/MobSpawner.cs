using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class MobSpawner : MonoBehaviour
{
    private static Collider2D[] OccupancyResults = new Collider2D[8];
    private static ContactFilter2D OccupancyContactFilter = new ContactFilter2D();

    [SerializeField] private bool spawnOnStart = true;
    [SerializeField] private bool clearExistingMobsBeforeSpawn = true;
    private bool ready = false;

    public void SetReady()
    {
        this.ready = true;
    }

    private void Start()
    {
        if (spawnOnStart)
        {
            StartCoroutine(SpawnWhenReady());
        }
    }

    private IEnumerator SpawnWhenReady()
    {
        while (!ready)
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

        var levels = FindAnyObjectByType<WorldGeneration>().Levels;

        if (levels == null || levels.Length == 0)
        {
            Debug.LogWarning("MobSpawner has no levels assigned.", this);
            return;
        }

        if (clearExistingMobsBeforeSpawn)
        {
            ClearChildren();
        }

        foreach (Level level in levels)
        {
            SpawnMobsForLevel(level);
        }
    }

    private void SpawnMobsForLevel(Level level)
    {
        if (level.mobSpawns == null || level.mobSpawns.Length == 0)
            return;

        List<MobSpawnEntry> validEntries = GetValidSpawnEntries(level.mobSpawns);
        if (validEntries.Count == 0)
        {
            Debug.LogWarning($"Level '{level.levelName}' has no valid mob spawn entries.", level);
            return;
        }

        float totalWeight = 0f;
        foreach (var entry in validEntries)
            totalWeight += entry.spawnWeight;

        List<Vector2Int> availableCoordinates = GetAvailableSpawnCoordinates(level);
        if (availableCoordinates.Count == 0)
        {
            Debug.LogWarning($"Level '{level.levelName}' has no valid spawn positions.", level);
            return;
        }

        int actualMobCount = level.mobCount;
        float reductionPercentage = 0f;
        NpcController npc = FindAnyObjectByType<NpcController>();

        if (npc != null && npc.reduceSpawn)
        {
            // Reduce spawns by 20% per sword tier (e.g. Diamond tier 4 = 80% reduction)
            reductionPercentage = npc.swordLevel * 0.20f;
        }
        actualMobCount = Mathf.RoundToInt(actualMobCount * (1f - reductionPercentage));

        Debug.Log($"Level '{level.levelName}' mob count: {level.mobCount} reduced to {actualMobCount}");

        for (int index = 0; index < actualMobCount; index++)
        {
            if (availableCoordinates.Count == 0)
            {
                Debug.LogWarning($"Level '{level.levelName}' ran out of spawn positions after {index} mobs.", level);
                break;
            }

            int randomIndex = Random.Range(0, availableCoordinates.Count);
            Vector2Int coordinate = availableCoordinates[randomIndex];
            availableCoordinates[randomIndex] = availableCoordinates[availableCoordinates.Count - 1];
            availableCoordinates.RemoveAt(availableCoordinates.Count - 1);

            MobType selectedMobType = PickWeighted(validEntries, totalWeight);
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

    private static MobType PickWeighted(List<MobSpawnEntry> entries, float totalWeight)
    {
        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;
        for (int i = 0; i < entries.Count; i++)
        {
            cumulative += entries[i].spawnWeight;
            if (roll < cumulative)
                return entries[i].mobType;
        }
        return entries[entries.Count - 1].mobType;
    }

    private List<MobSpawnEntry> GetValidSpawnEntries(MobSpawnEntry[] entries)
    {
        List<MobSpawnEntry> valid = new();
        foreach (var entry in entries)
        {
            if (entry.mobType == null || entry.mobType.prefab == null)
            {
                Debug.LogWarning($"MobSpawnEntry for mob type '{(entry.mobType == null ? "null" : entry.mobType.name)}' is invalid.");
                continue;
            }
            valid.Add(entry);
        }
        return valid;
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

    private List<Vector2Int> GetAvailableSpawnCoordinates(Level level)
    {
        HashSet<Vector2Int> occupiedCoordinates = GetOccupiedMobCoordinates(transform);
        List<Vector2Int> spawnablePositions = TileMapManager.Instance.GetSpawnablePositions(level);
        List<Vector2Int> coordinates = new();

        foreach (Vector2Int coordinate in spawnablePositions)
        {
            if (!TileMapManager.Instance.Walkable(coordinate))
            {
                continue;
            }

            if (IsCellOccupied(coordinate, transform))
            {
                continue;
            }

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
