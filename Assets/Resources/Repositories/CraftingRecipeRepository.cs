using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class CraftingRecipeRepository
{
    private static List<CraftingRecipe> recipes;

    private static void Initialize()
    {
        var craftingRecipes = Resources.LoadAll<CraftingRecipe>("CraftingRecipes");
        recipes = craftingRecipes.ToList();
        Debug.Log($"Initialized CraftingRecipeRepository with {recipes.Count} recipes.");
    }

    public static List<CraftingRecipe>GetAllRecipes()
    {
        if (recipes == null)
            Initialize();
        return recipes;
    }
}