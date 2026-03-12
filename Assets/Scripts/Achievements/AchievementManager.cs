using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    [SerializeField] private string fileName = "achievements.json";
    [SerializeField] private string templatePath = "Achievements/achievements";

    public static event Action<Achievement> OnAchievementUnlocked;

    [Serializable]
    public class Achievement
    {
        public string id;
        public string title;
        public string group;
        public int number;
        public int blockTypeId;
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
        savePath = Path.Combine(Application.persistentDataPath, fileName);

        /*if (File.Exists(savePath)) //Delete unlocked Achievements every start for testing
        {
            File.Delete(savePath);
        }*/
        LoadAchievements();
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

        SaveAchievements();
        OnAchievementUnlocked?.Invoke(ach);
    }

    private void LoadAchievements()
    {
        achievementOrderedList = LoadFromTemplate();

        achievementLookup.Clear();
        foreach (Achievement ach in achievementOrderedList)
        {
            achievementLookup[ach.id] = ach;
        }

        if (File.Exists(savePath))
        {
            try
            {
                string json = File.ReadAllText(savePath);
                AchievementDataWrapper wrapper = JsonUtility.FromJson<AchievementDataWrapper>(json);

                if (wrapper?.achievements != null)
                {
                    foreach (Achievement savedAch in wrapper.achievements)
                    {
                        if (achievementLookup.TryGetValue(savedAch.id, out Achievement masterAch))
                        {
                            masterAch.isUnlocked = savedAch.isUnlocked;
                            masterAch.currentProgress = savedAch.currentProgress;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to read save file: {e.Message}. Proceeding with clean template");
            }
        }
    }

    private List<Achievement> LoadFromTemplate()
    {
        TextAsset template = Resources.Load<TextAsset>(templatePath);

        if (template != null)
        {
            AchievementDataWrapper wrapper = JsonUtility.FromJson<AchievementDataWrapper>(template.text);
            if (wrapper?.achievements != null)
            {
                return wrapper.achievements;
            }
        }

        Debug.LogError("No achievement template found in Resources!");
        return new List<Achievement>();
    }

    private void SaveAchievements()
    {
        try
        {
            AchievementDataWrapper wrapper = new AchievementDataWrapper
            {
                achievements = achievementOrderedList
            };

            string json = JsonUtility.ToJson(wrapper, true);
            File.WriteAllText(savePath, json);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save achievements: {e.Message}");
        }
    }

    public IEnumerable<Achievement> GetAchievements() => achievementOrderedList;
}