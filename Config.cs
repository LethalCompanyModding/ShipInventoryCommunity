using BepInEx.Configuration;
using CSync.Extensions;
using CSync.Lib;
using ShipInventory.Compatibility;
using ShipInventory.Helpers;

namespace ShipInventory;

public class Config : SyncedConfig2<Config>
{
    #region Entries

    public readonly ConfigEntry<string> LangUsed;
    
    [SyncedEntryField] public readonly SyncedEntry<string> Blacklist;
    [SyncedEntryField] public readonly SyncedEntry<float> SpawnDelay;
    [SyncedEntryField] public readonly SyncedEntry<bool> RequireInOrbit;
    [SyncedEntryField] public readonly SyncedEntry<int> StopAfter;
    
    [SyncedEntryField] public readonly SyncedEntry<bool> ActAsSafe;
    [SyncedEntryField] public readonly SyncedEntry<int> MaxItemCount;
    [SyncedEntryField] public readonly SyncedEntry<bool> PersistThroughFire;
    
    [SyncedEntryField] public readonly SyncedEntry<bool> ShowConfirmation;
    [SyncedEntryField] public readonly SyncedEntry<bool> YesPlease;
    [SyncedEntryField] public readonly SyncedEntry<bool> ShowTrademark;

    [SyncedEntryField] public readonly SyncedEntry<OverrideMode> OverrideTrigger;
    public enum OverrideMode { NONE, NEVER, ALL }

    #endregion

    public Config(ConfigFile cfg) : base(MyPluginInfo.PLUGIN_GUID)
    {
        LangUsed = cfg.Bind("Language", "Lang", "en");
        Lang.LoadLang(LangUsed.Value);
        
        #region Chute

        string CHUTE = Lang.Get("CHUTE_SECTION");
        
        Blacklist = cfg.BindSyncedEntry(
            new ConfigDefinition(CHUTE, "ChuteBlacklist"),
            "",
            new ConfigDescription(Lang.Get("DESCRIPTION_BLACKLIST"))
        );
        Blacklist.Changed += (_, e) => ItemManager.UpdateBlacklist(e.NewValue);
        ItemManager.UpdateBlacklist(Blacklist.Value);

        SpawnDelay = cfg.BindSyncedEntry(
            new ConfigDefinition(CHUTE, "ChuteDelay"),
            0.5f,
            new ConfigDescription(Lang.Get("DESCRIPTION_SPAWN_DELAY"))
        );
        
        RequireInOrbit = cfg.BindSyncedEntry(
            new ConfigDefinition(CHUTE, "ChuteInOrbit"),
            false,
            new ConfigDescription(Lang.Get("DESCRIPTION_REQUIRE_IN_ORBIT"))
        );

        StopAfter = cfg.BindSyncedEntry(
            new ConfigDefinition(CHUTE, "ChuteMaxCapacity"),
            30,
            new ConfigDescription(Lang.Get("DESCRIPTION_STOP_AFTER"))
        );

        #endregion

        #region Inventory

        string INVENTORY = Lang.Get("INVENTORY_SECTION");

        ActAsSafe = cfg.BindSyncedEntry(
            new ConfigDefinition(INVENTORY, "ChuteSafe"),
            false,
            new ConfigDescription(Lang.Get("DESCRIPTION_AS_SAFE"))
        );
        
        MaxItemCount = cfg.BindSyncedEntry(
            new ConfigDefinition(INVENTORY, "MaxItemCount"),
            1_969_420,
            new ConfigDescription(Lang.Get("DESCRIPTION_MAX_ITEM_COUNT"))
        );

        PersistThroughFire = cfg.BindSyncedEntry(
            new ConfigDefinition(INVENTORY, "PersistThroughFire"),
            false,
            new ConfigDescription(Lang.Get("DESCRIPTION_PERSIST_THROUGH_FIRE"))
        );
        
        #endregion
        
        #region Terminal

        string TERMINAL = Lang.Get("TERMINAL_SECTION");

        ShowConfirmation = cfg.BindSyncedEntry(
            new ConfigDefinition(TERMINAL, "ShowConfirmation"),
            true,
            new ConfigDescription(Lang.Get("DESCRIPTION_SHOW_CONFIRMATION"))
        );
        
        YesPlease = cfg.BindSyncedEntry(
            new ConfigDefinition(TERMINAL, "YesPlease"),
            false,
            new ConfigDescription(Lang.Get("DESCRIPTION_YES_PLEASE"))
        );
        
        ShowTrademark = cfg.BindSyncedEntry(
            new ConfigDefinition(TERMINAL, "ShowTrademark"),
            true,
            new ConfigDescription(Lang.Get("DESCRIPTION_SHOW_TRADEMARK"))
        );
        
        #endregion

        #region Debug

        string DEBUG = Lang.Get("DEBUG_SECTION");

        OverrideTrigger = cfg.BindSyncedEntry(
            new ConfigDefinition(DEBUG, "OverrideTrigger"),
            OverrideMode.NONE,
            new ConfigDescription(string.Format(
                Lang.Get("DESCRIPTION_OVERRIDE_TRIGGER"),
                nameof(OverrideMode.NONE),
                nameof(OverrideMode.NEVER),
                nameof(OverrideMode.ALL)
            ))
        );
        
        #endregion

        if (LethalConfigCompatibility.enabled)
            LethalConfigCompatibility.AddConfigs(this);
    }
}