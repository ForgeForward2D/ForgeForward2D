using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AchievementSlotUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image background;

    public void Setup(AchievementManager.Achievement data)
    {
        titleText.text = data.title;
        descriptionText.text = data.description;

        background.color = data.isUnlocked ? new Color(1, 0.8f, 0, 0.5f) : new Color(0.2f, 0.2f, 0.2f, 0.5f);
    }
}