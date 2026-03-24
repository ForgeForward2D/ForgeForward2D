using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class AchievementUI : UIComponent<AchievementManager>
{
    public static event Action RequestRefresh;

    public void OnEnable()
    {
        AchievementManager.OnAchievementManagerUpdate += RefreshUI;
        RequestRefresh?.Invoke();
    }

    private void OnDisable()
    {
        AchievementManager.OnAchievementManagerUpdate -= RefreshUI;
    }

    public override void RefreshUI(AchievementManager achievementManager)
    {
        List<UIComponentBase> children = GetChildren();
        List<Achievement> achievements = achievementManager.GetAchievements();
        int selectedIndex = achievementManager.GetSelectedIndex();
        int childIndex = 0;

        Debug.Log($"Refreshing achievement UI with {achievements.Count} achievements, selected index {selectedIndex} (Slots: {children.Count})");

        for (int i = 0; i < achievements.Count; i++)
        {
            if (childIndex >= children.Count)
            {
                Debug.Log($"No more achievement slots available to display achievements, stopping at index {i}");
                break;
            }

            Achievement achievement = achievements[(selectedIndex + i) % achievements.Count];

            if (!achievement.visible)
            {
                Debug.Log($"Skipping achievement {achievement.title} at index {(selectedIndex + i) % achievements.Count} because it is not visible");
                continue;
            }

            Debug.Assert(children[childIndex] is AchievementSlotUI, $"Child of AchievementUI at index {childIndex} is not a AchievementSlotUI");
            Debug.Log($"Refreshing achievement slot {childIndex} with achievement {achievement.title} (index {(selectedIndex + i) % achievements.Count})");
            children[childIndex].SetActive(true);
            children[childIndex].RefreshUIDynamic(achievement);
            childIndex++;
        }

        for (; childIndex < children.Count; childIndex++)
        {
            children[childIndex].SetActive(false);
        }
    }
}
