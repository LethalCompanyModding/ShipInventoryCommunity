using System.Collections.Generic;
using System.Linq;
using ShipInventoryUpdated.Objects;
using UnityEngine;

namespace ShipInventoryUpdated.Items;

/// <summary>
/// Class that handles the I/O for items
/// </summary>
internal static class ItemsIO
{
    private static ItemData[]? loadedKeys;

    /// <summary>
    /// Saves the stored items to the given save file
    /// </summary>
    public static void SaveStoredItems(string saveFileName)
    {
        if (!ItemManager.HasItems())
        {
            ES3.DeleteKey(Constants.STORED_ITEMS, saveFileName);
            Helpers.Logger.Debug("Stored items cleared!");
            return;
        }
        
        var items = ItemManager.GetItems();
        
        Helpers.Logger.Debug("Saving stored items...");

        ES3.Save(Constants.STORED_ITEMS, Newtonsoft.Json.JsonConvert.SerializeObject(items), saveFileName);

        Helpers.Logger.Debug($"Successfully saved {ItemManager.GetCount()} items!");
    }

    /// <summary>
    /// Loads the stored items from the given save file
    /// </summary>
    public static void LoadStoredItems(string saveFileName)
    {
        ItemManager.ClearCache();
        
        if (!ES3.KeyExists(Constants.STORED_ITEMS, saveFileName))
        {
            Helpers.Logger.Debug("No items found.");
            return;
        }
        
        Helpers.Logger.Debug("Loading stored items...");
        
        string json = ES3.Load<string>(Constants.STORED_ITEMS, saveFileName);
        var items = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<ItemData>>(json);

        if (items == null)
        {
            Helpers.Logger.Error("Could not load items from the save file.");
            return;
        }
        
        ItemManager.AddItems(items.ToArray());
        Helpers.Logger.Debug("Loaded stored items!");
    }
    
    /// <summary>
    /// Saves the keys to the given save file
    /// </summary>
    public static void SaveKeys(string saveFileName)
    {
        // Find all physical bad items
        var badItems = Object.FindObjectsByType<BadItem>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);
        
        // Compile their items
        var keys = new List<ItemData>();

        foreach (var item in badItems)
        {
            if (item.ID == null)
                continue;

            item._index = keys.Count;

            keys.Add(new ItemData(item)
            {
                ID = item.ID
            });
        }

        if (keys.Count == 0)
        {
            ES3.DeleteKey(Constants.BAD_ITEM_KEYS, saveFileName);
            Helpers.Logger.Debug("Bad item keys cleared!");
            return;
        }
        
        Helpers.Logger.Debug("Saving bad item keys...");

        ES3.Save(Constants.BAD_ITEM_KEYS, Newtonsoft.Json.JsonConvert.SerializeObject(keys), saveFileName);
        
        Helpers.Logger.Debug($"Successfully saved {keys.Count} keys!");
    }

    /// <summary>
    /// Loads the keys from the given save file
    /// </summary>
    public static void LoadKeys(string saveFileName)
    {
        loadedKeys = null;
        
        if (!ES3.KeyExists(Constants.BAD_ITEM_KEYS, saveFileName))
        {
            Helpers.Logger.Debug("No bad item keys found.");
            return;
        }
        
        Helpers.Logger.Debug("Loading bad item keys...");
        
        string json = ES3.Load<string>(Constants.BAD_ITEM_KEYS, saveFileName);
        var keys = Newtonsoft.Json.JsonConvert.DeserializeObject<ItemData[]>(json);
        
        if (keys == null)
        {
            Helpers.Logger.Error("Could not load the bad item keys from the save file.");
            return;
        }

        loadedKeys = keys;
        
        Helpers.Logger.Debug($"Successfully loaded {keys.Length} keys!");
    }

    public static bool GetItemKey(int index, out ItemData? data)
    {
        data = null;
        if (loadedKeys == null)
            return false;

        if (index < 0 || loadedKeys.Length >= index)
            return false;

        data = loadedKeys[index];
        return true;
    }
}