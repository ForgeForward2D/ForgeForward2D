using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private ResourceInventoryUI resourceInventoryUI;

    [SerializeField] private CraftingTableUI craftingTableUI;
    [SerializeField] private BlockType craftingTableBlockType;

    public static event Action<UIPage> OnUpdatePage;

    void Awake()
    {
        InputManager.OnUIChangeInput += ProcessNavigationRequest;
        PlayerInteractionManager.OnInteraction += HandleInteraction;
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

        bool open = page != UIPage.None; 
        Time.timeScale = open ? 0f : 1f;
        OnUpdatePage?.Invoke(page);
    }

    private void HandleInteraction((UIPage, BlockType, Vector2Int) data)
    {
        var (uiPage, blockType, position) = data;

        if (blockType == craftingTableBlockType)
        {
            ProcessNavigationRequest(uiPage, UIPage.Crafting); 
        }

    }
}