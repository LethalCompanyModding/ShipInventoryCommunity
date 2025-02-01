using System;
using ShipInventory.Items;

namespace ShipInventory.Extensions;

internal static class ItemDataExtensions
{
    public static Item GetItem(this ItemData data)
    {
        var item = Compatibility.LethalLib.GetItem(data.ID) ?? ItemManager.FALLBACK_ITEM;

        if (item != null)
            return item;

        throw new NullReferenceException();
    }

    public static string GetItemName(this ItemData data) => data.GetItem().itemName;
}