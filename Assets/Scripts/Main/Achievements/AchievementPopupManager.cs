using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementPopupManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup popupCanvasGroup;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Image iconImage;

    [Header("Settings")]
    [SerializeField] private float displayDuration = 3f;
    [SerializeField] private float fadeDuration = 0.5f;


    private Queue<Achievement> achievementQueue = new Queue<Achievement>();
    private bool isShowingPopup = false;

    private void Awake()
    {
        AchievementManager.OnAchievementUnlocked += HandleAchievementUnlocked;

        if (popupCanvasGroup != null)
        {
            popupCanvasGroup.alpha = 0f;
            popupCanvasGroup.blocksRaycasts = false;
        }
    }

    private void HandleAchievementUnlocked(Achievement ach)
    {
        achievementQueue.Enqueue(ach);
        Debug.Log($"Added achievment to queue {ach.title}, {achievementQueue.Count}");

        if (!isShowingPopup && gameObject.activeInHierarchy)
        {
            StartCoroutine(ProcessPopupQueue());
        }
    }

    private IEnumerator ProcessPopupQueue()
    {
        Debug.Log($"Start processing achievement queue {achievementQueue.Count}");
        isShowingPopup = true;

        while (achievementQueue.Count > 0)
        {
            Achievement currentAch = achievementQueue.Dequeue();
            Debug.Log($"Processing: {currentAch.title}");

            titleText.text = currentAch.title;
            iconImage.sprite = currentAch.icon;

            yield return StartCoroutine(FadePopup(0f, 1f));

            yield return new WaitForSecondsRealtime(displayDuration);

            yield return StartCoroutine(FadePopup(1f, 0f));

            yield return new WaitForSecondsRealtime(0.2f);
        }

        isShowingPopup = false;
    }

    private IEnumerator FadePopup(float startAlpha, float targetAlpha)
    {
        float elapsedTime = 0f;
        popupCanvasGroup.alpha = startAlpha;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            popupCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            yield return null;
        }

        popupCanvasGroup.alpha = targetAlpha;
    }
}