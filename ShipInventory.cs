using System.Reflection;
using BepInEx;
using HarmonyLib;
using ShipInventory.Helpers;
using ShipInventory.Objects;
using UnityEngine;

namespace ShipInventory;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class ShipInventory : BaseUnityPlugin
{
    private void Awake()
    {
        Helpers.Logger.SetLogger(Logger);

        // Load bundle
        if (!Bundle.LoadBundle(Constants.BUNDLE))
            return;
    
        // --- Network stuff ---
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
        // ---
        
        // Load prefabs
        NetworkPrefabUtils.LoadPrefab(Constants.VENT_PREFAB, obj => obj.AddComponent<ChuteInteract>());
        
        // Apply patches
        Patch();

        Logger.LogInfo($"{PluginInfo.PLUGIN_GUID} v{PluginInfo.PLUGIN_VERSION} has loaded!");
    }

    #region Patches

    private static Harmony? Harmony { get; set; }

    internal static void Patch()
    {
        Helpers.Logger.Info("Patching...");

        Harmony ??= new Harmony(PluginInfo.PLUGIN_GUID);

        Harmony.PatchAll();

        Helpers.Logger.Info("Finished patching!");
    }

    #endregion
}