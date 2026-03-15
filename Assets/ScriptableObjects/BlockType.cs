using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Scriptable Objects/BlockType")]
public class BlockType : ScriptableObject
{
    public string displayName;

    public TileBase tile;

    public bool walkable;
    public bool breakable;
    public float hardness;

    public int respawnRate;
    public BlockType replacementBlock;

    public ToolType toolType;
    public ToolTier minimumToolTier;

    public LootDrop[] lootDrops;
}

[Serializable]
public struct LootDrop
{
    public ItemType itemType;
    public int amount;
    [Range(0f, 1f)] public float chance;
}