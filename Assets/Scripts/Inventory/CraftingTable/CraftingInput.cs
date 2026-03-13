
using System;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class CraftingInput : MonoBehaviour
{
    [SerializeField] private BlockType craftingTableBlockType;
    [SerializeField] private CraftingTableUI craftingTableUI;
    [SerializeField] private ResourceInventory playerInventory;

    // The move input is repurposed for recipe selection when the crafting table UI is open
    public void Move(InputAction.CallbackContext context)
    {
        if (!context.performed || craftingTableUI == null || !craftingTableUI.IsOpen)
            return;

        Vector2 input = context.ReadValue<Vector2>();

        if (input.y > 0f)
        {
            craftingTableUI.ScrollSelectedRecipe(-1);
        }
        else if (input.y < 0f)
        {
            craftingTableUI.ScrollSelectedRecipe(1);
        }

    }

    // The attack input is repurposed for crafting confirmation when the crafting table UI is open
    public void Attack(InputAction.CallbackContext context)
    {
        if (!context.performed || craftingTableUI == null || !craftingTableUI.IsOpen)
            return;
        CraftingRecipe selectedRecipe = craftingTableUI.GetSelectedRecipe();
        if (selectedRecipe == null) return;
        if(playerInventory.TryCraft(selectedRecipe))
        {
            craftingTableUI.RefreshUI();
            Debug.Log("Crafting succeeded: " + selectedRecipe.result.Item.DisplayName);
        }
    }

    private void OnEnable() {
        PlayerController.OnInteraction += Interact;
    }

    private void OnDisable() {
        PlayerController.OnInteraction -= Interact;
    }

    public void Interact((BlockType blockType, Vector2Int position) interactionInfo)
    {
        var (blockType, position) = interactionInfo;
        if (craftingTableUI == null) return;

        if (craftingTableUI.IsOpen) {
            Debug.Log("Closing crafting table UI");
            craftingTableUI.SetActive(false);
            return;
        }

        if (blockType != craftingTableBlockType) return;
        Debug.Log("Opening crafting table UI");
        craftingTableUI.SetActive(true);
    }
}