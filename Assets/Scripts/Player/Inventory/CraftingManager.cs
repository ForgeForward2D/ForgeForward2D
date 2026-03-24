using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class CraftingManager
{
    public static event Action<CraftingManager> OnCraftingManagerUpdate;

    [SerializeField] private InventoryManager inventoryManager;

    [Header("Debugging")]
    [SerializeField] private List<CraftingRecipe> allRecipes;
    [SerializeField] private List<CraftingRecipe> availableRecipes;
    [SerializeField] private int selectedRecipeIndex = 0;

    public void Start()
    {
        allRecipes = new List<CraftingRecipe>(Resources.LoadAll<CraftingRecipe>("CraftingRecipes"));
        SortRecipesByComplexity();
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

    private void HandleMovementInput((UIPage, bool, Vector2) data)
    {
        var (uiPage, performed, movementInput) = data;

        if (uiPage != UIPage.Crafting)
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
        var (uiPage, click) = data;

        if (uiPage != UIPage.Crafting || !click)
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
        return true;
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
        for (int i = 0; i < max; i++)
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

    private void SortRecipesByComplexity()
    {
        // Step 1: Find all ItemTypes that are a result of some recipe
        HashSet<ItemType> craftedItems = new();
        foreach (var recipe in allRecipes)
            craftedItems.Add(recipe.result.itemType);

        // Collect all ItemTypes referenced as ingredients
        Dictionary<ItemType, float> itemValue = new();
        foreach (var recipe in allRecipes)
            foreach (var ingredient in recipe.ingredients)
                if (!craftedItems.Contains(ingredient.itemType))
                    itemValue[ingredient.itemType] = 1f;

        // Step 2: Fixed-point iteration over recipes
        bool changed = true;
        while (changed)
        {
            changed = false;
            foreach (var recipe in allRecipes)
            {
                bool allValued = true;
                float cost = 0f;
                foreach (var ingredient in recipe.ingredients)
                {
                    if (itemValue.TryGetValue(ingredient.itemType, out float v))
                        cost += v * ingredient.count;
                    else
                    {
                        allValued = false;
                        break;
                    }
                }

                if (!allValued) continue;

                float newValue = cost / recipe.result.count;
                ItemType resultType = recipe.result.itemType;

                if (!itemValue.TryGetValue(resultType, out float current) || newValue < current)
                {
                    itemValue[resultType] = newValue;
                    changed = true;
                }
            }
        }

        allRecipes.Sort((a, b) =>
        {
            float va = itemValue.GetValueOrDefault(a.result.itemType, float.MaxValue);
            float vb = itemValue.GetValueOrDefault(b.result.itemType, float.MaxValue);
            int cmp = va.CompareTo(vb);
            if (cmp != 0) return cmp;
            return string.Compare(a.result.itemType.displayName, b.result.itemType.displayName, StringComparison.Ordinal);
        });
    }

}

public record CraftingRecipePreview(List<(Item required, int available)> ingredients, Item result);
