using System.Collections;
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
            StartCoroutine(SetInitialHighlight());
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (targetContainer is ToolHotbar hotbar)
        {
            hotbar.OnSelectionChanged -= UpdateHighlight;
        }
    }

    private IEnumerator SetInitialHighlight()
    {
        yield return new WaitForEndOfFrame();
        UpdateHighlight();
    }

    private void UpdateHighlight()
    {
        if (targetContainer is ToolHotbar hotbar && selectionHighlight != null && uiSlots.Length > 0)
        {
            RectTransform targetSlot = uiSlots[hotbar.SelectedIndex].GetComponent<RectTransform>();
            selectionHighlight.position = targetSlot.position;
            selectionHighlight.sizeDelta = targetSlot.sizeDelta + new Vector2(10, 10);;
        }
    }
}