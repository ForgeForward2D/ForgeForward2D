using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private GameObject visualPanel;

    private List<InventorySlotUI> uiSlots = new();

    public bool IsOpen => visualPanel.activeSelf;

    private void Awake()
    {
        uiSlots.AddRange(GetComponentsInChildren<InventorySlotUI>(true));
    }

    private void OnEnable()
    {
        inventoryManager.OnInventoryChanged += RefreshUI;
        RefreshUI();
    }

    private void OnDisable() => inventoryManager.OnInventoryChanged -= RefreshUI;
    
    public void RefreshUI()
    {
        var items = inventoryManager.GetItems();
        for(int i = 0; i < uiSlots.Count; i++)
        {
            uiSlots[i].UpdateSlot(i < items.Count ? items[i] : null);
        }
    }

    public void Toggle()
    {
        visualPanel.SetActive(!visualPanel.activeSelf);
        bool isOpen = visualPanel.activeSelf;

        if(isOpen)
        {
            RefreshUI();
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
}
