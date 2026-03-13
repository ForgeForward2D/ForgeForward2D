using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGeneration : MonoBehaviour
{
    [SerializeField] Tilemap backgroundTilemap;
    [SerializeField] TileBase backgroundTile;

    [SerializeField] Tilemap wallTilemap;
    [SerializeField] TileBase wallTile;

    void Start()
    {
        TileMapManager tileMapManager = GetComponent<TileMapManager>();

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
}
