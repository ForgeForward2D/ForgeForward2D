using UnityEngine;

public class AchievementTracker : MonoBehaviour
{
    [SerializeField] private AchievementManager achievementManager;

    private void OnEnable()
    {
        BlockBreakingManager.OnBlockBroken += HandleBlockBroken;
    }

    private void OnDisable()
    {
        BlockBreakingManager.OnBlockBroken -= HandleBlockBroken;
    }

    private void HandleBlockBroken((BlockType block, Vector2Int pos) brokenBlockInfo)
    {
        if (brokenBlockInfo.block.displayName == "Wood")
        {
            achievementManager.UnlockAchievement("first_wood");
        }
    }
}