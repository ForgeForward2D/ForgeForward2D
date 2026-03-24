
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public static event Action<Achievement> OnAchievementUnlocked;
    public static event Action<AchievementManager> OnAchievementManagerUpdate;

    [Header("Debugging")]
    [SerializeField] private List<Achievement> achievements = new List<Achievement>();
    [SerializeField] private int selectedIndex;

    private void Awake()
    {
        achievements = new List<Achievement>(Resources.LoadAll<Achievement>("Achievements"));
        foreach (var achievement in achievements)
        {
            achievement.completionTime = default;
        }
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

        // Skip one row (equal to 4 achievements)
        selectedIndex += delta * 4;
        selectedIndex = (selectedIndex % achievements.Count + achievements.Count) % achievements.Count;

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

    public int GetSelectedIndex()
    {
        return selectedIndex;
    }
}
