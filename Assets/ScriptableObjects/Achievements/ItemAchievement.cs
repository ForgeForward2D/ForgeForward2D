using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Achievements/ItemAchievement")]
public class ItemAchievement : Achievement
{
    public ItemType itemType;
    public int targetCount;

    public override string GetDescription()
    {
        return $"Obtain {targetCount} {itemType.displayName}.";
    }

    public override void CheckCompletion(Tracker tracker)
    {
        if (IsCompleted)
            return;

        int currentCount = tracker.GetItemsCollected().TryGetValue(itemType, out int obtained) ? obtained : 0;

        if (currentCount >= targetCount)
            completionTime = DateTime.Now;
    }
}