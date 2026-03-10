using UnityEngine;

public class ResourceInventoryUI : ContainerUI
{
    [SerializeField] private GameObject visualPanel;

    public bool IsOpen => visualPanel != null && visualPanel.activeSelf;

    public void Toggle()
    {
        if (visualPanel == null) return;

        bool newState = !visualPanel.activeSelf;
        visualPanel.SetActive(newState);

        if (newState) RefreshUI();

        Time.timeScale = newState ? 0f : 1f;
    }
}