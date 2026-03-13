using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public static event Action<Achievement> OnAchievementUnlocked;

    private List<Achievement> achievementList = new List<Achievement>();
    private string savePath;

    private void Awake()
    {
        LoadAchievementsFromTemplate();
    }

    public void UnlockAchievement(Achievement achievement)
    {
        if (achievement == null)
        {
            Debug.LogWarning("Attempted to unlock a null achievement!");
            return;
        }

        if (achievement.isUnlocked) return;

        achievement.isUnlocked = true;
        Debug.Log($"<color=green>Achievement Unlocked: {achievement.title}</color>");

        OnAchievementUnlocked?.Invoke(achievement);
    }

    private void LoadAchievementsFromTemplate()
    {
        Achievement[] achievements = Resources.LoadAll<Achievement>("Achievements");

        if (achievements != null)
        {
            AchievementDataWrapper wrapper = JsonUtility.FromJson<AchievementDataWrapper>(template.text);
            if (wrapper?.achievements != null)
            {
                achievementOrderedList = wrapper.achievements;
            }
        }
        else
        {
            Debug.LogError("No achievement template found in Resources!");
        }

        foreach (Achievement ach in achievementList)
        {
            ach.isUnlocked = false;
            ach.currentProgress = 0;
        }

        Debug.Log($"{achievementList.Count} achievements loaded.");
    }

    public List<Achievement> GetAchievements() => achievementList;
}