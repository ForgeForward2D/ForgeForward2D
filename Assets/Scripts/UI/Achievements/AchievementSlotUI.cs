
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AchievementSlotUI : UIComponent<Achievement>
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image background;
    [SerializeField] private Image iconImage;

    [Header("Settings")]
    [SerializeField] private Color unlockedColor = new Color(1f, 0.8f, 0f, 0.5f);
    [SerializeField] private Color lockedColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);

    public override void RefreshUI(Achievement achievement)
    {
        if (achievement == null)
        {
            titleText.text = "";
            descriptionText.text = "";
            iconImage.sprite = null;
            iconImage.color = new Color(1f, 1f, 1f, 0f);
            background.color = lockedColor;
            return;
        }

        titleText.text = achievement.title;
        descriptionText.text = achievement.GetDescription();
        iconImage.sprite = achievement.icon;
        iconImage.color = Color.white;
        background.color = achievement.IsCompleted ? unlockedColor : lockedColor;
    }
}
