using UnityEngine;
using UnityEngine.Tilemaps;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class DestroyTileRepository
{
    private static List<TileBase> destroyTiles;

    private static void Initialize()
    {
        destroyTiles = Resources.LoadAll<TileBase>("DestroyAnimation").ToList();
        Debug.Log("Initialized DestroyTileRepository with " + destroyTiles.Count + " destroy animation tiles.");
    }

    public static TileBase GetDestroyTile(int stage)
    {
        if (destroyTiles == null)
            Initialize();
        if (stage == 0)
            return null; // No damage

        if (stage < 0 || stage > destroyTiles.Count)
        {
            Debug.LogWarning("Invalid destroy tile stage: " + stage);
            return null;
        }
        return destroyTiles[stage - 1]; // stage is 1-indexed for the caller, but list is 0-indexed
    }
}