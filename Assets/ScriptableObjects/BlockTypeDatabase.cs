using UnityEngine;
using UnityEngine.Tilemaps;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class BlockTypeRepository
{
    private static Dictionary<int, BlockType> idLookup;
    private static Dictionary<TileBase, BlockType> tileLookup;
    // private static Dictionary<TileBase, BlockType> activeTileLookup;
    private static Dictionary<string, BlockType> nameLookup;

    private static void Initialize()
    {
        var BlockTypes = Resources.LoadAll<BlockType>("BlockData");

        idLookup = new Dictionary<int, BlockType>();
        nameLookup = new Dictionary<string, BlockType>();
        tileLookup = new Dictionary<TileBase, BlockType>();

        foreach (var block in BlockTypes)
        {
            idLookup[block.id] = block;
            if (block.tile != null)
            {
                tileLookup[block.tile] = block;
            }
            nameLookup[block.displayName] = block;
        }


        Debug.Log("Initialized BlockTypeRepository with " + BlockTypes.Length + " blocks.");
        Debug.Log("ID lookup keys: " + string.Join(", ", idLookup.Keys));
    }

    public static BlockType GetBlockById(int id)
    {
        if (idLookup == null)
            Initialize();
        // return idLookup[id];
        return idLookup.GetValueOrDefault(id);
    }

    public static BlockType GetBlockByTile(TileBase tile)
    {
        if (tileLookup == null)
            Initialize();
        // return tileLookup[tile];
        return tileLookup.GetValueOrDefault(tile);
    }

    public static BlockType GetBlockByName(string name)
    {
        if (nameLookup == null)
            Initialize();
        // return nameLookup[name];
        return nameLookup.GetValueOrDefault(name);
    }
}