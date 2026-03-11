using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Tool")]

public class Tool : ItemType
{
    [SerializeField] public ToolType type;
    [SerializeField] public ToolTier tier;
    [SerializeField] public float efficiency = 1f;
}

public enum ToolType
{
    None,
    Sword,
    Pickaxe,
    Axe,
    Shovel,
    Hammer,
}

public enum ToolTier
{
    None,
    Wood,
    Stone,
    Iron,
    Diamond
}