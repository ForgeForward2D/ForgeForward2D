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
    [SerializeField] private List<CraftingRecipe> availableRecipes = new List<CraftingRecipe>();
    [SerializeField] private int selectedRecipeIndex = 0;

    public bool IsOpen => visualPanel != null && visualPanel.activeSelf;

    private void Awake()
    {
        recipeEntries = new List<RecipeEntryUI>(GetComponentsInChildren<RecipeEntryUI>(true));
    }

    public void SetActive(bool active) {
        if (visualPanel == null) return;
        if (active == visualPanel.activeSelf) return;

        visualPanel.SetActive(active);
        if (active)
            RefreshUI();
        Time.timeScale = active ? 0f : 1f;
    }

    public void RefreshUI() {
        if (recipeEntries == null) return;

        availableRecipes = CraftingRecipeRepository.GetAllRecipes()
            .Where(recipe => playerInventory.CountFreeSpace(recipe.result.itemType) >= recipe.result.count)
            .ToList();
        selectedRecipeIndex = selectedRecipeIndex > availableRecipes.Count - 1 ? 0 : selectedRecipeIndex;

        for (int i = 0; i < recipeEntries.Count; i++)
        {
            if (i < availableRecipes.Count)
            {
                CraftingRecipe recipe = availableRecipes[(selectedRecipeIndex + i) % availableRecipes.Count];
                recipeEntries[i].SetActive(true);
                recipeEntries[i].Refresh(recipe, playerInventory);
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

        // Wrap around the recipe index using c# remainder operator (there is no built in modulo operator)
        selectedRecipeIndex %= availableRecipes.Count;
        selectedRecipeIndex += availableRecipes.Count;
        selectedRecipeIndex %= availableRecipes.Count;

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