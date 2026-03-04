using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/ItemType")]

public class ItemType : ScriptableObject 
{
    public int id;

    public int maxStackSize;

    public Sprite icon;

}