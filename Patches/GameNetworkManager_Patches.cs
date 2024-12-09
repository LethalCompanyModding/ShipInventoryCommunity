using System.Linq;
using HarmonyLib;
using Newtonsoft.Json;
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
        string currentSaveFileName = GameNetworkManager.Instance.currentSaveFileName;

        // Delete items
        ES3.DeleteKey("shipGrabbableItemIDs", currentSaveFileName);
        ES3.DeleteKey("shipGrabbableItemPos", currentSaveFileName);
        ES3.DeleteKey("shipScrapValues", currentSaveFileName);
        ES3.DeleteKey("shipItemSaveData", currentSaveFileName);

        Logger.Debug("Saving chute items...");

        var items = ItemManager.GetItems();

        // Save items if necessary
        if (items.Any())
        {
            try
            {
                string json = JsonConvert.SerializeObject(items);
                ES3.Save(Constants.STORED_ITEMS, json, currentSaveFileName);
                Logger.Debug("Chute items saved!");
            }
            catch (System.Exception ex)
            {
                Logger.Error($"Failed to save chute items. {ex}");
            }
        }
        else
        {
            ES3.DeleteKey(Constants.STORED_ITEMS, currentSaveFileName);
            Logger.Debug("Chute items saved!");
        }
    }
}