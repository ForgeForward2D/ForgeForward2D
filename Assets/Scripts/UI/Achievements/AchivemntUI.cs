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

        int achievementWrappingPoint = achievements.Count + 4 - achievements.Count % 4; // Round up to nearest multiple of 4

        for (int i = 0; i < achievementWrappingPoint; i++)
        {
            if (childIndex >= children.Count)
                break;

            int achievementIndex = (selectedIndex + i) % achievementWrappingPoint;

            if (achievementIndex >= achievements.Count)
            {
                children[childIndex].SetActive(true);
                children[childIndex].RefreshUIDynamic(null);
                childIndex++;
                continue;
            }

            Achievement achievement = achievements[achievementIndex];

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
