using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CraftingManager
{
    [SerializeField] private int selectedRecipeIndex = 0;
    [SerializeField] private InventoryManager inventoryManager;

    [Header("Debugging")]
    [SerializeField] private List<CraftingRecipe> allRecipes = new List<CraftingRecipe>();
    [SerializeField] private List<CraftingRecipe> availableRecipes = new List<CraftingRecipe>();
    
    public static event Action<CraftingManager> OnCraftingManagerUpdate;

    public void Start() 
    {
        allRecipes = CraftingRecipeRepository.GetAllRecipes();
        UpdateAvailableRecipes();
        inventoryManager.OnInventoryUpdate += HandleInventoryUpdate;
        CraftingTableUI.RequestRefresh += HandleRequestRefresh;
        InputManager.OnMoveInput += HandleMovementInput;
        InputManager.OnAttackUpdate += HandleAttackUpdate;
    }

    private void HandleInventoryUpdate(InventoryManager inventoryManager)
    {
        this.inventoryManager = inventoryManager;
        UpdateAvailableRecipes();      
    }

    private void HandleRequestRefresh()
    {
        OnCraftingManagerUpdate?.Invoke(this);
    }

    private void HandleMovementInput((UIPage, Vector2) data)
    {
        var (uiPage, movementInput) = data;

        if (uiPage != UIPage.Crafting)
            return;

        if (movementInput.y == 0)
            return;
        
        int delta = movementInput.y > 0 ? 1 : -1;

        selectedRecipeIndex += delta;
        selectedRecipeIndex %= availableRecipes.Count;
        selectedRecipeIndex += availableRecipes.Count;
        selectedRecipeIndex %= availableRecipes.Count;

        OnCraftingManagerUpdate?.Invoke(this);
    }

    private void HandleAttackUpdate((UIPage, bool) data)
    {
        var (uiPage, click) = data;

        if (uiPage != UIPage.Crafting || !click)
            return;
        
        Debug.Log("TODO: Crafting");
    }

    private void UpdateAvailableRecipes()
    {
        availableRecipes = CraftingRecipeRepository.GetAllRecipes()
            .Where(recipe => inventoryManager.CountFreeSpace(recipe.result.itemType) >= recipe.result.count)
            .ToList();
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

public class CraftingRecipePreview
{
    public List<(Item required, int available)> ingredients;
    public Item result;

    public CraftingRecipePreview(List<(Item,int)> ingredients, Item result)
    {
        this.ingredients = ingredients;
        this.result = result;
    }




}