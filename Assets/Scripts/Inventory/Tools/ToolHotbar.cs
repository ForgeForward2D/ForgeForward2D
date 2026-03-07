using System;
using UnityEngine;

public class ToolHotbar : ItemContainer
{
    [System.Serializable]
    public struct StartingTool { public int itemId;}

    [SerializeField] private StartingTool[] startingTools;

    [SerializeField] private WorldInteractionManager worldInteractionManager;
    public int SelectedIndex = 0;

    public event Action OnSelectionChanged;

    private void Start()
    {
        for (int i = 0; i < capacity && i < startingTools.Length; i++)
        {
            if (startingTools[i].itemId != 0)
            {
                ItemType data = ItemTypeRepository.GetItemById(startingTools[i].itemId);
                if (data != null) items[i] = new InventoryItem(data, 1);
            }
        }
        NotifyContentsChanged();
    }

    private void OnEnable()
    {
        if (worldInteractionManager != null)
        {
            worldInteractionManager.OnRequestActiveTool += GetSelectedTool;
        }
    }

    private void OnDisable()
    {
        if (worldInteractionManager != null)
        {
            worldInteractionManager.OnRequestActiveTool -= GetSelectedTool;
        }
    }

    public void ChangeSelectedSlot(int index)
    {
        if (index >= 0 && index < capacity)
        {
            SelectedIndex = index;
            OnSelectionChanged?.Invoke();
        }
    }

    public InventoryItem GetSelectedTool() => items[SelectedIndex];
}