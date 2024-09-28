using HarmonyLib;
using ShipInventory.Helpers;

namespace ShipInventory.Patches;

[HarmonyPatch(typeof(RoundManager))]
public class RoundManager_Patches
{
    /// <summary>
    /// Clears the inventory and prevents the despawn
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPatch(nameof(RoundManager.DespawnPropsAtEndOfRound))]
    private static void ClearInventory(RoundManager __instance)
    {
        if (!__instance.IsServer)
            return;
        
        // Prevent if a player is alive
        if (!StartOfRound.Instance.allPlayersDead)
            return;
        
        // Prevent if safe
        if (ShipInventory.Config.ActAsSafe.Value)
            return;

        // Clear the inventory
        Logger.Debug("Clearing the ship...");
        ItemManager.SetItems([], true);
    }
}