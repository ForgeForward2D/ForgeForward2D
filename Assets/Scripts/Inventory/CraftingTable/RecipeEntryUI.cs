using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeEntryUI : MonoBehaviour
{
    public CraftingRecipe AssignedRecipe { get; private set; }


    private List<InventorySlotUI> ingredientSlots;
    private InventorySlotUI resultSlot;

    private void OnEnable()
    {
        ingredientSlots = new List<InventorySlotUI>(GetComponentsInChildren<InventorySlotUI>(true));
        resultSlot = ingredientSlots[ingredientSlots.Count - 1];
        ingredientSlots.RemoveAt(ingredientSlots.Count - 1);
    }

    public void Refresh(CraftingRecipe recipe, ResourceInventory playerInventory)
    {
        AssignedRecipe = recipe;
        List<(ItemType ingredient, int requiredCount, int availableCount)> availability = playerInventory.ComputeAvailability(recipe);

        resultSlot.UpdateSlot(recipe.result);

        for (int i = 0; i < ingredientSlots.Count; i++)
        {
            if (i < recipe.ingredients.Count)
            {
                var slotAvailability = availability[i];
                ItemType ingredient = slotAvailability.ingredient;
                int requiredCount = slotAvailability.requiredCount;
                int availableCount = slotAvailability.availableCount;
                ingredientSlots[i].UpdateSlotWithString(ingredient, $"{availableCount}/{requiredCount}");
            }
            else
            {
                ingredientSlots[i].ClearSlot();
            }
        }
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }


}