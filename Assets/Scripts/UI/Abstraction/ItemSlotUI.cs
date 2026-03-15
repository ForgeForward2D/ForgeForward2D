using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlotUI : UIComponent<ItemWithText>
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI countText;

    public override void RefreshUI(ItemWithText data)
    {
        Item item = data?.item;
        string text = data?.text;

        if (item == null || item.itemType == null)
        {
            iconImage.sprite = null;
            iconImage.color = Color.clear;
            countText.text = text;
            return;
        }

        if (text == null)
        {
            text = item.count.ToString();
        }

        iconImage.sprite = item.itemType.icon;
        iconImage.color = Color.white;
        
        countText.text = text;
    }
}

public record ItemWithText(Item item, string text);