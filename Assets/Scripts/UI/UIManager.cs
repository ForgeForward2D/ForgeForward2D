using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static event Action<UIPage> OnUpdatePage;

    [SerializeField] private ResourceInventoryUI resourceInventoryUI;
    [SerializeField] private CraftingTableUI craftingTableUI;
    [SerializeField] private BlockType craftingTableBlockType;
    [SerializeField] private BlockType anvilBlockType;
    [SerializeField] private AchievementUI achievementUI;

    private InventoryManager inventoryManager;

    void Awake()
    {
        InputManager.OnUIChangeInput += ProcessNavigationRequest;
        PlayerInteractionManager.OnInteraction += HandleInteraction;
        inventoryManager = FindAnyObjectByType<InventoryManager>();
    }

    private void ProcessNavigationRequest(UIPage currentPage, UIPage requestedPage)
    {
        UIPage targetPage = GetTargetPage(currentPage, requestedPage);

        if (currentPage != targetPage)
            SetPage(targetPage);
    }

    private UIPage GetTargetPage(UIPage currentPage, UIPage requestedPage)
    {
        // If no page is open, open the page
        if (currentPage == UIPage.None)
        {
            return requestedPage;
        }

        // If exit was requested, exit
        if (requestedPage == UIPage.None)
        {
            return UIPage.None;
        }

        // If the open page was requested again, toggle
        if (requestedPage == currentPage)
        {
            return UIPage.None;
        }

        // Do nothing
        return currentPage;
    }

    private void SetPage(UIPage page)
    {
        resourceInventoryUI.SetActive(page == UIPage.Inventory);
        craftingTableUI.SetActive(page == UIPage.Crafting);
        achievementUI.SetActive(page == UIPage.Achievements);

        bool open = page != UIPage.None;
        Time.timeScale = open ? 0f : 1f;
        OnUpdatePage?.Invoke(page);
    }

    private void HandleInteraction((UIPage, BlockType, Vector2Int) data)
    {
        var (uiPage, blockType, position) = data;

        if (blockType == null) return;

        if (blockType == craftingTableBlockType)
        {
            craftingTableUI.SetCraftingManager(inventoryManager.craftingTableManager);
            ProcessNavigationRequest(uiPage, UIPage.Crafting);
        }
        else if (blockType == anvilBlockType)
        {
            craftingTableUI.SetCraftingManager(inventoryManager.anvilManager);
            ProcessNavigationRequest(uiPage, UIPage.Crafting);
        }
    }
}