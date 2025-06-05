using HarmonyLib;
using Unity.Netcode;
using ShipInventoryUpdated.Helpers;

namespace ShipInventoryUpdated.Patches;

[HarmonyPatch(typeof(GameNetworkManager))]
internal class GameNetworkManager_Patches
{
    [HarmonyPatch(nameof(GameNetworkManager.Start)), HarmonyPostfix]
    private static void Start_Postfix()
    {
        if (!NetworkManager.Singleton)
        {
            Logger.Error($"Tried to add prefabs to the network, but '{nameof(NetworkManager.Singleton)}' is not defined.");
            return;
        }

        if (!ShipInventoryUpdated.CHUTE_PREFAB)
        {
            Logger.Error($"Tried to add '{nameof(ShipInventoryUpdated.CHUTE_PREFAB)}' to the network, but it was not defined.");
            return;
        }
        
        NetworkManager.Singleton.AddNetworkPrefab(ShipInventoryUpdated.CHUTE_PREFAB);
    }
}