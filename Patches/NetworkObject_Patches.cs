using HarmonyLib;
using ShipInventory.Helpers;
using ShipInventory.Objects;
using Unity.Netcode;

namespace ShipInventory.Patches;

[HarmonyPatch(typeof(NetworkObject))]
public class NetworkObject_Patches
{
    /// <summary>
    /// Clears the inventory and prevents the despawn
    /// </summary>
    [HarmonyPrefix]
    [HarmonyPatch(nameof(NetworkObject.Despawn))]
    private static bool PreventChuteDespawn(NetworkObject __instance)
    {
        // If not vent, skip
        if (!__instance.TryGetComponent(out VentProp _))
            return true;

        // Prevent if safe
        if (ShipInventory.Config.ActAsSafe.Value)
            return false;

        // Clear the inventory
        Logger.Debug("Clearing the ship...");
        ItemManager.SetItems([], true);
        
        return false;
    }
}