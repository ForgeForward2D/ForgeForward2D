using UnityEngine;
using UnityEngine.InputSystem;

public class HotbarInput : MonoBehaviour
{
    [SerializeField] private ToolHotbar toolHotbar;

    private void Update()
    {
        if (Keyboard.current == null || Mouse.current == null) return;

        HandleNumberKeys();
        HandleScrollWheel();
    }

    private void HandleNumberKeys()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame) toolHotbar.ChangeSelectedSlot(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) toolHotbar.ChangeSelectedSlot(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) toolHotbar.ChangeSelectedSlot(2);
        if (Keyboard.current.digit4Key.wasPressedThisFrame) toolHotbar.ChangeSelectedSlot(3);
        if (Keyboard.current.digit5Key.wasPressedThisFrame) toolHotbar.ChangeSelectedSlot(4);
    }

    private void HandleScrollWheel()
    {
        float scrollY = Mouse.current.scroll.ReadValue().y;

        if (scrollY != 0f)
        {
            int currentIndex = toolHotbar.SelectedIndex;
            int hotbarSize = toolHotbar.GetItems().Length;

            if (scrollY > 0f)
            {
                currentIndex--;
                if (currentIndex < 0) currentIndex = hotbarSize - 1;
            }
            else if (scrollY < 0f)
            {
                currentIndex++;
                if (currentIndex >= hotbarSize) currentIndex = 0;
            }

            toolHotbar.ChangeSelectedSlot(currentIndex);
        }
    }
}