using HarmonyLib;
using ShipInventory.Helpers;
using ShipInventory.Objects;
using Logger = ShipInventory.Helpers.Logger;

namespace ShipInventory.Patches;

[HarmonyPatch(typeof(StartOfRound))]
internal class StartOfRound_Patches
{
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
}