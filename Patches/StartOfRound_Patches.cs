using HarmonyLib;
using ShipInventory.Helpers;
using ShipInventory.Objects;

namespace ShipInventory.Patches;

[HarmonyPatch(typeof(StartOfRound))]
internal class StartOfRound_Patches
{
    /// <summary>
    /// Loads the saved items from the file into the inventory
    /// </summary>
    [HarmonyPrefix]
    [HarmonyPatch(nameof(StartOfRound.LoadShipGrabbableItems))]
    private static void LoadStoredItems()
    {
        string currentSaveFileName = GameNetworkManager.Instance.currentSaveFileName;

        ItemData.LoadStoredItems(currentSaveFileName);
        BadItem.LoadKeys(currentSaveFileName);
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
}