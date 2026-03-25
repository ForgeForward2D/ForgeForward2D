using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Scriptable Objects/Level")]
public class Level : ScriptableObject
{
    public string levelName;
    public Vector2Int startingPoint;
    public TileBase backgroundTile;
    public BlockType portalBlock;
    public BlockType wallBlock;
    public TileBase paddingTile;
    public int levelSize; // ~radius
    public PerlinMapping[] blockMapping;

    public int mobCount = 4;
    public MobSpawnEntry[] mobSpawns;
}

[Serializable]
public class MobSpawnEntry
{
    public MobType mobType;
    [Min(0.01f)] public float spawnWeight = 1f;
}

[Serializable]
public class PerlinMapping
{
    public BlockType block;
    [Range(0f, 1f)] public float threshold;
    public Detail[] details;
}

[Serializable]
public class Detail
{
    public BlockType block;
    [Range(0f, 1f)] public float probability;
}
