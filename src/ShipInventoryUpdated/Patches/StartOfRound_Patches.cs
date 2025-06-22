using HarmonyLib;
using ShipInventoryUpdated.Scripts;
using Unity.Netcode;
using UnityEngine;
using Logger = ShipInventoryUpdated.Helpers.Logger;

namespace ShipInventoryUpdated.Patches;

[HarmonyPatch(typeof(StartOfRound))]
internal class StartOfRound_Patches
{
    [HarmonyPatch(nameof(StartOfRound.Start)), HarmonyPrefix]
    private static void Start_Prefix(StartOfRound __instance)
    {
        if (!__instance.IsServer)
            return;

        if (ShipInventoryUpdated.INVENTORY_PREFAB == null)
        {
            Logger.Error($"Tried to spawn '{nameof(ShipInventoryUpdated.INVENTORY_PREFAB)}', but it was not defined.");
            return;
        }

        var inventory = Object.Instantiate(ShipInventoryUpdated.INVENTORY_PREFAB);
        inventory.name = $"{nameof(ShipInventoryUpdated)}-{nameof(Inventory)}";
        
        if (inventory.TryGetComponent(out NetworkObject networkObject))
            networkObject.Spawn();
    }
}