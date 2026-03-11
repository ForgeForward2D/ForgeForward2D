using System;
using UnityEngine;

public class ToolHotbar : ItemContainer
{
    [SerializeField] private int[] startingItemIds;

    public int SelectedIndex = 0;

    public event Action OnSelectionChanged;

    private void Start()
    {
        int count = Mathf.Min(capacity, startingItemIds.Length);

        for (int i = 0; i < count; i++)
        {
            int itemId = startingItemIds[i];

            if (itemId != 0)
            {
                ItemType data = ItemTypeRepository.GetItemById(itemId);
                if (data != null)
                {
                    items[i] = new InventoryItem(data, 1);
                }
                else
                {
                    Debug.LogWarning($"No item found for ID {itemId} in ToolHotbar starting items.");
                }
            }
        }
        OnSelectionChanged?.Invoke();
        NotifyContentsChanged();
    }

    public void ChangeSelectedSlot(int index)
    {
        if (index >= 0 && index < capacity)
        {
            SelectedIndex = index;
            OnSelectionChanged?.Invoke();
        }
    }

    public Tool GetSelectedTool()
    {

        ItemType item = items[SelectedIndex]?.Item;

        if (item == null)
        {
            return ItemTypeRepository.GetDefaultTool(); // Return a default "empty" tool
        }

        if (item is Tool tool)
        {
            return tool;
        }
        return ItemTypeRepository.GetToolById(item.Id);
    }
}