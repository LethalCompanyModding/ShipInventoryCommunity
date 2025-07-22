using System;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using ShipInventoryUpdated.Configurations;
using ShipInventoryUpdated.Helpers.API;
using UnityEngine;

namespace ShipInventoryUpdated;

[BepInPlugin(LCMPluginInfo.PLUGIN_GUID, LCMPluginInfo.PLUGIN_NAME, LCMPluginInfo.PLUGIN_VERSION)]
// Hard
[BepInDependency("WhiteSpike.InteractiveTerminalAPI", "1.2.0")]
// Soft
[BepInDependency(LethalConfig.PluginInfo.Guid, BepInDependency.DependencyFlags.SoftDependency)]
public class ShipInventoryUpdated : BaseUnityPlugin
{
    private void Awake()
    {
        Helpers.Logger.SetLogger(Logger);

        if (!LoadAssets("si-bundle"))
            return;
        
        if (!PrepareRPCs())
            return;

        var language = Helpers.Localization.LoadLanguage(DEFAULT_LANG);
        Helpers.Localization.SetAsDefault(language);
        
        Configuration.Load(Config);
        Helpers.Dependencies.LoadDependencies(Configuration.Instance);
        Patch();

        InteractionHelper.LoadConditions();
        ItemConverter.LoadConversions();
        
        Helpers.Logger.Info($"{LCMPluginInfo.PLUGIN_GUID} v{LCMPluginInfo.PLUGIN_VERSION} has loaded!");
    }

    #region Constants

    public const string DEFAULT_LANG = "en";
    public const string SAVE_KEY = "shipInventoryItems";

    #endregion

    #region Bundle

    internal static GameObject? INVENTORY_PREFAB;
    internal static GameObject? CHUTE_PREFAB;
    internal static TerminalNode? CHUTE_BUY_NODE;
    internal static TerminalNode? CHUTE_CONFIRM_NODE;

    private static bool LoadAssets(string name)
    {
        if (!Helpers.Bundle.LoadBundle(name))
            return false;

        INVENTORY_PREFAB = Helpers.Bundle.LoadAsset<GameObject>("InventoryPrefab");
        CHUTE_PREFAB = Helpers.Bundle.LoadAsset<GameObject>("ChutePrefab");
        CHUTE_BUY_NODE = Helpers.Bundle.LoadAsset<TerminalNode>("ChuteBuy");
        CHUTE_CONFIRM_NODE = Helpers.Bundle.LoadAsset<TerminalNode>("ChuteConfirm");

        return true;
    }

    #endregion

    #region Patches

    private Harmony? Harmony;

    private void Patch()
    {
        Harmony = new Harmony(LCMPluginInfo.PLUGIN_GUID);
        Harmony.PatchAll(typeof(Patches.GameNetworkManager_Patches));
        Harmony.PatchAll(typeof(Patches.StartOfRound_Patches));
        Harmony.PatchAll(typeof(Patches.Terminal_Patches));
    }

    #endregion
    
    #region RPCs

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

    #endregion
}
