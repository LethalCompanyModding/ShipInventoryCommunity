using HarmonyLib;
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
        // If key missing, skip
        if (!ES3.KeyExists(Constants.STORED_ITEMS, GameNetworkManager.Instance.currentSaveFileName))
        {
            ItemManager.SetItems([]);
            return;
        }
        
        Logger.Debug("Loading stored items...");
        ItemManager.SetItems(
            ES3.Load<ItemData[]>(Constants.STORED_ITEMS, GameNetworkManager.Instance.currentSaveFileName)
        );
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
}