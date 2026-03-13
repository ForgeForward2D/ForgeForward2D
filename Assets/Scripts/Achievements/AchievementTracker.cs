using UnityEngine;

public class AchievementTracker : MonoBehaviour
{
    [SerializeField] private AchievementManager achievementManager;

    private void OnEnable()
    {
        if (achievementManager == null) return;
        BlockBreakingManager.OnBlockBroken += HandleBlockBroken;
        Debug.Log("AchievementTracker enabled and subscribed to BlockBreakingManager.OnBlockBroken.");
    }

    private void OnDisable()
    {
        BlockBreakingManager.OnBlockBroken -= HandleBlockBroken;
    }

    private void HandleBlockBroken((BlockType block, Vector2Int pos) brokenBlockInfo)
    {
        var (brokenBlock, _) = brokenBlockInfo;
        Debug.Log($"Block broken: {brokenBlock.displayName} at position {brokenBlockInfo.pos}. Checking achievements ({achievementManager.GetAchievements().Count} total).");
        foreach (var ach in achievementManager.GetAchievements())
        {
            if (ach.isUnlocked) continue;

            if (ach.group == "collect_material")
            {
                if (ach.blockType == brokenBlock)
                {
                    ach.currentProgress++;

                    if (ach.currentProgress >= ach.numberOfBlocks)
                    {
                        achievementManager.UnlockAchievement(ach);
                    }
                }
            }
        }
    }
}