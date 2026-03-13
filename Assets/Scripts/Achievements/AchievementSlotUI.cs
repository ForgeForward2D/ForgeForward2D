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

    [SerializeField] private Sprite defaultIcon;

    public void Setup(Achievement achievement)
    {
        if (achievement == null)
        {
            Debug.LogWarning("AchievementSlotUI: Attempted to setup with null achievement.");
            return;
        }

        titleText.text = achievement.title;

        BlockType type = achievement.blockType;
        string blockName = (type != null) ? type.displayName : "Unknown Block";

        descriptionText.text = achievement.GetDescription(blockName);

        iconImage.sprite = achievement.icon ?? defaultIcon;

        background.color = achievement.isUnlocked ? unlockedColor : lockedColor;
    }
}