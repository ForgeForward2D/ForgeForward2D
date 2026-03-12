using System;
using System.Collections.Generic;
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

    public void SetActive() {
        if (visualPanel == null) return;
        visualPanel.SetActive(true);
        RefreshUI();
        Time.timeScale = 0f;
    }

    public void SetInactive() {
        if (visualPanel == null) return;
        visualPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void RefreshUI() {
        if (recipeEntries == null) return;

        List<CraftingRecipe> availableRecipes = CraftingRecipeRepository.GetAllRecipes();

        Debug.Log("Refresh CraftingTableUI: Found " + availableRecipes.Count + " recipes in repository for "+recipeEntries.Count+" slots.");

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
        Debug.Log("Scrolling selected recipe by " + delta);

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
        RecipeEntryUI entryUi = recipeEntries[selectedRecipeIndex % recipeEntries.Count];
        return entryUi != null ? entryUi.AssignedRecipe : null;
    }

}