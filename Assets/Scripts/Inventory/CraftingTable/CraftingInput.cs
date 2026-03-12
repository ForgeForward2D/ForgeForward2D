
using System;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class CraftingInput : MonoBehaviour
{
    [SerializeField] private BlockType craftingTableBlockType;
    [SerializeField] private CraftingTableUI craftingTableUI;
    [SerializeField] private ResourceInventory playerInventory;

    public void Move(InputAction.CallbackContext context)
    {   
        if (!context.performed || craftingTableUI == null || !craftingTableUI.IsOpen)
            return;
        
        Vector2 input = context.ReadValue<Vector2>();

        if (input.y > 0.5f)
        {
            craftingTableUI.ScrollSelectedRecipe(-1);
        }
        else if (input.y < -0.5f)
        {
            craftingTableUI.ScrollSelectedRecipe(1);
        }

    }

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

    private void Awake() {
        PlayerController.OnInteraction += Interact;
    }

    public void Interact((BlockType, Vector2Int) interactionInfo)
    {
        BlockType blockType = interactionInfo.Item1;
        Vector2Int position = interactionInfo.Item2;
    
        if (craftingTableUI == null) return;

        if (craftingTableUI.IsOpen) {
            craftingTableUI.SetActive(false);
            return;
        }

        if (blockType != craftingTableBlockType) return;

        craftingTableUI.SetActive(true);
    }
}