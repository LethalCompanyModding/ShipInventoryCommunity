using System.Collections.Generic;
using ShipInventory.Compatibility;
using UnityEngine;
using Logger = ShipInventory.Helpers.Logger;

namespace ShipInventory.Objects;

public class BadItem : PhysicsProp
{
    public string? ID;
    
    public override void Start()
    {
        grabbable = true;
        grabbableToEnemies = true;
        isInFactory = true;
        
        base.Start();

        CheckForItem();
    }

    public static ItemData[]? PrepareItems()
    {
        // Find all physical bad items
        var badItems = FindObjectsByType<BadItem>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);
        
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
        
        // Return list
        return keys.Count == 0 ? null : keys.ToArray();
    }

    #region Data

    private ItemData? _data; // Data loaded from the save file
    private int _index; // Index to the data of this item

    public override int GetItemDataToSave() => _index;

    public override void LoadItemSaveData(int saveData)
    {
        _index = saveData;
        
        if (_loadedData == null)
            return;
        
        _data = _loadedData[saveData];
        ID = _data.Value.ID;
    }

    private void CheckForItem()
    {
        // If no data loaded, skip
        if (_data == null)
            return;
        
        var item = LethalLibCompatibility.GetItem(_data.Value.ID);
        
        // If item not found, skip
        if (item == null)
            return;
        
        Logger.Debug($"The item '{ID}' was found! Replacing the bad item with it...");
        
        GrabbableObject component = Instantiate(item.spawnPrefab, transform.position, Quaternion.identity, transform.parent).GetComponent<GrabbableObject>();
        component.fallTime = 1f;
        component.hasHitGround = true;
        component.scrapPersistedThroughRounds = true;
        component.isInElevator = true;
        component.isInShipRoom = true;
        
        if (item.isScrap)
            component.SetScrapValue(_data.Value.SCRAP_VALUE);
        
        if (item.saveItemVariable)
            component.LoadItemSaveData(_data.Value.SAVE_DATA);
        
        component.NetworkObject.Spawn();
        
        NetworkObject.Despawn();
        
        Logger.Debug("The replacement was spawned and the bad item was despawned!");
    }
    
    #endregion

    #region IO

    private static ItemData[]? _loadedData;

    /// <summary>
    /// Saves the keys to the given save file
    /// </summary>
    public static void SaveKeys(string saveFileName)
    {
        var keys = PrepareItems();

        if (keys == null || keys.Length == 0)
        {
            ES3.DeleteKey(Constants.BAD_ITEM_KEYS, saveFileName);
            Logger.Debug("Bad item keys cleared!");
            return;
        }
        
        Logger.Debug("Saving bad item keys...");

        ES3.Save(Constants.BAD_ITEM_KEYS, Newtonsoft.Json.JsonConvert.SerializeObject(keys), saveFileName);
        
        Logger.Debug($"Successfully saved {keys.Length} keys!");
    }

    /// <summary>
    /// Loads the keys from the given save file
    /// </summary>
    public static void LoadKeys(string saveFileName)
    {
        _loadedData = null;
        
        if (!ES3.KeyExists(Constants.BAD_ITEM_KEYS, saveFileName))
        {
            Logger.Debug("No bad item keys found.");
            return;
        }
        
        Logger.Debug("Loading bad item keys...");
        
        string json = ES3.Load<string>(Constants.BAD_ITEM_KEYS, saveFileName);
        var keys = Newtonsoft.Json.JsonConvert.DeserializeObject<ItemData[]>(json);
        
        if (keys == null)
        {
            Logger.Error("Could not load the bad item keys from the save file.");
            return;
        }

        _loadedData = keys;
        
        Logger.Debug($"Successfully loaded {keys.Length} keys!");
    }

    #endregion
}