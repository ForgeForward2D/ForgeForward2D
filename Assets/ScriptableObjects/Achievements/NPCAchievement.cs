using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Achievements/NPCAchievement")]
public class NPCAchievement : Achievement
{
    public NpcType npcType;
    public int targetCount;

    public override string GetDescription()
    {
        return $"Interact with {npcType.displayName} {targetCount} times.";
    }

    public override void CheckCompletion(Tracker tracker)
    {
        if (IsCompleted)
            return;

        int currentCount = tracker.GetNpcInteractions().TryGetValue(npcType, out int interacted) ? interacted : 0;

        if (currentCount >= targetCount)
            completionTime = DateTime.Now;
    }
}

