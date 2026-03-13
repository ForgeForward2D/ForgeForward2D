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

    public void Setup(AchievementManager.Achievement data)
    {
        if (data == null)
        {
            Debug.LogWarning("AchievementSlotUI: Attempted to setup with null data.");
            return;
        }

        titleText.text = data.title;

        BlockType type = BlockTypeRepository.GetBlockByName(data.blockTypeName);
        string blockName = (type != null) ? type.displayName : "Unknown Block";

        descriptionText.text = data.GetDescription(blockName);

        iconImage.sprite = defaultIcon;

        if (!string.IsNullOrEmpty(data.iconPath))
        {
            Sprite[] icons = Resources.LoadAll<Sprite>(data.iconPath);

            if (icons != null && icons.Length > 0)
            {
                iconImage.sprite = icons[0];
            }
            else
            {
                Debug.LogWarning($"AchievementSlotUI: Icon not found at 'Resources/{data.iconPath}' for achievement '{data.id}'");
            }
        }

        background.color = data.isUnlocked ? unlockedColor : lockedColor;
    }
}