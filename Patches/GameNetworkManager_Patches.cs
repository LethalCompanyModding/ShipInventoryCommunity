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

        string[] keys =
        [
            Constants.STORED_ITEMS,
            Constants.VANILLA_ITEM_IDS,
            Constants.VANILLA_ITEM_POS,
            Constants.VANILLA_ITEM_VALUES,
            Constants.VANILLA_ITEM_DATA
        ];

        // Delete if exist
        foreach (var key in keys)
        {
            if (ES3.KeyExists(key, __instance.currentSaveFileName))
                ES3.DeleteKey(key, __instance.currentSaveFileName);
        }
        
        var items = ChuteInteract.GetItems();

        // Save items if necessary
        if (items.Any())
            ES3.Save(Constants.STORED_ITEMS, items.ToArray(), __instance.currentSaveFileName);
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(nameof(GameNetworkManager.SaveItemsInShip))]
    private static void EnableBackChute()
    {
        ChuteInteract.Instance?.gameObject.SetActive(true);
    }
}