using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Achievements/BlockAchievement")]
public class BlockAchievement : Achievement
{
    public BlockAchievementType type;
    public BlockType blockType;
    public int targetCount;

    public override string GetDescription()
    {
        string action = type switch
        {
            BlockAchievementType.Attack => "Attack",
            BlockAchievementType.Break => "Break",
            BlockAchievementType.Interact => "Interact with",
            _ => "Unknown action"
        };

        return $"{action} {targetCount} {blockType.displayName} blocks.";
    }

    public override void CheckCompletion(Tracker tracker)
    {
        if (IsCompleted)
            return;

        int currentCount = type switch
        {
            BlockAchievementType.Attack => tracker.GetBlockAttacks().TryGetValue(blockType, out int attacked) ? attacked : 0,
            BlockAchievementType.Break => tracker.GetBlocksBroken().TryGetValue(blockType, out int broken) ? broken : 0,
            BlockAchievementType.Interact => tracker.GetBlockInteractions().TryGetValue(blockType, out int interacted) ? interacted : 0,
            _ => 0
        };

        if (currentCount >= targetCount)
            completionTime = DateTime.Now;
    }
}

public enum BlockAchievementType
{
    Attack,
    Break,
    Interact
}