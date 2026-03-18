using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/CraftingRecipe")]
public class CraftingRecipe : ScriptableObject
{
    public List<Item> ingredients;
    public Item result;

    private void OnValidate() {
        Debug.Assert(ingredients != null, $"CraftingRecipe {name} has null ingredients list.");
        Debug.Assert(ingredients.Count > 0, $"CraftingRecipe {name} has no ingredients.");
        Debug.Assert(ingredients.Count <= 5, $"CraftingRecipe {name} has more than 5 ingredients, which may cause UI issues.");
        foreach (var ingredient in ingredients) {
            Debug.Assert(ingredient.itemType != null, $"CraftingRecipe {name} has an ingredient with null item type.");
            Debug.Assert(ingredient.count > 0, $"CraftingRecipe {name} has an ingredient with non-positive count.");
        }

        Debug.Assert(result != null, $"CraftingRecipe {name} has null result.");
        Debug.Assert(result.itemType != null, $"CraftingRecipe {name} has a result with null item type.");
        Debug.Assert(result.count > 0, $"CraftingRecipe {name} has a result with non-positive count.");
    }
}