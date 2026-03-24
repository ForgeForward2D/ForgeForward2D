using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Achievements/CraftingAchievement")]
public class CraftingAchievement : Achievement
{
    public CraftingRecipe recipe;
    public int targetCount;

    public override string GetDescription()
    {
        return $"Craft {targetCount} {recipe.result.itemType.displayName}.";
    }

    public override void CheckCompletion(Tracker tracker)
    {
        if (IsCompleted())
            return;
            
        int currentCount = tracker.GetRecipesCrafted().TryGetValue(recipe, out int crafted) ? crafted : 0;

        if (currentCount >= targetCount)
            completionTime = DateTime.Now;
    }
}
