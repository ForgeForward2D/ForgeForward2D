using UnityEngine;

public abstract class ContainerUI : MonoBehaviour
{
    [SerializeField] protected ItemContainer targetContainer;
    protected InventorySlotUI[] uiSlots;

    protected virtual void Awake()
    {
        uiSlots = GetComponentsInChildren<InventorySlotUI>(true);
    }

    protected virtual void OnEnable()
    {
        if (targetContainer != null)
        {
            targetContainer.OnContentChanged += RefreshUI;
            RefreshUI();
        }
    }

    protected virtual void OnDisable()
    {
        if (targetContainer != null)
        {
            targetContainer.OnContentChanged -= RefreshUI;
        }
    }

    protected virtual void RefreshUI()
    {
        var items = targetContainer.GetItems();
        for (int i = 0; i < uiSlots.Length; i++)
        {
            if (i < items.Length)
            {
                uiSlots[i].UpdateSlot(items[i]);
            }
            else
            {
                uiSlots[i].ClearSlot();
            }
        }
    }
}