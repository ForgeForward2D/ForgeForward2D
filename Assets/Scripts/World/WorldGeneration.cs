using System.Collections.Generic;
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
    [SerializeField, Range(0f, 1f)] float woodChance;
    [SerializeField, Range(0f, 1f)] float saplingChance;
    [SerializeField, Range(0f, 1f)] float bigTreeChance;

    [SerializeField] int worldSeed;

    void Start()
    {
        if (worldSeed == 0)
        {
            worldSeed = System.Environment.TickCount;
        }
        Debug.Log($"Using Seed {worldSeed}");
        Random.InitState(worldSeed);

        TileMapManager tileMapManager = GetComponent<TileMapManager>();

        // Get initial bounds before generating terrain
        (int xMin, int xMax, int yMin, int yMax) initialBounds = tileMapManager.GetBounds();

        // Generate a 100x100 Perlin noise terrain to the left
        int terrainWidth = 100;
        int terrainHeight = 100;
        int terrainStartX = initialBounds.xMin - terrainWidth;
        int terrainStartY = initialBounds.yMin;

        BlockType clayBlock = BlockTypeRepository.GetBlockById(9);
        BlockType stoneBlock = BlockTypeRepository.GetBlockById(1);
        BlockType waterBlock = BlockTypeRepository.GetBlockById(6);
        BlockType diamondBlock = BlockTypeRepository.GetBlockById(3);
        BlockType bricksBlock = BlockTypeRepository.GetBlockById(7);
        BlockType woodBlock = BlockTypeRepository.GetBlockById(5);
        BlockType saplingBlock = BlockTypeRepository.GetBlockById(4);

        float noiseOffsetX = (worldSeed % 10000) + 0.5f;
        float noiseOffsetY = (worldSeed / 10000 % 10000) + 0.5f;
        HashSet<Vector2Int> treePositions = new HashSet<Vector2Int>();

        for (int x = terrainStartX; x < terrainStartX + terrainWidth; x++)
        {
            for (int y = terrainStartY; y < terrainStartY + terrainHeight; y++)
            {
                float noiseValue = Mathf.PerlinNoise(
                    (x + noiseOffsetX) * noiseScale,
                    (y + noiseOffsetY) * noiseScale
                );

                Vector3Int tilePos = new Vector3Int(x, y, 0);

                if (noiseValue < 0.15f)
                {
                    // water
                    wallTilemap.SetTile(tilePos, waterBlock.tile);
                }
                else if (noiseValue < 0.2f)
                {
                    // clay
                    wallTilemap.SetTile(tilePos, clayBlock.tile);
                }
                else if (noiseValue < 0.6f)
                {
                    // air / earth — randomly place wood or saplings
                    float roll = Random.value;
                    if (roll < woodChance && !IsAdjacentToTree(treePositions, x, y))
                    {
                        if (Random.value < bigTreeChance
                            && x + 1 < terrainStartX + terrainWidth
                            && y + 1 < terrainStartY + terrainHeight)
                        {
                            // Place a 2x2 tree
                            PlaceWood(wallTilemap, woodBlock, treePositions, x, y);
                            PlaceWood(wallTilemap, woodBlock, treePositions, x + 1, y);
                            PlaceWood(wallTilemap, woodBlock, treePositions, x, y + 1);
                            PlaceWood(wallTilemap, woodBlock, treePositions, x + 1, y + 1);
                        }
                        else
                        {
                            PlaceWood(wallTilemap, woodBlock, treePositions, x, y);
                        }
                    }
                    else if (roll < woodChance + saplingChance && !IsAdjacentToTree(treePositions, x, y))
                    {
                        walkableTilemap.SetTile(tilePos, saplingBlock.tile);
                        treePositions.Add(new Vector2Int(x, y));
                    }
                }
                else if (noiseValue < 0.7f)
                {
                    // stone
                    wallTilemap.SetTile(tilePos, stoneBlock.tile);
                }
                else if (noiseValue < 0.71f)
                {
                    // dia ore
                    wallTilemap.SetTile(tilePos, diamondBlock.tile);
                }
                else
                {
                    // bricks
                    wallTilemap.SetTile(tilePos, bricksBlock.tile);
                }
            }
        }

        // Recalculate bounds after terrain generation
        (int xMin, int xMax, int yMin, int yMax) mapBounds = tileMapManager.GetBounds();
        Debug.Log("Set world borders to x: (" + mapBounds.xMin + ", " + mapBounds.xMax + "), y: (" + mapBounds.yMin + ", " + mapBounds.yMax + ")");

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

    bool IsAdjacentToTree(HashSet<Vector2Int> treePositions, int x, int y)
    {
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                if (dx == 0 && dy == 0) continue;
                if (treePositions.Contains(new Vector2Int(x + dx, y + dy)))
                    return true;
            }
        }
        return false;
    }

    void PlaceWood(Tilemap tilemap, BlockType woodBlock, HashSet<Vector2Int> treePositions, int x, int y)
    {
        Vector3Int tilePos = new Vector3Int(x, y, 0);
        tilemap.SetTile(tilePos, woodBlock.tile);
        treePositions.Add(new Vector2Int(x, y));
    }
}
