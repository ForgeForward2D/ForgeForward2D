using UnityEngine;

public class PlayerHandController : MonoBehaviour
{
    [SerializeField] private ToolHotbar toolHotbar;
    [SerializeField] private Transform socketRightHand;

    private GameObject currentHandItem;

    private void OnEnable()
    {
        if (toolHotbar != null)
        {
            toolHotbar.OnSelectionChanged += UpdateHandItem;
            toolHotbar.OnContentChanged += UpdateHandItem;
        }
    }

    private void OnDisable()
    {
        if (toolHotbar != null)
        {
            toolHotbar.OnSelectionChanged -= UpdateHandItem;
            toolHotbar.OnContentChanged -= UpdateHandItem;
        }
    }

    private void UpdateHandItem()
    {
        if (currentHandItem != null)
        {
            Destroy(currentHandItem);
            currentHandItem = null;
        }

        Tool selected = toolHotbar.GetSelectedTool();

        if (selected == null || selected.Prefab == null)
            return;

        currentHandItem = Instantiate(selected.Prefab, socketRightHand);
        currentHandItem.transform.localPosition = Vector3.zero;
        currentHandItem.transform.localRotation = Quaternion.identity;
        currentHandItem.transform.localScale = Vector3.one;
    }
}
