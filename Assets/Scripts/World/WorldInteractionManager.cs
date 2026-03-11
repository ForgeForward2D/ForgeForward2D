
using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(TileMapManager))]
public class WorldInteractionManager : MonoBehaviour
{
    [SerializeField] GameConfig gameConfig;
    [SerializeField] private PlayerController playerController;

    private TileMapManager tileMapManager;

    public void Start()
    {
        tileMapManager = GetComponent<TileMapManager>();
    }

    public Vector2Int PositionToCoordinate(Vector3 worldPosition)
    {
        return tileMapManager.PositionToCoordinate(worldPosition);
    }

    public void InteractWithBlock(Vector2Int cellPosition)
    {
        // TODO: interactions with blocks (e.g. chests, crafting station, furnace )
    }
}
