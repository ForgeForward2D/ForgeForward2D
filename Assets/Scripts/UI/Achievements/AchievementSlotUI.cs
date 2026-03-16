
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
        titleText.text = achievement.title;
        iconImage.sprite = achievement.icon;
        background.color = achievement.isUnlocked ? unlockedColor : lockedColor;

        string blockName = "Unknown Block";
        if (achievement.blockType == null)
        {
            Debug.LogError($"Block Type not set for achievement {achievement.title}");
        }
        else
        {
            BlockType type = achievement.blockType;
            blockName = (type != null) ? type.displayName : "Unknown Block";
        }
        descriptionText.text = achievement.GetDescription(blockName);
    }
}
