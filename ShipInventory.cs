using System.Reflection;
using BepInEx;
using HarmonyLib;
using InteractiveTerminalAPI.UI;
using LethalConfig;
using ShipInventory.Applications;
using ShipInventory.Helpers;
using ShipInventory.Objects;
using UnityEngine;

namespace ShipInventory;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInDependency("WhiteSpike.InteractiveTerminalAPI")]
[BepInDependency("com.sigurd.csync", "5.0.1")] 
[BepInDependency("ainavt.lc.lethalconfig")]
public class ShipInventory : BaseUnityPlugin
{
    public new static Config Config = null!;
    
    private void Awake()
    {
        Helpers.Logger.SetLogger(Logger);

        Config = new Config(base.Config);
        
        // Load bundle
        if (!Bundle.LoadBundle(Constants.BUNDLE))
            return;

        LethalConfigManager.SetModIcon(Bundle.LoadAsset<Sprite>(Constants.MOD_ICON));
        LethalConfigManager.SetModDescription("Adds an inventory to the ship, allowing it to store items and retrieve them.");
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
        var grab = obj.AddComponent<VentProp>();
            
        // Vent Prop Item
        var item = ScriptableObject.CreateInstance<Item>();
        item.isScrap = true;
        item.lockedInDemo = true;
        item.itemName = "VENT_CHUTE";
        item.spawnPrefab = NetworkPrefabUtils.GetPrefab(Constants.VENT_PREFAB);
        item.saveItemVariable = true;
        
        grab.itemProperties = item;
        grab.grabbable = false;
        grab.grabbableToEnemies = false;
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
        
        // GRABBABLE
        var grabObj = vent.GetComponent<GrabbableObject>();

        if (Config.LetAsItem.Value)
        {
            grabObj.isInElevator = true;
            grabObj.isInShipRoom = true;
            grabObj.scrapPersistedThroughRounds = true;
            //grabObj.isHeld = true;
            //grabObj.isHeldByEnemy = true;
            grabObj.OnHitGround();
        }
        else if (grabObj != null)
            Destroy(grabObj);
        
        // Update scrap value of the chute
        ItemManager.UpdateValue();
    }

    #endregion
}