using System;
using System.Collections.Generic;
using ShipInventoryUpdated.Items;

namespace ShipInventoryUpdated.Helpers;

/// <summary>
/// Converts the given <see cref="GrabbableObject"/> into a list of <see cref="ItemData"/>
/// </summary>
public static class ConvertItemHelper
{
    static ConvertItemHelper()
    {
        AddConversion(DefaultConversion);
        AddConversion(BeltBagConversion);
        //AddConversion(Compatibility.CustomItemBehaviourLibrary.ConvertContainer);
    }
    
    #region Conversions

    private static readonly List<Func<GrabbableObject, ItemData[]?>> conversions = [];
    
    internal static ItemData[] ConvertItem(GrabbableObject item)
    {
        ItemData[]? items = null;
        
        foreach (var conversion in conversions)
        {
            if (conversion == null)
                continue;

            items = conversion.Invoke(item);
            
            if (items != null)
                break;
        }

        return items ?? [];
    }
    
    /// <summary>
    /// Adds a conversion that defines how the given object is converted to its data form
    /// </summary>
    /// <param name="conversion">Returns the list of data for the given object or null if this conversion doesn't handle this object</param>
    public static void AddConversion(Func<GrabbableObject, ItemData[]?> conversion) => conversions.Add(conversion);
   
    #endregion

    #region Built in

    private static ItemData[]? BeltBagConversion(GrabbableObject item)
    {
        if (item is not BeltBagItem beltBagItem)
            return null;
        
        var items = new List<ItemData>();

        for (int i = beltBagItem.objectsInBag.Count - 1; i >= 0; i--)
        {
            var _item = beltBagItem.objectsInBag[i];

            if (_item == null)
                continue;
            
            items.AddRange(ConvertItem(_item));
        }

        items.Add(new ItemData(beltBagItem));
        
        return items.ToArray();
    }

    private static ItemData[] DefaultConversion(GrabbableObject item) => [new ItemData(item)];

    #endregion
}