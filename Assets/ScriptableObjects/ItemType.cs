using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/ItemType")]

public class ItemType : ScriptableObject
{
    [SerializeField] public int Id;
    [SerializeField] public string DisplayName;
    [SerializeField] public int MaxStackSize = 64;
    [SerializeField] public Sprite Icon;

}