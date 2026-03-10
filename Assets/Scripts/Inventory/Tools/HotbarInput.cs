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
        var kb = Keyboard.current;
        
        if (kb.digit1Key.wasPressedThisFrame) toolHotbar.ChangeSelectedSlot(0);
        else if (kb.digit2Key.wasPressedThisFrame) toolHotbar.ChangeSelectedSlot(1);
        else if (kb.digit3Key.wasPressedThisFrame) toolHotbar.ChangeSelectedSlot(2);
        else if (kb.digit4Key.wasPressedThisFrame) toolHotbar.ChangeSelectedSlot(3);
        else if (kb.digit5Key.wasPressedThisFrame) toolHotbar.ChangeSelectedSlot(4);
    }

    private void HandleScrollWheel()
    {
        float scrollY = Mouse.current.scroll.ReadValue().y;

        if (scrollY == 0f) return;
        
        int size = toolHotbar.GetItems().Length;
        int step = scrollY > 0 ? -1 : 1;

        int nextIndex = (toolHotbar.SelectedIndex + step + size) % size;
        toolHotbar.ChangeSelectedSlot(nextIndex);
    }
}