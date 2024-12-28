using System;
using System.Collections.Generic;
using System.Linq;
using ShipInventory.Objects;

namespace ShipInventory.Helpers;

public static class ItemManager
{
    #region Getters

    public static IEnumerable<ItemData> GetInstances(ItemData data, int count)
    {
        // Take only one
        if (count == 1)
            return cachedItems.Where(d => d.Equals(data)).Take(1);
        
        // Take all with id
        return cachedItems.Where(d => d.ID == data.ID).Take(count);
    }

    #endregion

    #region Cache
    
    /// <summary>
    /// Host: List of the items stored
    /// Client: Cached list of the items stored
    /// </summary>
    private static List<ItemData> cachedItems = [];

    /// <returns>Copy of the items</returns>
    public static ItemData[] GetItems() => [..cachedItems];

    /// <summary>
    /// Updates the cache with the given values
    /// </summary>
    internal static void UpdateCache(string? key, List<ItemData> items)
    {
        cacheKey = key;
        cachedItems = items;
    }

    /// <summary>
    /// Clears the cache
    /// </summary>
    internal static void ClearCache() => UpdateCache(null, []);

    /// <summary>
    /// Removes all the items that are not present in the cache
    /// </summary>
    /// <returns>Filtered items</returns>
    internal static ItemData[] FilterExtras(params ItemData[] items) => items.Where(item => cachedItems.Contains(item)).ToArray();

    /// <summary>
    /// Adds the given item to the cached inventory
    /// </summary>
    internal static void AddItems(params ItemData[] items)
    {
        cachedItems.AddRange(items);
        NewKey();
    }
    
    /// <summary>
    /// Removes the given item from the cached inventory
    /// </summary>
    internal static void RemoveItems(params ItemData[] items)
    {
        foreach (var i in items)
            cachedItems.Remove(i);
        NewKey();
    }

    /// <summary>
    /// Sets all the items as persisted through rounds
    /// </summary>
    internal static void SetAllPersisted()
    {
        for (int i = 0; i < cachedItems.Count; i++)
        {
            var data = cachedItems[i];
            data.PERSISTED_THROUGH_ROUNDS = true;
            cachedItems[i] = data;
        }
        
        NewKey();
    }

    /// <returns>Number of items</returns>
    public static int GetCount() => cachedItems.Count;

    /// <returns>If there is at least one item</returns>
    public static bool HasItems() => cachedItems.Count > 0;

    /// <returns>Total value of all the items</returns>
    public static int GetTotalValue(bool onlyScraps = false)
    {
        var total = 0;
        
        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        foreach (var data in cachedItems)
        {
            // Don't count scrap from earlier rounds
            if (data.PERSISTED_THROUGH_ROUNDS)
                continue;
            
            var item = data.GetItem();
            
            if (item == null)
                continue;
            
            // Dont count non-scrap
            if (onlyScraps && !item.isScrap)
                continue;

            total += data.SCRAP_VALUE;
        }

        return total;
    }

    #endregion

    #region Cache Key

    /// <summary>
    /// Key representing the current inventory
    /// </summary>
    private static string? cacheKey;
    
    /// <summary>
    /// Obtains the current key of the cache
    /// </summary>
    public static string? GetKey() => cacheKey;
    
    /// <summary>
    /// Assigns a new key to the cache
    /// </summary>
    private static void NewKey() => cacheKey = cacheKey = Guid.NewGuid().ToString();

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