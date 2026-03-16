using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourceInventoryUI : UIComponent<ResourceInventory>
{
    public static event Action RequestRefresh;

    [SerializeField] private GameObject resourceInventorySlotPrefab;

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

        if (children.Count < items.Count)
        {
            int missingChildren = items.Count - children.Count;
            Debug.Log($"Creating {missingChildren} new slots for resource inventory");
            for (int i = 0; i < missingChildren; i++)
            {
                GameObject newSlot = Instantiate(resourceInventorySlotPrefab, new Vector3(0f, 0f, 0f), Quaternion.identity, gameObject.transform);
                ItemSlotUI slotUI = newSlot.GetComponent<ItemSlotUI>();
                if (slotUI != null && slotUI is UIComponentBase slotUIComponentBase)
                {
                    children.Add(slotUIComponentBase);
                }
                else
                {
                    Debug.LogError("Error instantiating new resource inventory item slot");
                }
            }
        }
        else if (children.Count > items.Count)
        {
            int slotsToRemove = children.Count - items.Count;
            Debug.Log($"Removing {slotsToRemove} excess slots from resource inventory");

            for (int i = 0; i < slotsToRemove; i++)
            {
                int lastIndex = children.Count - 1;
                UIComponentBase childToRemove = children[lastIndex];
                children.RemoveAt(lastIndex);
                if (childToRemove != null)
                {
                    Destroy(childToRemove.gameObject);
                }
            }
        }

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