using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Scriptable Objects/BlockType")]
public class BlockType : ScriptableObject
{
    public int id;

    public string displayName;

    public TileBase tile;
    // public TileBase activeTile; // Maybe for burning furnace

    public bool walkable;
    public bool breakable;
    public float hardness;
    // bool farmable;

    public int respawnRate;
    public int replacementBlockId;

    public int itemID;
}