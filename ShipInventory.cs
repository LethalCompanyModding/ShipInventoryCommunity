using System.Linq;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using InteractiveTerminalAPI.UI;
using LethalLib.Modules;
using ShipInventory.Applications;
using ShipInventory.Compatibility;
using ShipInventory.Helpers;
using ShipInventory.Objects;
using UnityEngine;

namespace ShipInventory;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("WhiteSpike.InteractiveTerminalAPI", "1.2.0")]
[BepInDependency("com.sigurd.csync", "5.0.1")]
[BepInDependency(LethalLib.Plugin.ModGUID, LethalLib.Plugin.ModVersion)]
[BepInDependency("SylviBlossom.SmartItemSaving", "1.2.4")]
[BepInDependency(LethalConfigCompatibility.LETHAL_CONFIG, BepInDependency.DependencyFlags.SoftDependency)]
public class ShipInventory : BaseUnityPlugin
{
    public new static Config Config = null!;
    
    private void Awake()
    {
        Helpers.Logger.SetLogger(Logger);
        
        // Load bundle
        if (!Bundle.LoadBundle(Constants.BUNDLE))
        {
            Helpers.Logger.Error("Failed to load the bundle. This mod will not continue further.");
            return;
        }

        Config = new Config(base.Config);
        
        PrepareNetwork();
        PrepareItems();
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

        _harmony.PatchAll();

        Helpers.Logger.Debug("Finished applying patches!");
    }

    private void RemovePatches()
    {
        Helpers.Logger.Debug("Removing patches...");

        _harmony?.UnpatchSelf();
        
        Helpers.Logger.Debug("Finished removing patches!");
    }

    #endregion
    
    #region Network

    private static void PrepareNetwork()
    {
        Helpers.Logger.Debug("Prepare RPCs...");
        
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
        Helpers.Logger.Debug("RPCs prepared!");
        
        Helpers.Logger.Debug("Registering all prefabs...");
        NetworkPrefabUtils.Register(Constants.VENT_PREFAB, LoadVent, ChuteInteract.IsUpgrade);
        Helpers.Logger.Debug("All prefabs registered!");
    }

    private static void LoadVent(GameObject vent)
    {
        vent.AddComponent<ChuteInteract>();
        
        var autoParent = vent.GetComponent<AutoParentToShip>();
        ChuteInteract.SetOffsets(autoParent);
        autoParent.overrideOffset = true;
        
        if (!ChuteInteract.IsUpgrade)
            return;

        var terminalNode = Bundle.LoadAsset<TerminalNode>("InventoryBuy");

        if (terminalNode == null)
        {
            Helpers.Logger.Error("Could not find the terminal node for the inventory. The chute is now always unlocked.");
            ChuteInteract.IsUpgrade = false;
            return;
        }
        
        var unlock = new UnlockableItem
        {
            unlockableName = "Ship Inventory",
            prefabObject = vent,
            unlockableType = 1,
            shopSelectionNode = null,
            alwaysInStock = true,
            IsPlaceable = true,
            canBeStored = true,
            maxNumber = 1,
            spawnPrefab = true
        };
        
        Unlockables.RegisterUnlockable(
            unlock, 
            StoreType.ShipUpgrade, 
            null!, 
            null!, 
            terminalNode, 
            Config.ChuteUnlockCost.Value
        );

        Config.ChuteUnlockCost.Changed += (_, e) =>
        {
            Unlockables.UpdateUnlockablePrice(unlock, e.NewValue);
        };
    }
    
    #endregion
    
    #region Items

    public static void PrepareItems()
    {
        Helpers.Logger.Debug("Preparing items...");
        Item? errorItem = Bundle.LoadAsset<Item>(Constants.ERROR_ITEM_ASSET);

        if (errorItem != null)
        {
            var badItem = errorItem.spawnPrefab.AddComponent<BadItem>();
            badItem.itemProperties = errorItem;
            
            NetworkPrefabs.RegisterNetworkPrefab(errorItem.spawnPrefab);
            Items.RegisterItem(errorItem);

            ItemData.FALLBACK_ITEM = errorItem;
        }
        else
        {
            var allItems = Resources.FindObjectsOfTypeAll<Item>();
            ItemData.FALLBACK_ITEM = allItems.FirstOrDefault(i => i.itemName == "Gold bar") ?? allItems[0];
        }
        
        Helpers.Logger.Debug("Items prepared!");
    }

    #endregion
}