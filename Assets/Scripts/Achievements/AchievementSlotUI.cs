using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AchievementSlotUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image background;
    [SerializeField] private Image iconImage;

    [SerializeField] private Color unlockedColor = new Color(1f, 0.8f, 0f, 0.5f);
    [SerializeField] private Color lockedColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);

    public void Setup(AchievementManager.Achievement data)
    {
        if (data == null)
        {
            Debug.LogWarning("AchievementSlotUI: Attempted to setup with null data.");
            return;
        }

        titleText.text = data.title;
        descriptionText.text = data.description;

        if (!string.IsNullOrEmpty(data.iconPath))
        {
            Sprite icon = Resources.Load<Sprite>(data.iconPath);
            if (icon != null)
            {
                iconImage.sprite = icon;
            }
            else
            {
                Debug.LogWarning($"AchievementSlotUI: Icon not found at 'Resources/{data.iconPath}' for achievement '{data.id}'");
            }
        }

        background.color = data.isUnlocked ? unlockedColor : lockedColor;
    }
}