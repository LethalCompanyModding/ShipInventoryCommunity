using System;
using System.Collections.Generic;
using System.Linq;
using ShipInventory.Objects;

namespace ShipInventory.Helpers;

public static class ItemManager
{
    /// <summary>
    /// List of the items stored in the ship's inventory
    /// </summary>
    private static IEnumerable<ItemData> storedItems = [];

    #region Getter/Setter

    /// <summary>
    /// Copies the items stored in the ship's inventory
    /// </summary>
    public static IEnumerable<ItemData> GetItems() => new List<ItemData>(
        storedItems.OrderBy(i => i.GetItem()?.itemName)
    );

    public static IEnumerable<ItemData> GetInstances(ItemData data, int count)
    {
        var item = data.GetItem();
        
        // If item invalid
        if (item is null)
            return [];
        
        return storedItems.Where(d => d.ID == data.ID).Take(count);
    }
    
    public static void SetItems(IEnumerable<ItemData> newItems, bool updateAll = false)
    {
        Logger.Debug($"Setting items from {storedItems.Count()} to {newItems.Count()}...");
        storedItems = newItems;
        
        UpdateValue();
        
        if (updateAll)
            ChuteInteract.Instance?.RequestAll();
    }

    #endregion
    #region Single alternation

    /// <summary>
    /// Adds the given item to the cached inventory
    /// </summary>
    internal static void Add(ItemData data) => SetItems(storedItems.Append(data));

    /// <summary>
    /// Removes the given item from the cached inventory
    /// </summary>
    internal static void Remove(ItemData data)
    {
        var copy = storedItems.ToList();
        copy.Remove(data);
        SetItems(copy);
    }

    #endregion
    #region Grabbable Object

    /// <summary>
    /// Updates the value of the chute
    /// </summary>
    public static void UpdateValue()
    {
        if (ChuteInteract.Instance == null)
            return;

        var grabbable = ChuteInteract.Instance.GetComponent<GrabbableObject>();
        
        // Skip if item invalid
        if (grabbable == null)
            return;

        grabbable.scrapValue = storedItems.Sum(i => i.SCRAP_VALUE);
        grabbable.OnHitGround(); // Update 
    }

    #endregion
    #region Item Data

    private static List<Item> itemsAllowed => StartOfRound.Instance.allItemsList.itemsList;
    
    public static ItemData Save(GrabbableObject item)
    {
        ItemData data = default;

        data.ID = itemsAllowed.FindIndex(i => i == item.itemProperties);
        
        if (item.itemProperties.isScrap)
            data.SCRAP_VALUE = item.scrapValue;
        
        if (item.itemProperties.saveItemVariable)
            data.SAVE_DATA = item.GetItemDataToSave();
        
        return data;
    }

    public static bool IsItemAllowed(Item? item)
    {
        // If item invalid
        if (item is null)
            return true;
        
        // If item in list
        return itemsAllowed.Contains(item);
    }

    public static Item? GetItem(ItemData data) => itemsAllowed.Count > data.ID && data.ID >= 0 
        ? itemsAllowed[data.ID] 
        : null;

    #endregion

    #region Print

    public enum PrintExtra
    {
        NONE,
        AVG,
        SUM
    }
    
    public static string DisplayItems(
        string original,
        IEnumerable<IGrouping<int, ItemData>> items, 
        PrintExtra extra = PrintExtra.NONE)
    {
        // Set text
        string content = !items.Any() ? "No item  stored!" : "";

        // - NAME xQT ($CRD)
        foreach (var data in items)
        {
            var item = data.First().GetItem();

            // Skip if invalid
            if (item == null)
                continue;

            string line = $"\u2219 {item.itemName} x{data.Count()}";

            // Add extra value
            if (item.isScrap && extra != PrintExtra.NONE)
            {
                switch (extra)
                {
                    case PrintExtra.AVG:
                        line += $" (AVG: ${data.Average(d => d.SCRAP_VALUE):N2})";
                        break;
                    case PrintExtra.SUM:
                        line += $" (SUM: ${data.Sum(d => d.SCRAP_VALUE):N2})";
                        break;
                }
            }

            content += line + "\n";
        }

        original = original.Replace(Constants.ITEMS, content);
        original = original.Replace(
            Constants.TOTAL, 
            items.Sum(g => g.Sum(i => i.SCRAP_VALUE)).ToString()
        );
        
        return original;
    }

    #endregion
}