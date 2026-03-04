using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform container;

    [SerializeField] private GameObject visualPanel;

    private List<InventorySlotUI> uiSlots = new List<InventorySlotUI>();

    private void Awake()
    {
        uiSlots.AddRange(container.GetComponentsInChildren<InventorySlotUI>(true));
    }

    private void OnEnable()
    {
        RefreshUI();
    }
    
    public void RefreshUI()
    {
        var items = inventoryManager.GetInventoryItems();

        for(int i = 0; i < uiSlots.Count; i++)
        {
            if (i < items.Count)
            {
                uiSlots[i].SetItem(items[i].Item, items[i].Count);
            }
            else
            {
                uiSlots[i].ClearSlot();
            }
        }
    }

    public void ToggleInventory()
    {
        if(visualPanel != null)
        {
            bool isActive = !visualPanel.activeSelf;
            visualPanel.SetActive(isActive);

            Debug.Log("Inventory Panel is now: " + (isActive ? "Visible" : "Hidden"));

            if (isActive)
            {
                RefreshUI();
            }
        }
    }
}
