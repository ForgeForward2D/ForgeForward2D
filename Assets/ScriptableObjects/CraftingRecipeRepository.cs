using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

public static class CraftingRecipeRepository
{
    private static Dictionary<int, CraftingRecipe> idLookup;
    private static Dictionary<ItemType, List<CraftingRecipe>> resultLookup;

    private static void Initialize()
    {
        var craftingRecipes = Resources.LoadAll<CraftingRecipe>("CraftingRecipes");

        idLookup = new Dictionary<int, CraftingRecipe>();
        resultLookup = new Dictionary<ItemType, List<CraftingRecipe>>();

        foreach (var recipe in craftingRecipes)
        {
            idLookup[recipe.id] = recipe;
            if (recipe.result != null)
            {
                if (recipe.result.Item == null)
                {
                    Debug.LogWarning($"CraftingRecipe {recipe.name} has a null result item.");
                    continue;
                }
                if (!resultLookup.ContainsKey(recipe.result.Item))
                {
                    resultLookup[recipe.result.Item] = new List<CraftingRecipe>();
                }
                resultLookup[recipe.result.Item].Add(recipe);
            }
        }


        Debug.Log("Initialized CraftingRecipeRepository with " + craftingRecipes.Length + " recipes.");
    }

    public static CraftingRecipe GetRecipeById(int id)
    {
        if (idLookup == null)
            Initialize();
        return idLookup.GetValueOrDefault(id);
    }

    public static List<CraftingRecipe> GetRecipesByResult(ItemType result)
    {
        if (resultLookup == null)
            Initialize();
        return resultLookup.GetValueOrDefault(result, new List<CraftingRecipe>());
    }

    public static List<CraftingRecipe>GetAllRecipes()
    {
        if (idLookup == null)
            Initialize();
        return idLookup.Values.ToList();
    }
}