using UnityEngine;
using System.Collections.Generic;

public class AchievementUI : MonoBehaviour
{
    [SerializeField] private AchievementManager achievementManager;
    [SerializeField] private AchievementSlotUI slotPrefab;
    [SerializeField] private Transform container;

    private List<AchievementSlotUI> spawnedSlots = new List<AchievementSlotUI>();

    private void OnEnable()
    {
        AchievementManager.OnAchievementUnlocked += HandleAchievementUnlocked;
        RefreshUI();
    }

    private void OnDisable()
    {
        AchievementManager.OnAchievementUnlocked -= HandleAchievementUnlocked;
    }

    private void HandleAchievementUnlocked(AchievementManager.Achievement ach)
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (achievementManager == null) return;

        var allAchievements = achievementManager.GetAchievements();

        int index = 0;
        foreach (var ach in allAchievements)
        {
            if (index >= spawnedSlots.Count)
            {
                AchievementSlotUI newSlot = Instantiate(slotPrefab, container);
                spawnedSlots.Add(newSlot);
            }

            spawnedSlots[index].gameObject.SetActive(true);
            spawnedSlots[index].Setup(ach);
            index++;
        }

        for (int i = index; i < spawnedSlots.Count; i++)
        {
            spawnedSlots[i].gameObject.SetActive(false);
        }
    }

    public bool IsOpen => gameObject.activeSelf;

    public void Toggle()
    {
        bool newState = !gameObject.activeSelf;

        gameObject.SetActive(newState);

        Time.timeScale = newState ? 0f : 1f;
    }
}