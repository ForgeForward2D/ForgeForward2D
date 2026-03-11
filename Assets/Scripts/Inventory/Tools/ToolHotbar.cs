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
            Debug.Log("No item in selected hotbar slot.");
            return ItemTypeRepository.GetToolById(0); // Return a default "empty" tool
        }

        if (item is Tool tool)
        {
            Debug.Log($"Selected tool: {tool.name} (Type: {tool.type}, Tier: {tool.tier}, Efficiency: {tool.efficiency})");
            return tool;
        }
        Debug.Log("Selected item is not a tool or is null.");
        return ItemTypeRepository.GetToolById(item.Id);
    }
}