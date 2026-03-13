using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI countText;

    public void UpdateSlot(InventoryItem item)
    {
        if (item == null)
        {
            ClearSlot();
            return;
        }

        iconImage.sprite = item.Item.Icon;
        iconImage.color = Color.white;
        countText.text = item.Count > 1 ? item.Count.ToString() : string.Empty;
    }

    public void UpdateSlotWithString(ItemType itemType, string s)
    {
        if (itemType == null)
        {
            ClearSlot();
            return;
        }

        iconImage.sprite = itemType.Icon;
        iconImage.color = Color.white;
        countText.text = s;
    }

    public void ClearSlot()
    {
        iconImage.sprite = null;
        iconImage.color = Color.clear;
        countText.text = string.Empty;
    }
}