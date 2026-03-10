using UnityEngine;
using System.Collections.Generic;

public static class ItemTypeRepository
{
    private static Dictionary<int, ItemType> idLookup;

    private static void Initialize()
    {
        var itemTypes = Resources.LoadAll<ItemType>("ItemData");
        var hotbarItemTypes = Resources.LoadAll<ItemType>("HotbarItemData");

        idLookup = new Dictionary<int, ItemType>();

        foreach (var item in itemTypes)
        {
            idLookup[item.Id] = item;
        }

        foreach (var item in hotbarItemTypes)
        {
            idLookup[item.Id] = item;
        }

        int totalItems = itemTypes.Length + hotbarItemTypes.Length;

        Debug.Log("Initialized ItemTypeRepository with " + totalItems + " items.");
    }

    public static ItemType GetItemById(int id)
    {
        if (idLookup == null)
            Initialize();
        // return idLookup[id];
        return idLookup.GetValueOrDefault(id);
    }
}