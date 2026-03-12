using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AchievementPopupManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup popupCanvasGroup;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image iconImage;

    [SerializeField] private float displayDuration = 3f;
    [SerializeField] private float fadeDuration = 0.5f;

    private Queue<AchievementManager.Achievement> achievementQueue = new Queue<AchievementManager.Achievement>();
    private bool isShowingPopup = false;

    private void OnEnable()
    {
        AchievementManager.OnAchievementUnlocked += HandleAchievementUnlocked;

        if (popupCanvasGroup != null)
        {
            popupCanvasGroup.alpha = 0f;
            popupCanvasGroup.blocksRaycasts = false;
        }
    }

    private void OnDisable()
    {
        AchievementManager.OnAchievementUnlocked -= HandleAchievementUnlocked;

        isShowingPopup = false;
        if (popupCanvasGroup != null) popupCanvasGroup.alpha = 0f;
        StopAllCoroutines();
    }

    private void HandleAchievementUnlocked(AchievementManager.Achievement ach)
    {
        achievementQueue.Enqueue(ach);

        if (!isShowingPopup && gameObject.activeInHierarchy)
        {
            StartCoroutine(ProcessPopupQueue());
        }
    }

    private IEnumerator ProcessPopupQueue()
    {
        isShowingPopup = true;

        while (achievementQueue.Count > 0)
        {
            AchievementManager.Achievement currentAch = achievementQueue.Dequeue();

            titleText.text = currentAch.title;
            descriptionText.text = currentAch.description;
            
            if (!string.IsNullOrEmpty(currentAch.iconPath))
            {
                Sprite icon = Resources.Load<Sprite>(currentAch.iconPath);
                if (icon != null) iconImage.sprite = icon;
            }

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