using HarmonyLib;
using ShipInventory.Helpers;
using ShipInventory.Items;
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

        ItemsIO.LoadStoredItems(currentSaveFileName);
        ItemsIO.LoadKeys(currentSaveFileName);
    }

    /// <summary>
    /// Resets the inventory when the ship gets reset
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPatch(nameof(StartOfRound.ResetShip))]
    private static void ResetInventory()
    {
        // Skip if persist
        if (ShipInventory.Configuration.PersistThroughFire.Value)
            return;

        ItemManager.ClearCache();
    }

    /// <summary>
    /// Adds the value of all the scraps collected this round and stored
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPatch(nameof(StartOfRound.GetValueOfAllScrap))]
    private static void GetValueOfAllScrap(ref int __result, bool onlyScrapCollected, bool onlyNewScrap)
    {
        __result += ItemManager.GetTotalValue(true, true);
    }

    /// <summary>
    /// Unlocks the chute when loading all the unlockables
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPatch(nameof(StartOfRound.LoadUnlockables))]
    private static void UnlockChute(StartOfRound __instance)
    {
        if (ShipInventory.Configuration.ChuteIsUnlock)
            return;
        
        int index = -1;

        for (int i = 0; i < __instance.unlockablesList.unlockables.Count; i++)
        {
            if (__instance.unlockablesList.unlockables[i] == ChuteInteract.UnlockableItem)
            {
                index = i;
                break;
            }
        }

        if (index == -1)
        {
            Logger.Error("Could not find the chute as an unlockable.");
            return;
        }

        __instance.UnlockShipObject(index);
    }

    /// <summary>
    /// Unlocks the chute back when the ship resets
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPatch(nameof(StartOfRound.ResetShip))]
    private static void UnlockChuteBack(StartOfRound __instance)
    {
        if (!__instance.IsServer)
            return;
        
        UnlockChute(__instance);
    }
}