using System.Collections.Generic;
using System.Linq;
using GameNetcodeStuff;
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
        storedItems.OrderBy(i => i.GetItem()?.itemName).ThenBy(i => i.SCRAP_VALUE)
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
            ChuteInteract.Instance?.RequestItemsAll();
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
        if (grabbable is null)
            return;
        
        grabbable.scrapValue = storedItems.Sum(i => i.SCRAP_VALUE);
        grabbable.OnHitGround(); // Update 
    }

    #endregion
    #region Item Data
    
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

    public static Item? GetItem(ItemData data) => itemsAllowed.Count > data.ID && data.ID >= 0 
        ? itemsAllowed[data.ID] 
        : null;
    
    #endregion
    #region Blacklist
    
    private static List<Item> itemsAllowed => StartOfRound.Instance.allItemsList.itemsList;
    private static string[] BLACKLIST = [];
    public static void UpdateBlacklist(string blacklistString)
    {
        BLACKLIST = blacklistString
            .Split(',', System.StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim().ToLower())
            .ToArray();
    }

    public static bool IsItemAllowed(Item? item)
    {
        // If no item, valid
        if (item is null)
            return true;
        
        // If item in list and not in blacklist
        return itemsAllowed.Contains(item) && !BLACKLIST.Contains(item.itemName.ToLower());
    }

    public static void UpdateTrigger(InteractTrigger trigger, PlayerControllerB local)
    {
        // Holding nothing
        if (!local.isHoldingObject)
        {
            trigger.interactable = false;
            trigger.disabledHoverTip = Constants.NOT_HOLDING_ITEM;
            return;
        }

        // Not in orbit
        if (!StartOfRound.Instance.inShipPhase && ShipInventory.Config.RequireInOrbit.Value)
        {
            trigger.interactable = false;
            trigger.disabledHoverTip = Constants.NOT_IN_ORBIT;
            return;
        }
        
        // Item not allowed
        if (!IsItemAllowed(local.currentlyHeldObjectServer?.itemProperties))
        {
            trigger.interactable = false;
            trigger.disabledHoverTip = Constants.ITEM_NOT_ALLOWED;
            return;
        }
        
        trigger.interactable = local.isHoldingObject;
    }

    #endregion
}