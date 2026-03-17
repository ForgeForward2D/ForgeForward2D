using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGeneration : MonoBehaviour
{
    [SerializeField] Tilemap backgroundTilemap;
    [SerializeField] TileBase backgroundTile;

    [SerializeField] Tilemap wallTilemap;
    [SerializeField] TileBase wallTile;
    [SerializeField] Tilemap walkableTilemap;

    [Header("World Generation")]
    [SerializeField] float noiseScale;

    [SerializeField] int worldSeed;

    [Header("Levels")]
    [SerializeField] Level[] levels;

    private TileMapManager tileMapManager;

    void Start()
    {
        if (worldSeed == 0)
        {
            worldSeed = System.Environment.TickCount;
        }
        Debug.Log($"Using Seed {worldSeed}");
        Random.InitState(worldSeed);

        tileMapManager = TileMapManager.Instance;

        foreach (var level in levels)
        {
            generateLevel(level);
        }

        // Recalculate bounds after terrain generation
        (int xMin, int xMax, int yMin, int yMax) mapBounds = tileMapManager.GetBounds();
        Debug.Log($"Set world borders to x: ({mapBounds.xMin}, {mapBounds.xMax}), y: ({mapBounds.yMin}, {mapBounds.yMax})");

        for (int y = mapBounds.yMin - 1; y <= mapBounds.yMax; y++)
        {
            // Left wall
            wallTilemap.SetTile(new Vector3Int(mapBounds.xMin - 1, y, 0), wallTile);
            // Right wall
            wallTilemap.SetTile(new Vector3Int(mapBounds.xMax, y, 0), wallTile);
        }
        for (int x = mapBounds.xMin; x < mapBounds.xMax; x++)
        {
            // Bottom wall
            wallTilemap.SetTile(new Vector3Int(x, mapBounds.yMin - 1, 0), wallTile);
            // Top wall
            wallTilemap.SetTile(new Vector3Int(x, mapBounds.yMax, 0), wallTile);

            for (int y = mapBounds.yMin; y < mapBounds.yMax; y++)
            {
                // Background
                backgroundTilemap.SetTile(new Vector3Int(x, y, 0), backgroundTile);
            }
        }
    }

    void generateLevel(Level level)
    {
        // (int xMin, int xMax, int yMin, int yMax) bounds = level.bounds;
        Vector2Int startingPoint = level.startingPoint;
        Vector2Int generationDirection = level.generationDirection;
        int levelSize = level.levelSize;

        PerlinMapping[] blockMapping = level.blockMapping.OrderBy(m => m.threshold).ToArray();

        (int xMin, int xMax, int yMin, int yMax) initialBounds = tileMapManager.GetBounds();

        float noiseOffsetX = (worldSeed % 10000) + 0.5f;
        float noiseOffsetY = (worldSeed / 10000 % 10000) + 0.5f;
        HashSet<Vector2Int> streuselPositions = new HashSet<Vector2Int>();

        int xStart, xEnd, xStep;
        if (generationDirection.x != 0)
        {
            xStart = startingPoint.x;
            xEnd = startingPoint.x + generationDirection.x * levelSize;
            xStep = generationDirection.x;
        }
        else
        {
            xStart = startingPoint.x;
            xEnd = startingPoint.x + 1;
            xStep = 1;
        }

        int yStart, yEnd, yStep;
        if (generationDirection.y != 0)
        {
            yStart = startingPoint.y;
            yEnd = startingPoint.y + generationDirection.y * levelSize;
            yStep = generationDirection.y;
        }
        else
        {
            yStart = startingPoint.y;
            yEnd = startingPoint.y + 1;
            yStep = 1;
        }

        for (int x = xStart; xStep > 0 ? x < xEnd : x > xEnd; x += xStep)
        {
            for (int y = yStart; yStep > 0 ? y < yEnd : y > yEnd; y += yStep)
            {
                float noiseValue = Mathf.PerlinNoise(
                    (x + noiseOffsetX) * noiseScale,
                    (y + noiseOffsetY) * noiseScale
                );
                Debug.Log($"{noiseValue}");

                Vector3Int tilePos = new Vector3Int(x, y, 0);

                foreach (var perlinMapping in blockMapping)
                {
                    BlockType block = perlinMapping.block;
                    float threshold = perlinMapping.threshold;
                    Streusel streusel = perlinMapping.streusel;

                    if (noiseValue < threshold)
                    {
                        if (streusel != null)
                        {
                            float roll = Random.value;
                            if (roll < streusel.probability)
                            {
                                PlaceStreusel(wallTilemap, streusel.block, streuselPositions, x, y);
                                break;
                            }
                        }
                        if (block == null)
                        {
                            break;
                        }
                        wallTilemap.SetTile(tilePos, block.tile);
                        break;
                    }
                }
            }
        }
    }

    // bool IsAdjacentToStreusel(HashSet<Vector2Int> streuselPositions, int x, int y)
    // {
    //     for (int dx = -1; dx <= 1; dx++)
    //     {
    //         for (int dy = -1; dy <= 1; dy++)
    //         {
    //             if (dx == 0 && dy == 0) continue;
    //             if (streuselPositions.Contains(new Vector2Int(x + dx, y + dy)))
    //                 return true;
    //         }
    //     }
    //     return false;
    // }

    void PlaceStreusel(Tilemap tilemap, BlockType streuselBlock, HashSet<Vector2Int> streuselPositions, int x, int y)
    {
        Vector3Int tilePos = new Vector3Int(x, y, 0);
        tilemap.SetTile(tilePos, streuselBlock.tile);
        streuselPositions.Add(new Vector2Int(x, y));
    }
}
