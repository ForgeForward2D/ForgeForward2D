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

    [SerializeField] private float displayDuration = 3f;
    [SerializeField] private float fadeDuration = 0.5f;

    [SerializeField] private Sprite defaultIcon;

    private Queue<Achievement> achievementQueue = new Queue<Achievement>();
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

    private void HandleAchievementUnlocked(Achievement achievement)
    {
        achievementQueue.Enqueue(achievement);

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
            Achievement currentAchievement = achievementQueue.Dequeue();

            titleText.text = currentAchievement.title;
            BlockType type = currentAchievement.blockType;
            string blockName = (type != null) ? type.displayName : "Air";

            iconImage.sprite = currentAchievement.icon ?? defaultIcon;

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