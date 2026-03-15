using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceInventoryUI : UIComponent<ResourceInventory>
{
    public static event Action RequestRefresh;
    public void OnEnable()
    {
        ResourceInventory.OnResourceInventoryUpdate += RefreshUI;
        RequestRefresh?.Invoke();
    }

    private void OnDisable()
    {
        ResourceInventory.OnResourceInventoryUpdate -= RefreshUI;
    }

    public override void RefreshUI(ResourceInventory inventory)
    {
        List<UIComponentBase> children = GetChildren();
        List<Item> items = inventory.GetItems();
        for (int i = 0; i < children.Count; i++)
        {
            if (i < items.Count)
            {
                ItemWithText itemWithText = new ItemWithText(items[i], null);
                children[i].RefreshUIDynamic(itemWithText);
            }
            else
            {
                children[i].RefreshUIDynamic(null);
            }
        }
    }
}