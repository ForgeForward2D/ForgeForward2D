
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public static event Action<Achievement> OnAchievementUnlocked;
    public static event Action<AchievementManager> OnAchievementManagerUpdate;

    [Header("Debugging")]
    [SerializeField] private List<Achievement> achievements;
    [SerializeField] private List<Achievement> visibleAchievements;
    [SerializeField] private int selectedIndex;

    private void Awake()
    {
        achievements = new List<Achievement>(Resources.LoadAll<Achievement>("Achievements"));
        foreach (var achievement in achievements)
        {
            achievement.completionTime = default;
        }
        visibleAchievements = achievements.Where(a => a.visible).ToList();
        selectedIndex = 0;

        AchievementUI.RequestRefresh += HandleRequestRefresh;
        InputManager.OnMoveInput += HandleMovementInput;
        Tracker.OnTrackerUpdate += HandleTrackerUpdate;
    }

    private void HandleRequestRefresh()
    {
        OnAchievementManagerUpdate?.Invoke(this);
    }

    private void HandleMovementInput((UIPage, bool, Vector2) data)
    {
        var (uiPage, performed, movementInput) = data;

        if (uiPage != UIPage.Achievements)
            return;

        if (!performed)
            return;

        if (movementInput.y == 0)
            return;

        int delta = movementInput.y > 0 ? -1 : 1;
        
        // Round up to nearest multiple of 4
        int achievementWrappingPoint = achievements.Count - achievements.Count % 4 + (achievements.Count % 4 == 0 ? 0 : 4);

        // Skip one row (equal to 4 achievements)
        selectedIndex += delta * 4;
        selectedIndex = (selectedIndex % achievementWrappingPoint + achievementWrappingPoint) % achievementWrappingPoint;

        OnAchievementManagerUpdate?.Invoke(this);
    }

    public void HandleTrackerUpdate(Tracker tracker)
    {
        foreach (var achievement in achievements)
        {
            if (achievement.IsCompleted)
                continue;
            achievement.CheckCompletion(tracker);
            if (achievement.IsCompleted)
                OnAchievementUnlocked?.Invoke(achievement);
        }
    }

    public List<Achievement> GetAchievements()
    {
        return achievements;
    }

    public List<Achievement> GetVisibleAchievements()
    {
        return visibleAchievements;
    }

    public int GetSelectedIndex()
    {
        return selectedIndex;
    }

    public string Dump()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("CompletionTime,Title,Description,Visible");

        foreach (var achievement in achievements.Where(a => a.IsCompleted).OrderBy(a => a.completionTime))
        {
            sb.AppendLine($"{achievement.completionTime.ToString("yyyy-MM-dd'T'HH-mm-ss")},{achievement.title},{achievement.GetDescription()},{achievement.visible}");
        }

        foreach (var achievement in achievements.Where(a => !a.IsCompleted))
        {
            sb.AppendLine($"Not Completed,{achievement.title},{achievement.GetDescription()},{achievement.visible}");
        }

        Debug.Log($"Achievement data:\n{sb.ToString()}");
        return sb.ToString();
    }
}
