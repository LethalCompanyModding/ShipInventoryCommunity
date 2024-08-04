using GameNetcodeStuff;
using HarmonyLib;
using ShipInventory.Objects;

namespace ShipInventory.Patches;

[HarmonyPatch(typeof(PlayerControllerB))]
public class PlayerControllerB_Patches
{
    [HarmonyPostfix]
    [HarmonyPatch(nameof(PlayerControllerB.ConnectClientToPlayerObject))]
    private static void RequestOnConnect(PlayerControllerB __instance)
    {
        // If not client, skip
        if (!__instance.IsClient)
            return;
        
        // If local not server, skip
        if (StartOfRound.Instance.localPlayerController.IsServer)
            return;

        ChuteInteract.Instance?.RequestItems();
    }
}