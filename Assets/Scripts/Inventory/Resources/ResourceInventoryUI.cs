using UnityEngine;

public class ResourceInventoryUI : ContainerUI
{
    [SerializeField] private GameObject visualPanel;

    public bool IsOpen => visualPanel != null && visualPanel.activeSelf;

    public void SetActive(bool active)
    {
        if (visualPanel == null) return;
        if (active == visualPanel.activeSelf) return;

        visualPanel.SetActive(active);

        if (active) RefreshUI();

        Time.timeScale = active ? 0f : 1f;
    }
}