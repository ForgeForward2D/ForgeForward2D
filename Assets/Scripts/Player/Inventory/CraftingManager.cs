using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CraftingManager
{
    public event Action<CraftingManager> OnCraftingManagerUpdate;

    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private WorkbenchType workbenchType;

    [Header("Debugging")]
    [SerializeField] private List<CraftingRecipe> allRecipes;
    [SerializeField] private List<CraftingRecipe> availableRecipes;
    [SerializeField] private int selectedRecipeIndex = 0;
    private bool active;

    public void Start()
    {
        allRecipes = Resources.LoadAll<CraftingRecipe>("CraftingRecipes")
            .Where(r => r.workbenchType == workbenchType)
            .ToList();
        UpdateAvailableRecipes();
        inventoryManager.OnInventoryUpdate += HandleInventoryUpdate;
        InputManager.OnMoveInput += HandleMovementInput;
        InputManager.OnAttackUpdate += HandleAttackUpdate;
    }

    public void SetActive(bool isActive)
    {
        active = isActive;
        if (!active) selectedRecipeIndex = 0;
    }

    private void HandleInventoryUpdate(InventoryManager inventoryManager)
    {
        this.inventoryManager = inventoryManager;
        UpdateAvailableRecipes();
    }

    public void RequestRefresh()
    {
        OnCraftingManagerUpdate?.Invoke(this);
    }

    private void HandleMovementInput((UIPage, bool, Vector2) data)
    {
        var (currentPage, performed, movementInput) = data;

        if (currentPage != UIPage.Crafting || !active)
            return;

        if (!performed)
            return;

        if (movementInput.y == 0)
            return;

        int delta = movementInput.y > 0 ? -1 : 1;

        selectedRecipeIndex += delta;
        selectedRecipeIndex = (selectedRecipeIndex % availableRecipes.Count + availableRecipes.Count) % availableRecipes.Count;

        OnCraftingManagerUpdate?.Invoke(this);
    }

    private void HandleAttackUpdate((UIPage, bool) data)
    {
        var (currentPage, click) = data;

        if (currentPage != UIPage.Crafting || !active || !click)
            return;

        CraftingRecipe recipe = availableRecipes[selectedRecipeIndex];
        if (TryCraft(recipe))
        {
            Debug.Log($"Successfully crafted {recipe.result.count} {recipe.result.itemType.displayName}");
        }
        else
        {
            Debug.Log($"Could not craft {recipe.result.count} {recipe.result.itemType.displayName}");
        }
    }

    private bool TryCraft(CraftingRecipe recipe)
    {
        CraftingRecipePreview availabilities = ComputePreview(recipe);
        bool anyNotAvailable = availabilities.ingredients.Any(ingredient => ingredient.available < ingredient.required.count);
        if (anyNotAvailable) return false;

        foreach (var ingredient in availabilities.ingredients)
        {
            inventoryManager.RemoveItemOfType(ingredient.required.itemType, ingredient.required.count);
        }
        inventoryManager.AddItemOfType(recipe.result.itemType, recipe.result.count);
        return  true;
    }

    private void UpdateAvailableRecipes()
    {
        availableRecipes = allRecipes
            .Where(recipe => inventoryManager.CountFreeSpace(recipe.result.itemType) >= recipe.result.count)
            .ToList();
        if (selectedRecipeIndex >= availableRecipes.Count)
        {
            if (availableRecipes.Count <= 1)
                selectedRecipeIndex = 0;
            else
                selectedRecipeIndex %= availableRecipes.Count;
        }
        OnCraftingManagerUpdate?.Invoke(this);
    }

    public int GetSelectedRecipeIndex()
    {
        return selectedRecipeIndex;
    }

    public List<CraftingRecipePreview> GetPreviews(int max)
    {
        List<CraftingRecipePreview> previews = new List<CraftingRecipePreview>();
        for (int i=0; i < max; i++)
        {
            if (i < availableRecipes.Count)
            {
                CraftingRecipe recipe = availableRecipes[(selectedRecipeIndex + i) % availableRecipes.Count];
                previews.Add(ComputePreview(recipe));
            }
            else
            {
                previews.Add(null);
            }
        }
        return previews;
    }

    private CraftingRecipePreview ComputePreview(CraftingRecipe recipe)
    {
        List<(Item, int)> availabilityList = recipe.ingredients.Select(ingredient =>
        {
            ItemType itemType = ingredient.itemType;
            int availableAmount = inventoryManager.CountElements(itemType);
            return (ingredient, availableAmount);
        }).ToList();

        return new CraftingRecipePreview(availabilityList, recipe.result);
    }

    public CraftingRecipe GetSelectedRecipe()
    {
        return availableRecipes != null && availableRecipes.Count > 0 ? availableRecipes[selectedRecipeIndex] : null;
    }

}

public record CraftingRecipePreview(List<(Item required, int available)> ingredients, Item result);
