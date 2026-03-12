using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CraftingTableUI : MonoBehaviour
{
    [SerializeField] private GameObject visualPanel;
    [SerializeField] private ResourceInventory playerInventory;


    [Header("Debugging")]
    [SerializeField] private List<RecipeEntryUI> recipeEntries = new List<RecipeEntryUI>();
    [SerializeField] private int selectedRecipeIndex = 0;

    public bool IsOpen => visualPanel != null && visualPanel.activeSelf;

    private void Awake()
    {
        recipeEntries = new List<RecipeEntryUI>(GetComponentsInChildren<RecipeEntryUI>(true));
    }

    public void SetActive(bool active) {
        if (visualPanel == null) return;
        visualPanel.SetActive(active);
        if (active) 
            RefreshUI();
        Time.timeScale = active ? 0f : 1f;
    }

    public void RefreshUI() {
        if (recipeEntries == null) return;

        List<CraftingRecipe> availableRecipes = CraftingRecipeRepository.GetAllRecipes()
            .Where(recipe => playerInventory.CountFreeSpace(recipe.result.Item) >= recipe.result.Count)
            .ToList();
        selectedRecipeIndex = selectedRecipeIndex > availableRecipes.Count - 1 ? 0 : selectedRecipeIndex;

        for (int i = 0; i < recipeEntries.Count; i++)
        {
            if (i < availableRecipes.Count)
            {
                CraftingRecipe recipe = availableRecipes[(selectedRecipeIndex + i) % availableRecipes.Count];
                recipeEntries[i].Refresh(recipe, playerInventory);
                recipeEntries[i].SetActive(true);
            }
            else
            {
                recipeEntries[i].SetActive(false);
            }
        }
    }

    public void ScrollSelectedRecipe(int delta)
    {
        selectedRecipeIndex += delta;
        if (selectedRecipeIndex < 0)
            selectedRecipeIndex += CraftingRecipeRepository.GetAllRecipes().Count;
        if (selectedRecipeIndex >= CraftingRecipeRepository.GetAllRecipes().Count)
            selectedRecipeIndex -= CraftingRecipeRepository.GetAllRecipes().Count;
        RefreshUI();
    }

    public CraftingRecipe GetSelectedRecipe()
    {
        if (recipeEntries == null || recipeEntries.Count == 0)
            return null;
        RecipeEntryUI entryUi = recipeEntries[0].gameObject.activeSelf ? recipeEntries[0] : null;
        return entryUi != null ? entryUi.AssignedRecipe : null;
    }

}