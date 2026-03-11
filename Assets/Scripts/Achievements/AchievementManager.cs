using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    [SerializeField] private string fileName = "achievements.json";

    public static event Action<Achievement> OnAchievementUnlocked;

    [System.Serializable]
    public class Achievement
    {
        public string id;
        public string title;
        public string description;
        public bool isUnlocked;
        public string iconPath;
    }

    [Serializable]
    private class AchievementDataWrapper
    {
        public List<Achievement> achievements;
    }

    private List<Achievement> achievementList = new List<Achievement>();
    private string savePath;

    private void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, fileName);
        LoadAchievements();
    }

    public void UnlockAchievement(string id)
    {
        Achievement ach = achievementList.Find(a => a.id == id);

        if (ach == null)
        {
            Debug.LogWarning($"Achievement ID '{id}' not found!");
            return;
        }

        if (ach.isUnlocked) return;

        ach.isUnlocked = true;
        Debug.Log($"<color=green>Achievement Unlocked: {ach.title}</color>");

        SaveAchievements();

        OnAchievementUnlocked?.Invoke(ach);
    }
    private void LoadAchievements()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            AchievementDataWrapper wrapper = JsonUtility.FromJson<AchievementDataWrapper>(json);

            if (wrapper != null && wrapper.achievements != null)
            {
                achievementList =wrapper.achievements;
            }
            else
            {
                Debug.LogWarning("Save file was corrupted or empty. Loading from tamplate...");
                LoadFromTemplate();
            }
        }
        else
        {
            LoadFromTemplate();
        }

        if (achievementList == null)
        {
            achievementList = new List<Achievement>();
        }
    }

    private void LoadFromTemplate()
    {
        TextAsset template = Resources.Load<TextAsset>("Achievements/achievements");

        if (template != null)
        {
            AchievementDataWrapper wrapper = JsonUtility.FromJson<AchievementDataWrapper>(template.text);
            if (wrapper != null && wrapper.achievements != null)
            {
                achievementList = wrapper.achievements;
            }
        }
        else
        {
            Debug.LogError("No achievement template found in Resources!");
        }
    }

    private void SaveAchievements()
    {
        string json = JsonUtility.ToJson(new AchievementDataWrapper { achievements = achievementList}, true);
        File.WriteAllText(savePath, json);
    }

    public List<Achievement> GetAchievements() => achievementList;
}