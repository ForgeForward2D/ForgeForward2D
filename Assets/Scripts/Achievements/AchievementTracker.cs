using UnityEngine;

public class AchievementTracker : MonoBehaviour
{
    [SerializeField] private AchievementManager achievementManager;

    private void OnEnable()
    {
        if (achievementManager == null) return;
        BlockBreakingManager.OnBlockBroken += HandleBlockBroken;
    }

    private void OnDisable()
    {
        BlockBreakingManager.OnBlockBroken -= HandleBlockBroken;
    }

    private void HandleBlockBroken((BlockType block, Vector2Int pos) brokenBlockInfo)
    {
        var (brokenBlock, _) = brokenBlockInfo;

        foreach (var ach in achievementManager.GetAchievements())
        {
            if (ach.isUnlocked) continue;

            if (ach.group == "collect_material")
            {
                if (ach.blockTypeName == brokenBlock.displayName)
                {
                    ach.currentProgress++;

                    if (ach.currentProgress >= ach.number)
                    {
                        achievementManager.UnlockAchievement(ach.id);
                    }
                }
            }
        }
    }
}