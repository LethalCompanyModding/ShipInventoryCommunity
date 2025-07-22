using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using ShipInventoryUpdated.Objects;
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
    
    [HarmonyPatch(nameof(StartOfRound.LoadShipGrabbableItems)), HarmonyPrefix]
    private static void LoadShipGrabbableItems_Prefix()
    {
        string currentSaveFileName = GameNetworkManager.Instance.currentSaveFileName;

        Inventory.Clear();
        
        if (!ES3.KeyExists(ShipInventoryUpdated.SAVE_KEY, currentSaveFileName))
            return;
        
        string json = ES3.Load<string>(ShipInventoryUpdated.SAVE_KEY, currentSaveFileName);
        var items = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<ItemData>>(json);

        if (items == null)
        {
            Logger.Error("Could not load items from the save file.");
            return;
        }
        
        Inventory.Add(items.ToArray());
    }
    
    [HarmonyPatch(nameof(StartOfRound.GetValueOfAllScrap)), HarmonyPostfix]
    private static void GetValueOfAllScrap_Postfix(ref int __result, bool onlyScrapCollected, bool onlyNewScrap)
    {
        foreach (var data in Inventory.Items)
        {
            if (data.PERSISTED_THROUGH_ROUNDS)
                continue;
            
            var item = data.GetItem();
            
            if (item == null)
                continue;
            
            if (!item.isScrap)
                continue;

            __result += data.SCRAP_VALUE;
        }
    }
}