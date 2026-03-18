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
    [SerializeField] Tilemap animationTilemap;
    [SerializeField] Tilemap portalTilemap;

    [Header("World Generation")]
    [SerializeField] float noiseScale;
    [SerializeField] int borderWaves = 8;

    [SerializeField] int worldSeed;

    [Header("Levels")]
    [SerializeField] public Level[] levels;

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

        // Draw border around starter level
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

        foreach (var level in levels)
        {
            generateLevel(level);
        }
    }

    void generateLevel(Level level)
    {
        Vector2Int startingPoint = level.startingPoint;
        int levelSize = level.levelSize;
        float baseRadius = levelSize / 2f;

        PerlinMapping[] blockMapping = level.blockMapping.OrderBy(m => m.threshold).ToArray();

        float noiseOffsetX = (worldSeed % 10000) + 0.5f;
        float noiseOffsetY = (worldSeed / 10000 % 10000) + 0.5f;
        HashSet<Vector2Int> streuselPositions = new HashSet<Vector2Int>();

        // Generate random wave parameters for the irregular border
        int N = borderWaves;
        float[] amps = new float[N];
        float[] phases = new float[N];
        for (int i = 0; i < N; i++)
        {
            amps[i] = Random.Range(0f, 1f / (2f * N));
            phases[i] = Random.Range(0f, 2f * Mathf.PI);
        }

        // Precompute the border shape: for each tile, determine if inside
        // Max normalized radius is 1 + sum(amps), scan area must cover the full possible extent
        float maxAmpSum = 0f;
        for (int i = 0; i < N; i++) maxAmpSum += amps[i];
        int scanRadius = Mathf.CeilToInt(baseRadius * (1f + maxAmpSum)) + 2;

        int xStart = startingPoint.x - scanRadius;
        int xEnd = startingPoint.x + scanRadius;
        int yStart = startingPoint.y - scanRadius;
        int yEnd = startingPoint.y + scanRadius;

        HashSet<Vector2Int> interiorTiles = new HashSet<Vector2Int>();
        HashSet<Vector2Int> borderTiles = new HashSet<Vector2Int>();

        for (int x = xStart; x <= xEnd; x++)
        {
            for (int y = yStart; y <= yEnd; y++)
            {
                float dx = x - startingPoint.x;
                float dy = y - startingPoint.y;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                float angle = Mathf.Atan2(dy, dx);

                float normalizedRadius = 1f;
                for (int i = 0; i < N; i++)
                {
                    normalizedRadius += amps[i] * Mathf.Cos((i + 1) * angle + phases[i]);
                }
                float radius = baseRadius * normalizedRadius;

                if (dist < radius - 1f)
                {
                    interiorTiles.Add(new Vector2Int(x, y));
                }
                else if (dist < radius + 1f)
                {
                    borderTiles.Add(new Vector2Int(x, y));
                }
            }
        }

        // Remove border tiles that are also interior
        borderTiles.ExceptWith(interiorTiles);

        // Place interior tiles using Perlin noise
        foreach (var tile in interiorTiles)
        {
            int x = tile.x;
            int y = tile.y;

            float noiseValue = Mathf.PerlinNoise(
                (x + noiseOffsetX) * noiseScale,
                (y + noiseOffsetY) * noiseScale
            );

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

            backgroundTilemap.SetTile(tilePos, backgroundTile);
        }

        // Place border wall tiles
        foreach (var tile in borderTiles)
        {
            Vector3Int tilePos = new Vector3Int(tile.x, tile.y, 0);
            wallTilemap.SetTile(tilePos, wallTile);
        }

        // Place portal at the level entry
        Vector3Int entryPos = new Vector3Int(startingPoint.x, startingPoint.y, 0);
        portalTilemap.SetTile(entryPos, level.portalBlock.tile);
        wallTilemap.SetTile(entryPos, null);
        backgroundTilemap.SetTile(entryPos, backgroundTile);
    }

    void PlaceStreusel(Tilemap tilemap, BlockType streuselBlock, HashSet<Vector2Int> streuselPositions, int x, int y)
    {
        Vector3Int tilePos = new Vector3Int(x, y, 0);
        tilemap.SetTile(tilePos, streuselBlock.tile);
        streuselPositions.Add(new Vector2Int(x, y));
    }
}
