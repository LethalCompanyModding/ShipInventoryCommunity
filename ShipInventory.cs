using System.Reflection;
using BepInEx;
using HarmonyLib;
using InteractiveTerminalAPI.UI;
using ShipInventory.Applications;
using ShipInventory.Helpers;
using ShipInventory.Objects;
using UnityEngine;

namespace ShipInventory;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("WhiteSpike.InteractiveTerminalAPI")]
public class ShipInventory : BaseUnityPlugin
{
    internal static ShipInventoryConfigs CONFIG { get; private set; } = null!; 
    
    private void Awake()
    {
        CONFIG = new ShipInventoryConfigs(Config); 
        
        Helpers.Logger.SetLogger(Logger);

        // Load bundle
        if (!Bundle.LoadBundle(Constants.BUNDLE))
            return;

        PrepareNetwork();
        Patch();

        InteractiveTerminalManager.RegisterApplication<ShipApplication>("ship", true);
        
        Helpers.Logger.Info($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
    }

    #region Patches

    private static Harmony? Harmony { get; set; }

    private static void Patch()
    {
        Helpers.Logger.Debug("Patching...");

        Harmony ??= new Harmony(MyPluginInfo.PLUGIN_GUID);

        Harmony.PatchAll();

        Helpers.Logger.Debug("Finished patching!");
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
        
        Helpers.Logger.Debug("Loading all prefabs...");
        NetworkPrefabUtils.LoadPrefab(Constants.VENT_PREFAB, obj => {
            obj.AddComponent<ChuteInteract>();
            var grab = obj.AddComponent<VentProp>();
            
            // Vent Prop Item
            var item = ScriptableObject.CreateInstance<Item>();
            item.isScrap = true;
            item.lockedInDemo = true;
            item.itemName = "VENT_CHUTE";
            item.spawnPrefab = NetworkPrefabUtils.GetPrefab(Constants.VENT_PREFAB);
            item.saveItemVariable = true;
        
            grab.itemProperties = item;
        });
        Helpers.Logger.Debug("All prefabs loaded!");
    }

    #endregion
}