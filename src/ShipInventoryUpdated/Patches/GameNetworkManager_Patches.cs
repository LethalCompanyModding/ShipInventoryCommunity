using HarmonyLib;
using Unity.Netcode;
using ShipInventoryUpdated.Helpers;
using ShipInventoryUpdated.Scripts;

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

        if (ShipInventoryUpdated.CHUTE_PREFAB is not null)
            NetworkManager.Singleton.AddNetworkPrefab(ShipInventoryUpdated.CHUTE_PREFAB);
        else
            Logger.Error($"Tried to add '{nameof(ShipInventoryUpdated.CHUTE_PREFAB)}' to the network, but it was not defined.");

        if (ShipInventoryUpdated.INVENTORY_PREFAB is not null)
            NetworkManager.Singleton.AddNetworkPrefab(ShipInventoryUpdated.INVENTORY_PREFAB);
        else
            Logger.Error($"Tried to add '{nameof(ShipInventoryUpdated.INVENTORY_PREFAB)}' to the network, but it was not defined.");
    }
    
    [HarmonyPatch(nameof(GameNetworkManager.SaveItemsInShip)), HarmonyPrefix]
    private static void SaveItemsInShip_Prefix(GameNetworkManager __instance)
    {
        string currentSaveFileName = GameNetworkManager.Instance.currentSaveFileName;

        // Delete items
        ES3.DeleteKey("shipGrabbableItemIDs", currentSaveFileName);
        ES3.DeleteKey("shipGrabbableItemPos", currentSaveFileName);
        ES3.DeleteKey("shipScrapValues", currentSaveFileName);
        ES3.DeleteKey("shipItemSaveData", currentSaveFileName);

        if (Inventory.Count == 0)
        {
            ES3.DeleteKey(ShipInventoryUpdated.SAVE_KEY, currentSaveFileName);
            return;
        }
        
        ES3.Save(ShipInventoryUpdated.SAVE_KEY, Newtonsoft.Json.JsonConvert.SerializeObject(Inventory.Items), currentSaveFileName);
    }
}