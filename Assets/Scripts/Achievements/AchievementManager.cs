using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    [SerializeField] private string fileName = "achievements.json";

    [Serializable]
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
    }
    private void LoadAchievements()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            achievementList = JsonUtility.FromJson<AchievementDataWrapper>(json).achievements;
        }
        else
        {
            TextAsset template = Resources.Load<TextAsset>("achievements_template");
            if (template != null)
            {
                achievementList = JsonUtility.FromJson<AchievementDataWrapper>(template.text).achievements;
            }
            else
            {
                Debug.LogError("No achievement template found in Resources!");
            }
        }
    }

    private void SaveAchievements()
    {
        string json = JsonUtility.ToJson(new AchievementDataWrapper { achievements = achievementList}, true);
        File.WriteAllText(savePath, json);
    }

    public List<Achievement> GetAchievements() => achievementList;
}