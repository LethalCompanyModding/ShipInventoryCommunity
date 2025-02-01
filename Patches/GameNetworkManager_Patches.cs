using HarmonyLib;
using ShipInventory.Items;
using ShipInventory.Objects;

namespace ShipInventory.Patches;

[HarmonyPatch(typeof(GameNetworkManager))]
public class GameNetworkManager_Patches
{
    /// <summary>
    /// Loads the required assets when the game starts
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPatch(nameof(GameNetworkManager.Start))]
    private static void LoadRequiredAssets(GameNetworkManager __instance)
    {
        if (!ShipInventory.LoadChute(out var chutePrefab) || chutePrefab == null)
            return;

        if (!ShipInventory.LoadTerminalNode(chutePrefab))
            return;
    }
    
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

        ItemsIO.SaveStoredItems(currentSaveFileName);
        ItemsIO.SaveKeys(currentSaveFileName);
    }
}