using BepInEx;
using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;

namespace ShipInventoryUpdated;

[BepInPlugin(LCMPluginInfo.PLUGIN_GUID, LCMPluginInfo.PLUGIN_NAME, LCMPluginInfo.PLUGIN_VERSION)]
public class ShipInventoryUpdated : BaseUnityPlugin
{
    internal static GameObject? CHUTE_PREFAB;
    internal static UnlockableItem? CHUTE_UNLOCK_ITEM;

    private void Awake()
    {
        Helpers.Logger.SetLogger(Logger);

        if (!Helpers.Bundle.LoadBundle(Constants.BUNDLE_FILE_NAME))
            return;

        if (!PrepareRPCs())
            return;

        ApplyPatches();

        Helpers.Logger.Info($"{LCMPluginInfo.PLUGIN_GUID} v{LCMPluginInfo.PLUGIN_VERSION} has loaded!");
    }

    #region Patches

    private Harmony? _harmony;

    private void ApplyPatches()
    {
        Helpers.Logger.Debug("Applying patches...");

        _harmony ??= new Harmony(LCMPluginInfo.PLUGIN_GUID);

        _harmony.PatchAll(typeof(Patches.GameNetworkManager_Patches));
        _harmony.PatchAll(typeof(Patches.StartOfRound_Patches));

        Helpers.Logger.Debug("Finished applying patches!");
    }

    #endregion

    private static bool PrepareRPCs()
    {
        try
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                    if (attributes.Length > 0)
                    {
                        method.Invoke(null, null);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Helpers.Logger.Error($"Error while preparing RPCs: '{e.Message}'");
            return false;
        }

        return true;
    }

    internal static GameObject? LoadChute()
    {
        var chutePrefab = Helpers.Bundle.LoadAsset<GameObject>(Constants.CHUTE_PREFAB);

        if (chutePrefab == null)
            return null;

        var autoParent = chutePrefab.GetComponent<AutoParentToShip>();
        autoParent.positionOffset = new Vector3(1.9f, 1f, -4.5f);
        autoParent.rotationOffset = new Vector3(35, 0, 0);
        autoParent.overrideOffset = true;

        Unity.Netcode.NetworkManager.Singleton.AddNetworkPrefab(chutePrefab);
        return chutePrefab;
    }

    internal static UnlockableItem? LoadTerminalNode(GameObject prefab)
    {
        var inventoryBuyNode = Helpers.Bundle.LoadAsset<TerminalNode>(Constants.INVENTORY_BUY_TERMINAL_NODE);

        Helpers.Logger.Info($"Unlock ID: {inventoryBuyNode.shipUnlockableID}");
        inventoryBuyNode.shipUnlockableID = 901;

        if (inventoryBuyNode == null)
            return null;

        var unlock = new UnlockableItem
        {
            unlockableName = "ship inventory",
            prefabObject = prefab,
            unlockableType = 1,
            shopSelectionNode = null,
            alwaysInStock = true,
            IsPlaceable = true,
            canBeStored = true,
            maxNumber = 1,
            spawnPrefab = true,
        };

        LethalLib.Modules.Unlockables.RegisterUnlockable(
            unlock,
            LethalLib.Modules.StoreType.ShipUpgrade,
            null!,
            null!,
            inventoryBuyNode,
            60
        );
        return unlock;
    }
}
