using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Scriptable Objects/Level")]
public class Level : ScriptableObject
{
    public string levelName;
    public Vector2Int startingPoint;
    public BlockType portalBlock;
    public BlockType borderBlock;
    public BlockType decorationAroundBorder;
    // public Vector2Int generationDirection; // TODO: make this a selection list from north, south, east, west
    public int levelSize; // ~radius
    public (int xMin, int xMax, int yMin, int yMax) bounds;
    public PerlinMapping[] blockMapping;
}

[Serializable]
public class PerlinMapping
{
    public BlockType block;
    [Range(0f, 1f)] public float threshold;
    public Streusel streusel; // TODO: make optional
}

[Serializable]
public class Streusel
{
    public BlockType block;
    [Range(0f, 1f)] public float probability;
}
