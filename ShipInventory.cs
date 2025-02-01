using System;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using InteractiveTerminalAPI.UI;
using LethalLib.Modules;
using ShipInventory.Applications;
using ShipInventory.Compatibility;
using ShipInventory.Helpers;
using ShipInventory.Objects;
using ShipInventory.Patches;
using UnityEngine;

namespace ShipInventory;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("WhiteSpike.InteractiveTerminalAPI", "1.2.0")]
[BepInDependency("com.sigurd.csync", "5.0.1")]
[BepInDependency(LethalLib.Plugin.ModGUID, LethalLib.Plugin.ModVersion)]
[BepInDependency(LethalConfigCompatibility.LETHAL_CONFIG, BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency(CustomItemBehaviourLibraryCompatibility.CUSTOM_ITEM_BEHAVIOUR_LIBRARY, BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency(OpenMonitorsCompatibility.OPEN_MONITOR, BepInDependency.DependencyFlags.SoftDependency)]
public class ShipInventory : BaseUnityPlugin
{
    public new static Config Config = null!;
    
    private void Awake()
    {
        Helpers.Logger.SetLogger(Logger);
        
        Config = new Config(base.Config);
        
        if (!Bundle.LoadBundle(Constants.BUNDLE_MAIN))
            return;
        
        if (!LoadFallbackItem())
            return;

        if (!PrepareRPCs())
            return;
        
        ApplyPatches();

        InteractiveTerminalManager.RegisterApplication<ShipApplication>(Config.InventoryCommand.Value, true);
        
        Helpers.Logger.Info($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
    }

    #region Patches

    private Harmony? _harmony;

    private void ApplyPatches()
    {
        Helpers.Logger.Debug("Applying patches...");

        _harmony ??= new Harmony(MyPluginInfo.PLUGIN_GUID);

        _harmony.PatchAll(typeof(GameNetworkManager_Patches));
        _harmony.PatchAll(typeof(RoundManager_Patches));
        _harmony.PatchAll(typeof(StartOfRound_Patches));
        
        if (OpenMonitorsCompatibility.Enabled)
            OpenMonitorsCompatibility.PatchAll(_harmony);

        Helpers.Logger.Debug("Finished applying patches!");
    }

    private void RemovePatches()
    {
        Helpers.Logger.Debug("Removing patches...");

        _harmony?.UnpatchSelf();
        
        Helpers.Logger.Debug("Finished removing patches!");
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
    
    private static bool LoadFallbackItem()
    {
        var errorItem = Bundle.LoadAsset<Item>(Constants.ERROR_ITEM_ASSET);

        if (errorItem == null)
            return false;
        
        var badItem = errorItem.spawnPrefab.AddComponent<BadItem>();
        badItem.itemProperties = errorItem;
            
        NetworkPrefabs.RegisterNetworkPrefab(errorItem.spawnPrefab);
        Items.RegisterItem(errorItem);
        
        ItemData.FALLBACK_ITEM = errorItem;
        return true;
    }
    internal static bool LoadChute(out GameObject? prefab)
    {
        prefab = null;

        var chutePrefab = Bundle.LoadAsset<GameObject>(Constants.CHUTE_PREFAB);

        if (chutePrefab == null)
            return false;
        
        chutePrefab.AddComponent<ChuteInteract>();

        var autoParent = chutePrefab.GetComponent<AutoParentToShip>();
        ChuteInteract.SetOffsets(autoParent);
        autoParent.overrideOffset = true;
        
        Unity.Netcode.NetworkManager.Singleton.AddNetworkPrefab(chutePrefab);
        prefab = chutePrefab;
        return true;
    }
    internal static bool LoadTerminalNode(GameObject prefab)
    {
        var inventoryBuyNode = Bundle.LoadAsset<TerminalNode>(Constants.INVENTORY_BUY_TERMINAL_NODE);

        if (inventoryBuyNode == null)
            return false;
        
        var unlock = new UnlockableItem {
            unlockableName = Config.ChuteUnlockName.Value,
            prefabObject = prefab,
            unlockableType = 1,
            shopSelectionNode = null,
            alwaysInStock = true,
            IsPlaceable = true,
            canBeStored = true,
            maxNumber = 1,
            spawnPrefab = true,
        };
        
        Unlockables.RegisterUnlockable(
            unlock, 
            StoreType.ShipUpgrade, 
            null!, 
            null!, 
            inventoryBuyNode, 
            Config.ChuteUnlockCost.Value
        );
        ChuteInteract.UnlockableItem = unlock;
        return true;
    }
}