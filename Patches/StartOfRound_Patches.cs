using System.Collections.Generic;
using HarmonyLib;
using Newtonsoft.Json;
using ShipInventory.Helpers;
using ShipInventory.Objects;
using Logger = ShipInventory.Helpers.Logger;

namespace ShipInventory.Patches;

[HarmonyPatch(typeof(StartOfRound))]
internal class StartOfRound_Patches
{
    /// <summary>
    /// Loads the saved items from the file into the inventory
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPatch(nameof(StartOfRound.LoadShipGrabbableItems))]
    private static void LoadStoredItems()
    {
        string currentSaveFileName = GameNetworkManager.Instance.currentSaveFileName;

        Logger.Debug("Loading stored items...");

        try
        {
            ItemManager.SetItems(LoadItems(currentSaveFileName));
        }
        catch (System.Exception ex)
        {
            Logger.Error($"Failed to load stored items. {ex}");
        }
        
        Logger.Debug("Loaded stored items!");
    }

    /// <summary>
    /// Resets the inventory when the ship gets reset
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPatch(nameof(StartOfRound.ResetShip))]
    private static void ResetInventory()
    {
        // Skip if persist
        if (ShipInventory.Config.PersistThroughFire.Value)
            return;
        
        ItemManager.SetItems([]);
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(StartOfRound.GetValueOfAllScrap))]
    private static void GetValueOfAllScrap(ref int __result, bool onlyScrapCollected, bool onlyNewScrap)
    {
        foreach (var data in ItemManager.GetItems())
        {
            // Don't count scrap from earlier rounds
            if (data.PERSISTED_THROUGH_ROUNDS)
                continue;
            
            var item = data.GetItem();
            
            if (item == null)
                continue;
            
            // Dont count non-scrap
            if (!item.isScrap)
                continue;

            __result += data.SCRAP_VALUE;
        }
    }

    private static IEnumerable<ItemData> LoadItems(string saveFileName)
    {
        if (!ES3.KeyExists(Constants.STORED_ITEMS, saveFileName))
            return [];
        
        string json = ES3.Load<string>(Constants.STORED_ITEMS, saveFileName);
        var items = JsonConvert.DeserializeObject<IEnumerable<ItemData>>(json);

        if (items == null)
        {
            items = [];
            Logger.Error("Could not load items from the save file.");
        }

        return items;
    }
}