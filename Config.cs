using BepInEx.Configuration;
using CSync.Extensions;
using CSync.Lib;
using ShipInventory.Compatibility;
using ShipInventory.Helpers;
using ShipInventory.Objects;

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
    [SyncedEntryField] public readonly SyncedEntry<float> KeepRate;
    
    // Terminal
    [SyncedEntryField] public readonly SyncedEntry<string> InventoryCommand;
    [SyncedEntryField] public readonly SyncedEntry<bool> YesPlease;
    [SyncedEntryField] public readonly SyncedEntry<bool> ShowConfirmation;
    [SyncedEntryField] public readonly SyncedEntry<bool> ShowTrademark;
    
    // Network
    [SyncedEntryField] public readonly SyncedEntry<float> InventoryRefreshRate;
    [SyncedEntryField] public readonly SyncedEntry<bool> InventoryUpdateCheckSilencer;
    [SyncedEntryField] public readonly SyncedEntry<bool> ForceUpdateUponAdding;
    [SyncedEntryField] public readonly SyncedEntry<bool> ForceUpdateUponRemoving;
    
    // Unlock
    [SyncedEntryField] public readonly SyncedEntry<bool> ChuteIsUnlock;
    [SyncedEntryField] public readonly SyncedEntry<int> ChuteUnlockCost;
    
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
        
        KeepRate = cfg.BindSyncedEntry(
            new ConfigDefinition(INVENTORY, "KeepRate"),
            0f,
            new ConfigDescription(Lang.Get("DESCRIPTION_KEEP_RATE"))
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

        #region Network

        string NETWORK = Lang.Get("NETWORK_SECTION");
        
        InventoryRefreshRate = cfg.BindSyncedEntry(
            new ConfigDefinition(NETWORK, "InventoryRefreshRate"),
            15f,
            new ConfigDescription(Lang.Get("DESCRIPTION_INVENTORY_REFRESH_RATE"))
        );
        
        InventoryUpdateCheckSilencer = cfg.BindSyncedEntry(
            new ConfigDefinition(NETWORK, "InventoryUpdateCheckSilencer"),
            false,
            new ConfigDescription(Lang.Get("DESCRIPTION_INVENTORY_UPDATE_CHECK_SILENCER"))
        );
        InventoryUpdateCheckSilencer.Changed += (_, e) =>
        {
            if (!e.OldValue && e.NewValue)
                Logger.Debug("Inventory Update Check has been silenced.");
        };
        
        ForceUpdateUponAdding = cfg.BindSyncedEntry(
            new ConfigDefinition(NETWORK, "ForceUpdateUponAdding"),
            true,
            new ConfigDescription(Lang.Get("DESCRIPTION_FORCE_UPDATE_UPON_ADDING"))
        );
        
        ForceUpdateUponRemoving = cfg.BindSyncedEntry(
            new ConfigDefinition(NETWORK, "ForceUpdateUponRemoving"),
            true,
            new ConfigDescription(Lang.Get("DESCRIPTION_FORCE_UPDATE_UPON_REMOVING"))
        );

        #endregion

        #region Unlock

        string UNLOCK = Lang.Get("UNLOCK_SECTION");
        
        ChuteIsUnlock = cfg.BindSyncedEntry(
            new ConfigDefinition(UNLOCK, "ChuteIsUnlock"),
            true,
            new ConfigDescription(Lang.Get("DESCRIPTION_CHUTE_IS_UNLOCK"))
        );
        ChuteInteract.IsUpgrade = ChuteIsUnlock.Value;
        
        ChuteUnlockCost = cfg.BindSyncedEntry(
            new ConfigDefinition(UNLOCK, "ChuteUnlockCost"),
            60,
            new ConfigDescription(Lang.Get("DESCRIPTION_UNLOCK_COST"))
        );

        #endregion

        if (LethalConfigCompatibility.enabled)
            LethalConfigCompatibility.AddConfigs(this);
    }
}