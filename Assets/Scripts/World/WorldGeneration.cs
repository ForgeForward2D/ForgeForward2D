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
        for (int y = -1; y <= gameConfig.world_height; y++)
        {
            // Left wall
            wallTilemap.SetTile(new Vector3Int(-1, y, 0), wallTile);
            // Right wall
            wallTilemap.SetTile(new Vector3Int(gameConfig.world_width, y, 0), wallTile);
        }
        for (int x = 0; x < gameConfig.world_width; x++)
        {
            // Bottom wall
            wallTilemap.SetTile(new Vector3Int(x, -1, 0), wallTile);
            // Top wall
            wallTilemap.SetTile(new Vector3Int(x, gameConfig.world_height, 0), wallTile);

            for (int y = 0; y < gameConfig.world_height; y++)
            {
                // Background
                backgroundTilemap.SetTile(new Vector3Int(x, y, 0), backgroundTile);
            }
        }

        
    }
}
