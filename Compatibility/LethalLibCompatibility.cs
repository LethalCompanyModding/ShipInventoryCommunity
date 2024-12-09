using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ShipInventory.Helpers;
using ShipInventory.Objects;

namespace ShipInventory.Compatibility;

public class LethalLibCompatibility
{
    private static List<ModdedItemData>? _cachedModdedItems;

    public static Item? GetItem(string ID)
    {
        var parts = ID.Split('/');
        string modName = parts[0];
        string itemName = parts[1];

        if (modName == nameof(ShipInventory) && itemName == "BadItem")
            return ItemData.FALLBACK_ITEM;
        
        if (modName == "Vanilla" || string.IsNullOrEmpty(modName))
            return GetVanillaItem(itemName);

        var moddedItems = GetModdedItems();

        foreach (var moddedItem in moddedItems)
        {
            if (moddedItem.ItemName == itemName && moddedItem.ModName == modName)
                return moddedItem.Item;
        }

        return null;
    }

    public static string GetID(Item item)
    {
        if (item == ItemData.FALLBACK_ITEM)
            return $"{nameof(ShipInventory)}/BadItem";
        
        var moddedItems = GetModdedItems();
        ModdedItemData? moddedItem = moddedItems.FirstOrDefault(i => i.Item == item);

        var modName = moddedItem?.ModName ?? "Vanilla";
        
        return $"{modName}/{item.itemName}";
    }

    private static Item? GetVanillaItem(string itemName)
    {
        foreach (var item in StartOfRound.Instance.allItemsList.itemsList)
        {
            if (item.itemName == itemName)
                return item;
        }

        return null;
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    private static List<ModdedItemData> GetModdedItems()
    {
        if (_cachedModdedItems != null)
            return _cachedModdedItems;

        _cachedModdedItems = [];

        foreach (var item in LethalLib.Modules.Items.scrapItems)
            _cachedModdedItems.Add(new ModdedItemData(item.item, item.modName));

        foreach (var item in LethalLib.Modules.Items.shopItems)
            _cachedModdedItems.Add(new ModdedItemData(item.item, item.modName));

        foreach (var item in LethalLib.Modules.Items.plainItems)
            _cachedModdedItems.Add(new ModdedItemData(item.item, item.modName));

        return _cachedModdedItems;
    }
}

public struct ModdedItemData
{
    public Item Item { get; }
    public string ModName { get; }
    public string? ItemName => Item?.itemName;

    public ModdedItemData(Item item, string modName)
    {
        Item = item;
        ModName = modName;
    }
}