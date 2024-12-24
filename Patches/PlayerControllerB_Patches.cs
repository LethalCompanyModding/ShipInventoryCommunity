using GameNetcodeStuff;
using HarmonyLib;
using ShipInventory.Objects;

namespace ShipInventory.Patches;

[HarmonyPatch(typeof(PlayerControllerB))]
public class PlayerControllerB_Patches
{
    /// <summary>
    /// Loads all the items and requests the items from the host
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPatch(nameof(PlayerControllerB.ConnectClientToPlayerObject))]
    private static void RequestOnConnect(PlayerControllerB __instance)
    {
        // If host, skip
        if (__instance.IsHost)
            return;

        ChuteInteract.Instance?.RequestItems();
    }
}