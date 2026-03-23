using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static event Action<UIPage> OnUpdatePage;

    [SerializeField] private ResourceInventoryUI resourceInventoryUI;
    [SerializeField] private CraftingTableUI craftingTableUI;
    [SerializeField] private BlockType craftingTableBlockType;
    [SerializeField] private AchievementUI achievementUI;
    [SerializeField] private DialogueUI dialogueUI;

    [Header("Debugging")]
    [SerializeField] private NpcController activeNpc;

    void Awake()
    {
        InputManager.OnUIChangeInput += ProcessNavigationRequest;
        PlayerInteractionManager.OnInteraction += HandleInteraction;
        NpcController.OnNpcInteraction += HandleNpcInteraction;
        NpcController.OnNpcInteractionEnd += HandleNpcInteractionEnd;
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
        if (page != UIPage.Dialogue && activeNpc != null)
        {
            NpcController npc = activeNpc;
            activeNpc = null;
            npc.EndDialogue();
        }

        resourceInventoryUI.SetActive(page == UIPage.Inventory);
        craftingTableUI.SetActive(page == UIPage.Crafting);
        achievementUI.SetActive(page == UIPage.Achievements);
        dialogueUI.SetActive(page == UIPage.Dialogue);

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

    private void HandleNpcInteraction((UIPage uiPage, NpcController npc) data)
    {
        if (data.uiPage != UIPage.None) return;

        activeNpc = data.npc;
        SetPage(UIPage.Dialogue);
        data.npc.BeginDialogue();
    }

    private void HandleNpcInteractionEnd()
    {
        activeNpc = null;
        SetPage(UIPage.None);
    }
}
