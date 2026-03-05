using UnityEngine;
using System.Collections.Generic;

public static class ItemTypeRepository
{
    private static Dictionary<int, ItemType> idLookup;

    private static void Initialize()
    {
        var itemTypes = Resources.LoadAll<ItemType>("ItemData");

        idLookup = new Dictionary<int, ItemType>();

        foreach (var item in itemTypes)
        {
            idLookup[item.Id] = item;
        }


        Debug.Log("Initialized ItemTypeRepository with " + itemTypes.Length + " items.");
    }

    public static ItemType GetItemById(int id)
    {
        if (idLookup == null)
            Initialize();
        // return idLookup[id];
        return idLookup.GetValueOrDefault(id);
    }
}