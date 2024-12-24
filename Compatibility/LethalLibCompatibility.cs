using System;
using System.Collections.Generic;
using System.Linq;
using ShipInventory.Helpers;
using ShipInventory.Objects;

namespace ShipInventory.Compatibility;

public static class LethalLibCompatibility
{
    #region ID

    private const string ID_FORMAT = "{0}/{1}";

    private static string ID(this Item item, string mod = VANILLA_ITEM_MOD) => string.Format(ID_FORMAT, mod, item.itemName);

    private static (string mod, string name) Extract(string id)
    {
        var parts = id.Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (parts == null || parts.Length < 2)
            throw new ArgumentException();

        return (parts[0], parts[1]);
    }
    
    public static string GetID(Item item)
    {
        if (item == ItemData.FALLBACK_ITEM)
            return BAD_ITEM;

        return GetModdedID(item) ?? item.ID();
    }

    #endregion
    
    #region Item

    private const string BAD_ITEM = $"{nameof(ShipInventory)}/BadItem";
    private const string VANILLA_ITEM_MOD = "Vanilla";

    public static Item? GetItem(string ID)
    {
        // If bad item, return fallback item
        if (ID == BAD_ITEM)
            return ItemData.FALLBACK_ITEM;
        
        var (mod, name) = Extract(ID);
        return mod == VANILLA_ITEM_MOD ? GetVanillaItem(name) : GetModdedItem(ID);
    }
    
    #endregion

    #region Vanilla Items

    private static Item? GetVanillaItem(string name) => StartOfRound.Instance.allItemsList.itemsList.FirstOrDefault(item => item.itemName == name);

    #endregion
    
    #region Modded Items

    private static Dictionary<string, Item>? _cachedModdedItems;

    private static void LoadModdedItems()
    {
        // If already loaded, skip
        if (_cachedModdedItems != null)
            return;
        
        _cachedModdedItems = [];

        foreach (var item in LethalLib.Modules.Items.scrapItems)
            _cachedModdedItems.Add(item.item.ID(item.modName), item.item);

        foreach (var item in LethalLib.Modules.Items.shopItems)
            _cachedModdedItems.Add(item.item.ID(item.modName), item.item);

        foreach (var item in LethalLib.Modules.Items.plainItems)
            _cachedModdedItems.Add(item.item.ID(item.modName), item.item);
    }

    private static Item? GetModdedItem(string id)
    {
        LoadModdedItems();
        return _cachedModdedItems.GetValueOrDefault(id);
    }

    private static string? GetModdedID(Item item)
    {
        LoadModdedItems();

        if (_cachedModdedItems == null)
            return null;
        
        foreach (var (key, value) in _cachedModdedItems)
        {
            if (value != item)
                continue;

            return key;
        }

        return null;
    }

    #endregion
}