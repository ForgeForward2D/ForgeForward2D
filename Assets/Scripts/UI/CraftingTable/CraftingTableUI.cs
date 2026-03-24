using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CraftingTableUI : UIComponent<CraftingManager>
{
    private CraftingManager craftingManager;

    public void SetCraftingManager(CraftingManager manager)
    {
        if (craftingManager == manager) return;

        if (isActiveAndEnabled && craftingManager != null)
        {
            craftingManager.SetActive(false);
            craftingManager.OnCraftingManagerUpdate -= RefreshUI;
        }

        craftingManager = manager;

        if (isActiveAndEnabled && craftingManager != null)
        {
            craftingManager.SetActive(true);
            craftingManager.OnCraftingManagerUpdate += RefreshUI;
            craftingManager.RequestRefresh();
        }
    }

    public void OnEnable()
    {
        if (craftingManager == null) return;
        craftingManager.SetActive(true);
        craftingManager.OnCraftingManagerUpdate += RefreshUI;
        craftingManager.RequestRefresh();
    }

    private void OnDisable()
    {
        if (craftingManager == null) return;
        craftingManager.SetActive(false);
        craftingManager.OnCraftingManagerUpdate -= RefreshUI;
    }

    public override void RefreshUI(CraftingManager craftingManager)
    {
        List<UIComponentBase> children = GetChildren();
        List<CraftingRecipePreview> recipePreviews = craftingManager.GetPreviews(children.Count);
        int selectedRecipeIndex = craftingManager.GetSelectedRecipeIndex();
        for (int i = 0; i < children.Count; i++)
        {
            if (recipePreviews[i] != null)
            {
                Debug.Assert(children[i] is RecipeEntryUI, $"Child of CraftingTableUI at index {i} is not a RecipeEntryUI");
                children[i].SetActive(true);
                children[i].RefreshUIDynamic(recipePreviews[i]);
            }
            else
            {
                children[i].SetActive(false);
            }
        }
    }
}