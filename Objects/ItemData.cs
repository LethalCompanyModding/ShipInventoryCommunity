using System;
using System.Collections.Generic;
using System.Linq;
using ShipInventory.Compatibility;
using ShipInventory.Helpers;
using Unity.Netcode;

namespace ShipInventory.Objects;

/// <summary>
/// Data transferred between the clients and the host
/// </summary>
[Serializable]
public struct ItemData : INetworkSerializable
{
    #nullable disable
    public static Item FALLBACK_ITEM; // Item to use when the original item is missing
    #nullable restore
    
    public string ID;
    
    public int SCRAP_VALUE;
    public int SAVE_DATA;
    public bool PERSISTED_THROUGH_ROUNDS;
    
    public ItemData(GrabbableObject item)
    {
        ID = LethalLibCompatibility.GetID(item.itemProperties);
        
        if (item.itemProperties.isScrap)
            SCRAP_VALUE = item.scrapValue;
    
        if (item.itemProperties.saveItemVariable)
            SAVE_DATA = item.GetItemDataToSave();

        PERSISTED_THROUGH_ROUNDS = item.scrapPersistedThroughRounds;
    }

    public Item GetItem() => LethalLibCompatibility.GetItem(ID) ?? FALLBACK_ITEM;
    public string GetItemName() => GetItem().itemName;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ID);
        serializer.SerializeValue(ref SCRAP_VALUE);
        serializer.SerializeValue(ref SAVE_DATA);
        serializer.SerializeValue(ref PERSISTED_THROUGH_ROUNDS);
    }

    #region IO

    /// <summary>
    /// Saves the stored items to the given save file
    /// </summary>
    public static void SaveStoredItems(string saveFileName)
    {
        if (!ItemManager.HasItems())
        {
            ES3.DeleteKey(Constants.STORED_ITEMS, saveFileName);
            Logger.Debug("Stored items cleared!");
            return;
        }
        
        var items = ItemManager.GetItems();
        
        Logger.Debug("Saving stored items...");

        ES3.Save(Constants.STORED_ITEMS, Newtonsoft.Json.JsonConvert.SerializeObject(items), saveFileName);

        Logger.Debug($"Successfully saved {ItemManager.GetCount()} items!");
    }

    /// <summary>
    /// Loads the stored items from the given save file
    /// </summary>
    public static void LoadStoredItems(string saveFileName)
    {
        ItemManager.ClearCache();
        
        if (!ES3.KeyExists(Constants.STORED_ITEMS, saveFileName))
        {
            Logger.Debug("No items found.");
            return;
        }
        
        Logger.Debug("Loading stored items...");
        
        string json = ES3.Load<string>(Constants.STORED_ITEMS, saveFileName);
        var items = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<ItemData>>(json);

        if (items == null)
        {
            Logger.Error("Could not load items from the save file.");
            return;
        }
        
        ItemManager.AddItems(items.ToArray());
        Logger.Debug("Loaded stored items!");
    }

    #endregion
}