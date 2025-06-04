using System;
using ShipInventoryUpdated.Items;

namespace ShipInventoryUpdated.Extensions;

/// <summary>
/// Extension methods for <see cref="ItemData"/>
/// </summary>
internal static class ItemDataExtensions
{
    /// <summary>
    /// Fetches the item from the given data
    /// </summary>
    public static Item GetItem(this ItemData data)
    {
        var item = Compatibility.LethalLib.GetItem(data.ID) ?? ItemManager.FALLBACK_ITEM;

        if (item != null)
            return item;

        throw new NullReferenceException();
    }

    /// <summary>
    /// Fetches the name of the item from the given data
    /// </summary>
    public static string GetItemName(this ItemData data) => data.GetItem().itemName;
}