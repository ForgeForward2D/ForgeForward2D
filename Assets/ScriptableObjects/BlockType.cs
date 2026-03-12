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

    public int respawnRate;
    public int replacementBlockId;

    public ToolType toolType;
    public ToolTier minimumToolTier;

    public LootDrop[] lootDrops;
}

[System.Serializable]
public struct LootDrop
{
    public ItemType itemType;
    public int amount;
    [Range(0f, 1f)] public float chance;
}