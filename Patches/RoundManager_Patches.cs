using HarmonyLib;
using ShipInventory.Helpers;

namespace ShipInventory.Patches;

[HarmonyPatch(typeof(RoundManager))]
public class RoundManager_Patches
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(RoundManager.DespawnPropsAtEndOfRound))]
    private static void ClearInventory(RoundManager __instance)
    {
        if (!__instance.IsServer)
            return;
        
        // Clear the inventory
        if (StartOfRound.Instance.allPlayersDead && !ShipInventory.Config.ActAsSafe.Value)
        {
            ItemManager.ClearCache();
            return;
        }

        // Set all items to persist
        ItemManager.SetAllPersisted();
    }
}