using UnityEngine;

[System.Serializable]
public struct LootDrop
{
    public ItemType itemType;
    [Range(0f, 1f)] public float chance;
}

[System.Serializable]
public struct BlockLootTable
{
    public BlockType blockType;
    public LootDrop[] drops;
}
