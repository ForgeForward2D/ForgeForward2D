using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/ItemType")]

public class ItemType : ScriptableObject 
{
    [field: SerializeField] public int Id { get; private set; }
    [field: SerializeField] public string DisplayName { get; private set; }
    [field: SerializeField] public int MaxStackSize { get; private set; } = 64;
    [field: SerializeField] public Sprite Icon { get; private set; }

}