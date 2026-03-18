using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGeneration : MonoBehaviour
{

    [SerializeField] Tilemap backgroundTilemap;
    [SerializeField] TileBase backgroundTile;

    [SerializeField] BlockType wallBlock;
    [SerializeField] Tilemap portalTilemap;

    [Header("Settings")]
    [SerializeField] float noiseScale;
    [SerializeField] int borderWaves = 8;
    [SerializeField] float borderWidth = 0.6f;

    [SerializeField] int worldSeed;

    [SerializeField] public Level[] levels;

    void Start()
    {
        if (worldSeed == 0)
        {
            worldSeed = System.Environment.TickCount;
        }
        Debug.Log($"Using Seed {worldSeed}");
        Random.InitState(worldSeed);

        // Draw border around starter level
        (int xMin, int xMax, int yMin, int yMax) mapBounds = TileMapManager.Instance.GetBounds();
        Debug.Log($"Set world borders to x: ({mapBounds.xMin}, {mapBounds.xMax}), y: ({mapBounds.yMin}, {mapBounds.yMax})");

        for (int y = mapBounds.yMin - 1; y <= mapBounds.yMax; y++)
        {
            // Left wall
            TileMapManager.Instance.DrawBlock(wallBlock, new Vector2Int(mapBounds.xMin - 1, y));
            // Right wall
            TileMapManager.Instance.DrawBlock(wallBlock, new Vector2Int(mapBounds.xMax, y));
        }
        for (int x = mapBounds.xMin; x < mapBounds.xMax; x++)
        {
            // Bottom wall
            TileMapManager.Instance.DrawBlock(wallBlock, new Vector2Int(x, mapBounds.yMin - 1));
            // Top wall
            TileMapManager.Instance.DrawBlock(wallBlock, new Vector2Int(x, mapBounds.yMax));

            for (int y = mapBounds.yMin; y < mapBounds.yMax; y++)
            {
                // Background
                backgroundTilemap.SetTile(new Vector3Int(x, y, 0), backgroundTile);
            }
        }

        foreach (var level in levels)
        {
            GenerateLevel(level);
        }
    }

    void GenerateLevel(Level level)
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
        int scanRadius = Mathf.CeilToInt(baseRadius * (1f + maxAmpSum)) + 2 + 10;

        int xStart = startingPoint.x - scanRadius;
        int xEnd = startingPoint.x + scanRadius;
        int yStart = startingPoint.y - scanRadius;
        int yEnd = startingPoint.y + scanRadius;

        HashSet<Vector2Int> interiorTiles = new HashSet<Vector2Int>();
        HashSet<Vector2Int> borderTiles = new HashSet<Vector2Int>();
        HashSet<Vector2Int> paddingTiles = new HashSet<Vector2Int>();
        int paddingThickness = 10;

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

                if (dist < radius - borderWidth)
                {
                    interiorTiles.Add(new Vector2Int(x, y));
                }
                else if (dist < radius + borderWidth)
                {
                    borderTiles.Add(new Vector2Int(x, y));
                }
                else if (dist < radius + borderWidth + paddingThickness)
                {
                    paddingTiles.Add(new Vector2Int(x, y));
                }
            }
        }

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
                            PlaceStreusel(streusel.block, streuselPositions, new Vector2Int(x, y));
                            break;
                        }
                    }
                    if (block == null)
                    {
                        break;
                    }
                    TileMapManager.Instance.DrawBlock(block, new Vector2Int(x, y));
                    break;
                }
            }

            backgroundTilemap.SetTile(tilePos, backgroundTile);
        }

        // Place border wall tiles
        foreach (var tile in borderTiles)
        {
            Vector3Int tilePos = new Vector3Int(tile.x, tile.y, 0);
            TileMapManager.Instance.DrawBlock(level.wallBlock, tile);
            backgroundTilemap.SetTile(tilePos, level.paddingBlock.tile);
        }

        if (level.paddingBlock != null)
        {
            foreach (var tile in paddingTiles)
            {
                Vector3Int tilePos = new Vector3Int(tile.x, tile.y, 0);
                backgroundTilemap.SetTile(tilePos, level.paddingBlock.tile);
            }
        }

        Debug.Assert(level.portalBlock != null, $"No portal block set for level {level.levelName}");

        // Place portal at the level entry
        Vector3Int entryPos = new Vector3Int(startingPoint.x, startingPoint.y, 0);
        portalTilemap.SetTile(entryPos, level.portalBlock.tile);
        TileMapManager.Instance.DrawBlock(null, startingPoint);
        backgroundTilemap.SetTile(entryPos, backgroundTile);
    }

    void PlaceStreusel(BlockType streuselBlock, HashSet<Vector2Int> streuselPositions, Vector2Int pos)
    {
        TileMapManager.Instance.DrawBlock(streuselBlock, pos);
        streuselPositions.Add(pos);
    }
}
