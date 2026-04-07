using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Tool")]

public class Tool : ItemType
{
    public ToolType type;
    public ToolTier tier;
    public float efficiency = 1f;
    public GameObject prefab;
}

public enum ToolType
{
    None,
    Pickaxe,
    Axe,
    Shovel,
}

public enum ToolTier
{
    None,
    Wood,
    Stone,
    Iron,
    Diamond
}