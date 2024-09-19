using System.Linq;
using HarmonyLib;
using ShipInventory.Helpers;

namespace ShipInventory.Patches;

[HarmonyPatch(typeof(GameNetworkManager))]
public class GameNetworkManager_Patches
{
    /// <summary>
    /// Saves the inventory into the file
    /// </summary>
    [HarmonyPrefix]
    [HarmonyPatch(nameof(GameNetworkManager.SaveItemsInShip))]
    private static void SaveChuteItems(GameNetworkManager __instance)
    {
        // Delete items
        ES3.DeleteKey("shipGrabbableItemIDs", __instance.currentSaveFileName);
        ES3.DeleteKey("shipGrabbableItemPos", __instance.currentSaveFileName);
        ES3.DeleteKey("shipScrapValues", __instance.currentSaveFileName);
        ES3.DeleteKey("shipItemSaveData", __instance.currentSaveFileName);
        
        Logger.Debug("Saving chute items...");

        var items = ItemManager.GetItems();

        // Save items if necessary
        if (items.Any())
            ES3.Save(Constants.STORED_ITEMS, items.ToArray(), __instance.currentSaveFileName);
        else
            ES3.DeleteKey(Constants.STORED_ITEMS, __instance.currentSaveFileName);
        
        Logger.Debug("Chute items saved!");
    }
}