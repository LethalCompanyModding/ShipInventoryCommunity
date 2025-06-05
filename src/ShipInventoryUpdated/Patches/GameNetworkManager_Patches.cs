using HarmonyLib;

namespace ShipInventoryUpdated.Patches;

[HarmonyPatch(typeof(GameNetworkManager))]
internal class GameNetworkManager_Patches
{
    [HarmonyPatch(nameof(GameNetworkManager.Start)), HarmonyPostfix]
    private static void Start_Postfix()
    {
        
    }
}