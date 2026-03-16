using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/ItemType")]

public class ItemType : ScriptableObject
{
    public string displayName;
    public int maxStackSize = 64;
    public Sprite icon;
}
