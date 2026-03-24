using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Achievements/LevelAchievement")]
public class LevelAchievement : Achievement
{
    public Level level;
    public int targetCount;

    public override string GetDescription()
    {
        if (targetCount == 1)
            return $"Travel to {level.levelName}.";
        else
            return $"Travel to {level.levelName} {targetCount} times.";
    }

    public override void CheckCompletion(Tracker tracker)
    {
        if (IsCompleted)
            return;

        int currentCount = tracker.GetVisitedLevels().TryGetValue(level, out int visited) ? visited : 0;

        if (currentCount >= targetCount)
            completionTime = DateTime.Now;
    }
}