using UnityEngine;
using UnityEngine.InputSystem;

public class HotbarInput : MonoBehaviour
{
    [SerializeField] private ToolHotbar toolHotbar;

    public void OnHotbarSelected(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        string keyName = context.control.name;

        if (int.TryParse(keyName, out int digit))
        {
            toolHotbar.ChangeSelectedSlot(digit - 1);
        }
    }

    public void OnHotbarScroll(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        Vector2 scrollValue = context.ReadValue<Vector2>();

        if (scrollValue.y == 0f) return;

        int size = toolHotbar.GetItems().Length;
        int step = scrollValue.y > 0 ? -1 : 1;

        int nextIndex = (toolHotbar.SelectedIndex + step + size) % size;
        toolHotbar.ChangeSelectedSlot(nextIndex);
    }
}