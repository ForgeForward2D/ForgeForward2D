using UnityEngine;

public class ToolHotbarUI : ContainerUI
{
    [SerializeField] private RectTransform selectionHighlight;

    protected override void OnEnable()
    {
        base.OnEnable();
        if (targetContainer is ToolHotbar hotbar)
        {
            hotbar.OnSelectionChanged += UpdateHighlight;
            UpdateHighlight();
        }
    }

    private void UpdateHighlight()
    {
        if (targetContainer is ToolHotbar hotbar && selectionHighlight != null)
        {
            selectionHighlight.position = uiSlots[hotbar.SelectedIndex].transform.position;
        }
    }
}