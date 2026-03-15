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

        for (int i = 0; i < children.Count; i++)
        {
            if (i < achievements.Count)
            {
                Debug.Assert(children[i] is AchievementSlotUI, $"Child of AchievementUI at index {i} is not a AchievementSlotUI");
                children[i].SetActive(true);
                children[i].RefreshUIDynamic(achievements[(selectedIndex + i) % achievements.Count]);
            }
            else
            {
                children[i].SetActive(false);
            }
        }
    }
}
