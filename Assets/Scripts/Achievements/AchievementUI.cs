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
}