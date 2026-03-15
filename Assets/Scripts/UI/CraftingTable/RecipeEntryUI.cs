using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeEntryUI : UIComponent<CraftingRecipePreview>
{
    [SerializeField] private List<UIComponentBase> childrenView;
    public void Start()
    {
        childrenView = GetChildren();
    }

    public override void RefreshUI(CraftingRecipePreview recipePreview)
    {
        List<UIComponentBase> children = GetChildren();
        for (int i = 0; i < children.Count - 1; i++)
        {
            if (i < recipePreview.ingredients.Count)
            {
                var slotAvailability = recipePreview.ingredients[i];
                int requiredCount = slotAvailability.required.count;
                int availableCount = slotAvailability.available;
                ItemWithText ingredientContent = new ItemWithText(slotAvailability.required, $"{availableCount}/{requiredCount}");
                children[i].RefreshUIDynamic(ingredientContent);
            }
            else
            {
                children[i].RefreshUIDynamic(null);
            }
        }

        ItemWithText resultContent = new ItemWithText(recipePreview.result, null);
        children[children.Count - 1].RefreshUIDynamic(resultContent);
    }
}