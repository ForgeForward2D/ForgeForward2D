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

        for (int i = 0; i < achievements.Count; i++)
        {
            if (childIndex >= children.Count)
                break;

            Achievement achievement = achievements[(selectedIndex + i) % achievements.Count];
            if (!achievement.visible)
                continue;

            Debug.Assert(children[childIndex] is AchievementSlotUI, $"Child of AchievementUI at index {childIndex} is not a AchievementSlotUI");
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
