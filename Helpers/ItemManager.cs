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

    #region Getters

    /// <summary>
    /// Copies the items stored in the ship's inventory
    /// </summary>
    public static IEnumerable<ItemData> GetItems() => new List<ItemData>(
        storedItems.OrderBy(i => i.GetItemName()).ThenBy(i => i.SCRAP_VALUE)
    );

    public static IEnumerable<ItemData> GetInstances(ItemData data, int count)
    {
        // Take only one
        if (count == 1)
            return storedItems.Where(d => d.Equals(data)).Take(1);
        
        // Take all with id
        return storedItems.Where(d => d.ID == data.ID).Take(count);
    }

    public static int GetTotalValue() => storedItems.Sum(i => i.SCRAP_VALUE);

    public static int GetCount() => storedItems.Count();

    #endregion
    #region Setters

    public static void SetItems(IEnumerable<ItemData> newItems, bool updateAll = false)
    {
        Logger.Debug($"Setting items from {storedItems.Count()} to {newItems.Count()}...");
        storedItems = newItems;
        
        ChuteInteract.Instance?.UpdateValue();
        
        if (updateAll)
            ChuteInteract.Instance?.RequestItemsAll();
    }

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
    #region Store Item

    public static void StoreItem(GrabbableObject item)
    {
        if (ChuteInteract.Instance == null)
            return;

        ItemData data = new ItemData(item);
        
        item.OnBroughtToShip();
        
        // Send store request to server
        Logger.Debug("Sending new item to server...");
        ChuteInteract.Instance.StoreItemServerRpc(data);
    }

    #endregion
    #region Blacklist

    private static readonly HashSet<string> BLACKLIST = [];
    internal static void UpdateBlacklist(string blacklistString)
    {
        BLACKLIST.Clear();
        foreach (var s in blacklistString.Split(',', StringSplitOptions.RemoveEmptyEntries))
            BLACKLIST.Add(s);
    }

    public static bool IsBlacklisted(Item item) => BLACKLIST.Contains(item.itemName.ToLower());

    #endregion
}