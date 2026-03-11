using UnityEngine;
using System.Collections.Generic;

public class AchievementUI : MonoBehaviour
{
    [SerializeField] private AchievementManager achievementManager;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform container;

    private void OnEnable()
    {
        RefreshUI();
        AchievementManager.OnAchievementUnlocked += HandleAchievementUnlocked;
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

        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        List<AchievementManager.Achievement> allAchievements = achievementManager.GetAchievements();

        foreach (var ach in allAchievements)
        {
            GameObject newSlot = Instantiate(slotPrefab, container);
            newSlot.GetComponent<AchievementSlotUI>().Setup(ach);
        }
    }

    public bool IsOpen => gameObject.activeSelf;

    public void Toggle()
    {
        bool newState = !gameObject.activeSelf;
        gameObject.SetActive(newState);

        if (newState) RefreshUI();

        Time.timeScale = newState ? 0f : 1f;
    }
}