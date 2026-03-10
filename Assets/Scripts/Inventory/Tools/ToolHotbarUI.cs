using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolHotbarUI : ContainerUI
{
    [SerializeField] private RectTransform selectionHighlight;

    private List<RectTransform> slotTransforms = new List<RectTransform>();

    protected override void Awake()
    {
        base.Awake();
        CacheSlotTransforms();
    }

    private void CacheSlotTransforms()
    {
        slotTransforms.Clear();
        foreach (var slot in uiSlots)
        {
            if (slot != null)
            {
                slotTransforms.Add(slot.GetComponent<RectTransform>());
            }
        }
    }

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
        if (targetContainer is ToolHotbar hotbar && selectionHighlight != null)
        {
            int index = hotbar.SelectedIndex;

            if (index >= 0 && index < slotTransforms.Count)
            {
                RectTransform targetSlot = slotTransforms[index];

                selectionHighlight.position = targetSlot.position;
                selectionHighlight.sizeDelta = targetSlot.sizeDelta + new Vector2(10, 10);
            }
        }
    }
}