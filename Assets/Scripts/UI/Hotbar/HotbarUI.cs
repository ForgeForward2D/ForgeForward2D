using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotBarUI : UIComponent<HotBar>
{
    public static event Action RequestRefresh;

    [SerializeField] private RectTransform selectionHighlight;
    private List<RectTransform> slotTransforms = new List<RectTransform>();

    public void Start()
    {
        Debug.Log("HotBarUI setup");
        List<UIComponentBase> children = GetChildren();
        slotTransforms.Clear();
        foreach (var child in children)
        {
            if (child != null)
            {
                slotTransforms.Add(child.GetComponent<RectTransform>());
            }
        }

        // Make the slot transform have the correct values for the initial refresh
        Canvas.ForceUpdateCanvases();

        HotBar.OnHotBarUpdate += RefreshUI;
        RequestRefresh?.Invoke();
    }

    public override void RefreshUI(HotBar hotBar)
    {
        List<UIComponentBase> children = GetChildren();
        List<Tool> currentTools = hotBar.GetCurrentTools();
        if (children.Count != currentTools.Count)
            Debug.LogError($"Error in hot bar Refresh: got {currentTools.Count} tools for {children.Count} slots");
        for (int i = 0; i < children.Count; i++)
        {
            Debug.Assert(children[i] is ItemSlotUI, $"Child of HotBarUI at index {i} is not a ItemSlotUI");
            ItemWithText content = new ItemWithText(new Item(currentTools[i], 1), "");
            children[i].RefreshUIDynamic(content);
        }

        if (hotBar != null && selectionHighlight != null)
        {
            int index = hotBar.GetSelectedIndex();

            if (index >= 0 && index < slotTransforms.Count)
            {
                RectTransform targetSlot = slotTransforms[index];

                selectionHighlight.position = targetSlot.position;
                selectionHighlight.sizeDelta = targetSlot.sizeDelta + new Vector2(10, 10);
            }
            else
            {
                Debug.LogWarning("Could not set selection highlight");
            }
        }
    }
}