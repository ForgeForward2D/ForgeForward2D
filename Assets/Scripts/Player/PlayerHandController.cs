using UnityEngine;

public class PlayerHandController : MonoBehaviour
{
    [SerializeField] private ToolHotbar toolHotbar;
    [SerializeField] private Transform socketRightHand;

    private GameObject _currentHandItem;

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
        if (_currentHandItem != null)
        {
            Destroy(_currentHandItem);
            _currentHandItem = null;
        }

        InventoryItem selected = toolHotbar.GetSelectedTool();

        if (selected == null || selected.Item == null || selected.Item.Prefab == null)
            return;

        _currentHandItem = Instantiate(selected.Item.Prefab, socketRightHand);
        _currentHandItem.transform.localPosition = Vector3.zero;
        _currentHandItem.transform.localRotation = Quaternion.identity;
        _currentHandItem.transform.localScale = Vector3.one;
    }
}
