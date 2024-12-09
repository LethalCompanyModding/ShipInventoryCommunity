using System.Linq;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using InteractiveTerminalAPI.UI;
using ShipInventory.Applications;
using ShipInventory.Compatibility;
using ShipInventory.Helpers;
using ShipInventory.Objects;
using UnityEngine;

namespace ShipInventory;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("WhiteSpike.InteractiveTerminalAPI")]
[BepInDependency("com.sigurd.csync", "5.0.1")]
[BepInDependency("evaisa.lethallib", "0.16.1")]
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
        NetworkPrefabUtils.LoadPrefab(new NetworkPrefabUtils.PrefabData
        {
            name = Constants.VENT_PREFAB,
            onLoad = LoadVent,
            onSetup = SetUpVent
        });
        Helpers.Logger.Debug("All prefabs loaded!");
    }

    private static void LoadVent(GameObject obj)
    {
        obj.AddComponent<ChuteInteract>();
            
        // Vent Prop Item
        var item = ScriptableObject.CreateInstance<Item>();
        item.isScrap = true;
        item.lockedInDemo = true;
        item.itemName = "VENT_CHUTE";
        item.spawnPrefab = NetworkPrefabUtils.GetPrefab(Constants.VENT_PREFAB);
        item.saveItemVariable = true;
    }
    private static void SetUpVent(GameObject vent)
    {
        var chute = vent.GetComponent<ChuteInteract>();
        ChuteInteract.Instance = chute;
        
        // TRIGGER
        var interact = chute.GetComponent<InteractTrigger>();
        interact.onInteract.AddListener(chute.StoreHeldItem);
        interact.timeToHold = 0.5f;

        // TRANSFORM
        vent.transform.localPosition = new Vector3(1.9f, 1f, -4.5f);
        vent.transform.localRotation = Quaternion.Euler(35, 0, 0);
        
        // Update scrap value of the chute
        chute.UpdateValue();
    }

    #endregion
    #region Items

    public static void PrepareItems()
    {
        Helpers.Logger.Debug("Preparing items...");
        var allItems = Resources.FindObjectsOfTypeAll<Item>();
        ItemData.FALLBACK_ITEM = allItems.FirstOrDefault(i => i.itemName == "Gold bar") ?? allItems[0];
        Helpers.Logger.Debug("Items prepared!");
    }

    #endregion
}