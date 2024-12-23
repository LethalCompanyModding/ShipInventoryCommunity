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
    
    // Chute
    [SyncedEntryField] public readonly SyncedEntry<PermissionLevel> ChutePermission;
    [SyncedEntryField] public readonly SyncedEntry<bool> RequireInOrbit;
    [SyncedEntryField] public readonly SyncedEntry<float> TimeToStore;
    [SyncedEntryField] public readonly SyncedEntry<float> TimeToRetrieve;
    [SyncedEntryField] public readonly SyncedEntry<int> StopAfter;
    [SyncedEntryField] public readonly SyncedEntry<string> Blacklist;
    
    // Inventory
    [SyncedEntryField] public readonly SyncedEntry<PermissionLevel> InventoryPermission;
    [SyncedEntryField] public readonly SyncedEntry<bool> ActAsSafe;
    [SyncedEntryField] public readonly SyncedEntry<bool> PersistThroughFire;
    [SyncedEntryField] public readonly SyncedEntry<int> MaxItemCount;
    
    // Terminal
    [SyncedEntryField] public readonly SyncedEntry<string> InventoryCommand;
    [SyncedEntryField] public readonly SyncedEntry<bool> YesPlease;
    [SyncedEntryField] public readonly SyncedEntry<bool> ShowConfirmation;
    [SyncedEntryField] public readonly SyncedEntry<bool> ShowTrademark;

    public enum PermissionLevel { HOST_ONLY, CLIENTS_ONLY, EVERYONE, NO_ONE  }

    #endregion

    public Config(ConfigFile cfg) : base(MyPluginInfo.PLUGIN_GUID)
    {
        LangUsed = cfg.Bind("Language", "Language", Lang.DEFAULT_LANG);
        Lang.LoadLang(LangUsed.Value);
        
        #region Chute

        string CHUTE = Lang.Get("CHUTE_SECTION");
        
        ChutePermission = cfg.BindSyncedEntry(
            new ConfigDefinition(CHUTE, "ChutePermission"),
            PermissionLevel.EVERYONE,
            new ConfigDescription(Lang.Get("DESCRIPTION_CHUTE_PERMISSION"))
        );
        
        RequireInOrbit = cfg.BindSyncedEntry(
            new ConfigDefinition(CHUTE, "ChuteInOrbit"),
            false,
            new ConfigDescription(Lang.Get("DESCRIPTION_REQUIRE_IN_ORBIT"))
        );
        
        TimeToStore = cfg.BindSyncedEntry(
            new ConfigDefinition(CHUTE, "TimeToStore"),
            0.5f,
            new ConfigDescription(Lang.Get("DESCRIPTION_TIME_TO_STORE"))
        );
        
        TimeToRetrieve = cfg.BindSyncedEntry(
            new ConfigDefinition(CHUTE, "ChuteDelay"),
            0.5f,
            new ConfigDescription(Lang.Get("DESCRIPTION_TIME_TO_RETRIEVE"))
        );
        
        StopAfter = cfg.BindSyncedEntry(
            new ConfigDefinition(CHUTE, "ChuteMaxCapacity"),
            30,
            new ConfigDescription(Lang.Get("DESCRIPTION_STOP_AFTER"))
        );
        
        Blacklist = cfg.BindSyncedEntry(
            new ConfigDefinition(CHUTE, "ChuteBlacklist"),
            "",
            new ConfigDescription(Lang.Get("DESCRIPTION_BLACKLIST"))
        );
        Blacklist.Changed += (_, e) => ItemManager.UpdateBlacklist(e.NewValue);
        ItemManager.UpdateBlacklist(Blacklist.Value);

        #endregion

        #region Inventory

        string INVENTORY = Lang.Get("INVENTORY_SECTION");

        InventoryPermission = cfg.BindSyncedEntry(
            new ConfigDefinition(INVENTORY, "InventoryPermission"),
            PermissionLevel.EVERYONE,
            new ConfigDescription(Lang.Get("DESCRIPTION_INVENTORY_PERMISSION"))
        );
        
        ActAsSafe = cfg.BindSyncedEntry(
            new ConfigDefinition(INVENTORY, "ChuteSafe"),
            false,
            new ConfigDescription(Lang.Get("DESCRIPTION_AS_SAFE"))
        );
        
        PersistThroughFire = cfg.BindSyncedEntry(
            new ConfigDefinition(INVENTORY, "PersistThroughFire"),
            false,
            new ConfigDescription(Lang.Get("DESCRIPTION_PERSIST_THROUGH_FIRE"))
        );
        
        MaxItemCount = cfg.BindSyncedEntry(
            new ConfigDefinition(INVENTORY, "MaxItemCount"),
            5_000,
            new ConfigDescription(Lang.Get("DESCRIPTION_MAX_ITEM_COUNT"))
        );
        
        #endregion
        
        #region Terminal

        string TERMINAL = Lang.Get("TERMINAL_SECTION");

        InventoryCommand = cfg.BindSyncedEntry(
            new ConfigDefinition(TERMINAL, "InventoryCommand"),
            "ship",
            new ConfigDescription(Lang.Get("DESCRIPTION_INVENTORY_COMMAND"))
        );
        
        YesPlease = cfg.BindSyncedEntry(
            new ConfigDefinition(TERMINAL, "YesPlease"),
            false,
            new ConfigDescription(Lang.Get("DESCRIPTION_YES_PLEASE"))
        );
        
        ShowConfirmation = cfg.BindSyncedEntry(
            new ConfigDefinition(TERMINAL, "ShowConfirmation"),
            true,
            new ConfigDescription(Lang.Get("DESCRIPTION_SHOW_CONFIRMATION"))
        );
        
        ShowTrademark = cfg.BindSyncedEntry(
            new ConfigDefinition(TERMINAL, "ShowTrademark"),
            true,
            new ConfigDescription(Lang.Get("DESCRIPTION_SHOW_TRADEMARK"))
        );
        
        #endregion

        if (LethalConfigCompatibility.enabled)
            LethalConfigCompatibility.AddConfigs(this);
    }
}