using HarmonyLib;
using BepInEx;

namespace ShipInventoryUpdated.Patches;

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
        var chutePrefab = ShipInventoryUpdated.LoadChute();

        if (chutePrefab == null)
            return;

        ShipInventoryUpdated.CHUTE_PREFAB = chutePrefab;

        var unlockNode = ShipInventoryUpdated.LoadTerminalNode(chutePrefab);

        if (unlockNode == null)
            return;
        
        ShipInventoryUpdated.CHUTE_UNLOCK_ITEM = unlockNode;
    }
}