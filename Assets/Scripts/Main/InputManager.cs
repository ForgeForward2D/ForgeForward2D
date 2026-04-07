using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum UIPage
{
    None,
    Inventory,
    Crafting,
    Achievements,
    Dialogue,
}

public class InputManager : MonoBehaviour
{
    [SerializeField] private GameConfig gameConfig;

    [Header("Debugging")]
    [SerializeField] private UIPage uiPage = UIPage.None;

    private void Awake()
    {
        UIManager.OnUpdatePage += HandleUpdatePage;
    }

    private void HandleUpdatePage(UIPage uiPage)
    {
        this.uiPage = uiPage;
    }

    public static event Action<(UIPage, bool, Vector2)> OnMoveInput;
    public void Move(InputAction.CallbackContext context)
    {
        bool performed = context.phase == InputActionPhase.Performed;
        Vector2 input = context.ReadValue<Vector2>();
        OnMoveInput?.Invoke((uiPage, performed, input));
    }

    public static event Action<(UIPage, bool)> OnAttackUpdate;
    public void Attack(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                OnAttackUpdate?.Invoke((uiPage, true));
                break;
            case InputActionPhase.Canceled:
                OnAttackUpdate?.Invoke((uiPage, false));
                break;
        }
    }

    // Interact
    public static event Action<UIPage> OnInteractionInput;
    public void Interact(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed) return;
        OnInteractionInput?.Invoke(uiPage);
    }

    // UI
    public static event Action<UIPage, UIPage> OnUIChangeInput;
    public void Escape(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed) return;
        OnUIChangeInput?.Invoke(uiPage, UIPage.None);
    }
    public void Inventory(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed) return;
        OnUIChangeInput?.Invoke(uiPage, UIPage.Inventory);
    }
    public void AchievementMenu(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed) return;
        OnUIChangeInput?.Invoke(uiPage, UIPage.Achievements);
    }


    // Tutorial Dialogue
    public static event Action<UIPage> OnTutorialInput;
    public void TutorialDialogue(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Performed) return;
        OnTutorialInput?.Invoke(uiPage);
    }

    // HotBar
    public static event Action<(UIPage, int)> OnHotBarSelected;
    public static event Action<(UIPage, int)> OnHotBarScroll;
    public void HotBarSelected(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        string keyName = context.control.name;

        if (int.TryParse(keyName, out int digit))
        {
            OnHotBarSelected?.Invoke((uiPage, digit - 1));
        }
    }

    private float scrollAccumulator = 0f;

    public void HotBarScroll(InputAction.CallbackContext context)
    {
        // Use .performed or .started depending on your Input Action settings
        if (!context.performed) return;

        Vector2 scrollValue = context.ReadValue<Vector2>();
        scrollAccumulator += scrollValue.y;

        // Only step if we've scrolled past the threshold
        if (Mathf.Abs(scrollAccumulator) >= gameConfig.scrollThreshold)
        {
            // Determine direction
            int step = scrollAccumulator > 0 ? -1 : 1;

            // Reset accumulator (or subtract threshold to allow "fast" scrolling)
            scrollAccumulator = 0f;

            OnHotBarScroll?.Invoke((uiPage, step));
        }
    }
}
