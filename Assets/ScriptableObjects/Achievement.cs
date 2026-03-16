using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Achievement")]
public class Achievement : ScriptableObject
{
    public string title;
    public string group;
    public BlockType blockType;
    public int numberOfBlocks;
    public Sprite icon;

    public bool isUnlocked;
    public int currentProgress;

    public string GetDescription(string blockName)
    {
        return $"Break {numberOfBlocks} blocks of {blockName}.";
    }

}
