using UnityEngine;
using UnityEngine.Tilemaps;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class BlockTypeRepository
{
    private static Dictionary<string, BlockType> nameLookup;
    private static Dictionary<TileBase, BlockType> tileLookup;

    private static void Initialize()
    {
        var BlockTypes = Resources.LoadAll<BlockType>("BlockData");

        nameLookup = new Dictionary<string, BlockType>();
        tileLookup = new Dictionary<TileBase, BlockType>();

        foreach (var block in BlockTypes)
        {
            nameLookup[block.displayName] = block;
            if (block.tile != null)
            {
                tileLookup[block.tile] = block;
            }
        }

        Debug.Log($"Initialized BlockTypeRepository with {BlockTypes.Length} blocks.");
    }

    public static BlockType GetBlockByName(string name)
    {
        if (nameLookup == null)
            Initialize();

        nameLookup.TryGetValue(name, out BlockType block);
        return block;
    }

    public static BlockType GetBlockByTile(TileBase tile)
    {
        if (tileLookup == null)
            Initialize();
        
        tileLookup.TryGetValue(tile, out BlockType block);
        return block;
    }
}