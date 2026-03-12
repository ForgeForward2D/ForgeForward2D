
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
        //TODO: cap scroll speed
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
        if (!context.performed) return;
        CraftingRecipe selectedRecipe = craftingTableUI.GetSelectedRecipe();
        if (selectedRecipe == null) return;
        bool result = playerInventory.TryCraft(selectedRecipe);
        if (result)
        {
            Debug.Log("Crafting succeeded: " + selectedRecipe.result.Item.DisplayName);
        }
        else
        {
            Debug.Log("Crafting failed: " + selectedRecipe.result.Item.DisplayName);
        }


        craftingTableUI.RefreshUI();
    }

    void Awake() {
        Debug.Log("Registering CraftingInput to PlayerController.OnInteraction");
        PlayerController.OnInteraction += Interact;
    }

    public void Interact((BlockType, Vector2Int) interactionInfo)
    {
        Debug.Log("CraftingInput received interaction event with block type: " + interactionInfo.Item1 + " at position: " + interactionInfo.Item2);
        BlockType blockType = interactionInfo.Item1;
        Vector2Int position = interactionInfo.Item2;
    
        if (craftingTableUI == null) return;

        if (craftingTableUI.IsOpen) {
            craftingTableUI.SetInactive();
            return;
        }

        if (blockType != craftingTableBlockType) return;

        Debug.Log("Interacted with crafting table at position: " + position);
        craftingTableUI.SetActive();
    }
}