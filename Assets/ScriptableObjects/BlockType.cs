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
    public bool interactable;
    public float hardness;

    public int respawnRate;
    public BlockType replacementBlock;

    public ToolType toolType;
    public ToolTier minimumToolTier;

    public LootDrop[] lootDrops;

    private void OnValidate()
    {
        Debug.Assert(!string.IsNullOrEmpty(displayName), $"BlockType '{name}' has no displayName.");
        Debug.Assert(tile != null || displayName == "Air", $"BlockType '{name}' has no tile assigned.");

        if (lootDrops != null)
        {
            for (int i = 0; i < lootDrops.Length; i++)
            {
                Debug.Assert(lootDrops[i].itemType != null, $"BlockType '{name}': lootDrop[{i}] has no itemType assigned.");
                Debug.Assert(lootDrops[i].amount > 0, $"BlockType '{name}': lootDrop[{i}] has non-positive amount.");
            }
        }
    }
}

[Serializable]
public struct LootDrop
{
    public ItemType itemType;
    public int amount;
    [Range(0f, 1f)] public float chance;
}