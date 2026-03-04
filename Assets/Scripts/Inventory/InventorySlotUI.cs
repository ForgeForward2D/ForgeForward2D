using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI countText;

    public void SetItem(ItemType item, int count)
    {
        if (item == null || iconImage == null)
        {
            return;
        }

        iconImage.sprite = item.icon;
        iconImage.color = Color.white;

        if(countText != null)
        {
        countText.text = count > 1 ? count.ToString() : "";
        }
    }

    public void ClearSlot()
    {
        if(iconImage == null)
        {
            return;
        }

        iconImage.sprite = null;
        iconImage.color = new Color(1, 1, 1, 0);
        if(countText != null)
        {
            countText.text = "";
        }
    }
}