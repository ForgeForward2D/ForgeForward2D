using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/NpcType")]
public class NpcType : ScriptableObject
{
    public string displayName;
    public Sprite characterSprite;
    public string[] dialogueLines;
}