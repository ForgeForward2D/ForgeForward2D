using UnityEngine;

public class AchievementTracker : MonoBehaviour
{
    [SerializeField] private AchievementManager achievementManager;

    private const string BlockNameWood = "Wood";
    private const string AchIdFirstWood = "first_wood";

    private void OnEnable()
    {
        if (achievementManager == null)
        {
            Debug.LogError("AchievementTracker: AchievementManager reference is missing!");
            return;
        }

        BlockBreakingManager.OnBlockBroken += HandleBlockBroken;
    }

    private void OnDisable()
    {
        BlockBreakingManager.OnBlockBroken -= HandleBlockBroken;
    }

    private void HandleBlockBroken((BlockType block, Vector2Int pos) brokenBlockInfo)
    {
        var (block, _) = brokenBlockInfo;

        switch (block.displayName)
        {
            case BlockNameWood:
                achievementManager.UnlockAchievement(AchIdFirstWood);
                break;
        }
    }
}