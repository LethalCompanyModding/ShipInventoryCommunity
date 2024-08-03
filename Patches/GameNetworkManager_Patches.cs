using System.Linq;
using HarmonyLib;
using ShipInventory.Objects;

namespace ShipInventory.Patches;

[HarmonyPatch(typeof(GameNetworkManager))]
public class GameNetworkManager_Patches
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(GameNetworkManager.SaveItemsInShip))]
    private static void SaveStoredItems(GameNetworkManager __instance)
    {
        ChuteInteract.Instance?.gameObject.SetActive(false);
        
        ES3.DeleteKey(Constants.VANILLA_ITEM_IDS, __instance.currentSaveFileName);
        ES3.DeleteKey(Constants.VANILLA_ITEM_POS, __instance.currentSaveFileName);
        ES3.DeleteKey(Constants.VANILLA_ITEM_VALUES, __instance.currentSaveFileName);
        ES3.DeleteKey(Constants.VANILLA_ITEM_DATA, __instance.currentSaveFileName);
        
        var items = ChuteInteract.GetItems();
        
        // Delete keys if empty
        if (items.Any())
        {
            ES3.DeleteKey(Constants.STORED_ITEMS, __instance.currentSaveFileName);
            return;
        }
        
        ES3.Save(Constants.STORED_ITEMS, items.ToArray(), __instance.currentSaveFileName);
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(nameof(GameNetworkManager.SaveItemsInShip))]
    private static void EnableBackChute()
    {
        ChuteInteract.Instance?.gameObject.SetActive(true);
    }
}