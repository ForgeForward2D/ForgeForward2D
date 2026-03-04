using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGeneration : MonoBehaviour
{
    [SerializeField] GameConfig gameConfig;

    [SerializeField] Tilemap backgroundTilemap;
    [SerializeField] TileBase backgroundTile;

    [SerializeField] Tilemap wallTilemap;
    [SerializeField] TileBase wallTile;

    void Start()
    {
        BoundsInt bounds = wallTilemap.cellBounds;
        gameConfig.world_width = bounds.size.x;
        gameConfig.world_height = bounds.size.y;
        Debug.Log("Set world size to (" + gameConfig.world_width + ", " + gameConfig.world_height + ")");

        gameConfig.xMin = bounds.xMin;
        gameConfig.xMax = bounds.xMax;
        gameConfig.yMin = bounds.yMin;
        gameConfig.yMax = bounds.yMax;

        for (int y = gameConfig.yMin - 1; y <= gameConfig.yMax; y++)
        {
            // Left wall
            wallTilemap.SetTile(new Vector3Int(gameConfig.xMin - 1, y, 0), wallTile);
            // Right wall
            wallTilemap.SetTile(new Vector3Int(gameConfig.xMax, y, 0), wallTile);
        }
        for (int x = gameConfig.xMin; x < gameConfig.xMax; x++)
        {
            // Bottom wall
            wallTilemap.SetTile(new Vector3Int(x, gameConfig.yMin - 1, 0), wallTile);
            // Top wall
            wallTilemap.SetTile(new Vector3Int(x, gameConfig.yMax, 0), wallTile);

            for (int y = gameConfig.yMin; y < gameConfig.yMax; y++)
            {
                // Background
                backgroundTilemap.SetTile(new Vector3Int(x, y, 0), backgroundTile);
            }
        }
    }
}
