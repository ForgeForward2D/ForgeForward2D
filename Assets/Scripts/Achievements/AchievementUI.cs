using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class AchievementUI : MonoBehaviour
{
    [SerializeField] private AchievementManager achievementManager;
    [SerializeField] private AchievementSlotUI slotPrefab;
    [SerializeField] private Transform container;

    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private float scrollSpeed = 5f;

    private List<AchievementSlotUI> spawnedSlots = new List<AchievementSlotUI>();

    private float currentScrollInput;

    private void OnEnable()
    {
        AchievementManager.OnAchievementUnlocked += HandleAchievementUnlocked;
        RefreshUI();
    }

    private void OnDisable()
    {
        AchievementManager.OnAchievementUnlocked -= HandleAchievementUnlocked;
    }

    private void Update()
    {
        if (!gameObject.activeSelf || scrollRect == null || currentScrollInput == 0)
        {
            return;
        }

        scrollRect.verticalNormalizedPosition = Mathf.Clamp01(
            scrollRect.verticalNormalizedPosition + (currentScrollInput * scrollSpeed * Time.unscaledDeltaTime)
        );
    }

    public void SetScrollInput(float value)
    {
        currentScrollInput = value;
    }

    private void HandleAchievementUnlocked(Achievement ach)
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
                AchievementSlotUI newSlot = Instantiate(slotPrefab, container, false);
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

        if (newState && scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 1f;
        }

        if (!newState)
        {
            currentScrollInput = 0;
        }

        Time.timeScale = newState ? 0f : 1f;
    }
}