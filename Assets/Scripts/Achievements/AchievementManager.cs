using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    [SerializeField] private string templatePath = "Achievements/achievements";

    public static event Action<Achievement> OnAchievementUnlocked;

    [Serializable]
    public class Achievement
    {
        public string id;
        public string title;
        public string group;
        public int number;
        public string blockTypeName;
        public string iconPath;

        public bool isUnlocked;
        public int currentProgress;

        public string GetDescription(string blockName)
        {
            return $"Break {number} blocks of {blockName}.";
        }
    }

    [Serializable]
    private class AchievementDataWrapper
    {
        public List<Achievement> achievements = new List<Achievement>();
    }

    private List<Achievement> achievementOrderedList = new List<Achievement>();
    private Dictionary<string, Achievement> achievementLookup = new Dictionary<string, Achievement>();
    private string savePath;

    private void Awake()
    {
        LoadAchievementsFromTemplate();
    }

    public void UnlockAchievement(string id)
    {
        if (!achievementLookup.TryGetValue(id, out Achievement ach))
        {
            Debug.LogWarning($"Achievement ID '{id}' not found!");
            return;
        }

        if (ach.isUnlocked) return;

        ach.isUnlocked = true;
        Debug.Log($"<color=green>Achievement Unlocked: {ach.title}</color>");

        OnAchievementUnlocked?.Invoke(ach);
    }

    private void LoadAchievementsFromTemplate()
    {
        TextAsset template = Resources.Load<TextAsset>(templatePath);

        if (template != null)
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

        achievementLookup.Clear();
        foreach (Achievement ach in achievementOrderedList)
        {
            ach.isUnlocked = false;
            ach.currentProgress = 0;

            achievementLookup[ach.id] = ach;
        }

        Debug.Log($"{achievementOrderedList.Count} achievements loaded.");
    }

    public IEnumerable<Achievement> GetAchievements() => achievementOrderedList;
}