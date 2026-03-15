
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AchievementSlotUI : UIComponent<Achievement>
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image background;
    [SerializeField] private Image iconImage;

    [Header("Settings")]
    [SerializeField] private Color unlockedColor = new Color(1f, 0.8f, 0f, 0.5f);
    [SerializeField] private Color lockedColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);

    public override void RefreshUI(Achievement achievement)
    {
        BlockType type = achievement.blockType;
        string blockName = (type != null) ? type.displayName : "Unknown Block";

        titleText.text = achievement.title;
        descriptionText.text = achievement.GetDescription(blockName);
    
        iconImage.sprite = achievement.icon;

        background.color = achievement.isUnlocked ? unlockedColor : lockedColor;
    }
}
