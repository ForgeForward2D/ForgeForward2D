using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CraftingTableUI : UIComponent<CraftingManager>
{
    public static event Action RequestRefresh;

    public void OnEnable()
    {
        CraftingManager.OnCraftingManagerUpdate += RefreshUI;
        RequestRefresh?.Invoke();
    }

    private void OnDisable()
    {
        CraftingManager.OnCraftingManagerUpdate -= RefreshUI;
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