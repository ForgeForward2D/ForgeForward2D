using UnityEngine;

public class ResourceInventoryUI : ContainerUI
{
    [SerializeField] private GameObject visualPanel;

    public bool IsOpen => visualPanel != null && visualPanel.activeSelf;

    public void Toggle()
    {
        visualPanel.SetActive(!visualPanel.activeSelf);
        bool isOpen = visualPanel.activeSelf;
        
        if(isOpen) RefreshUI();

        Time.timeScale = isOpen ? 0f : 1f;
        // Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
        // Cursor.visible = isOpen;
    }
}