using BepInEx.Configuration;
using CSync.Extensions;
using CSync.Lib;
using LethalLib.Modules;
using ShipInventory.Helpers;
using ShipInventory.Items;
using ShipInventory.Objects;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace ShipInventory;

public class Configuration : SyncedConfig2<Configuration>
{
    public Configuration(ConfigFile cfg) : base(LCMPluginInfo.PLUGIN_GUID)
    {
        LangUsed = cfg.Bind("Language", "Language", Lang.DEFAULT_LANG);
        Lang.LoadLang(LangUsed.Value);

        LoadChuteConfig(cfg);
        LoadInventoryConfig(cfg);
        LoadTerminalConfig(cfg);
        LoadNetworkConfig(cfg);
        LoadUnlockConfig(cfg);

        if (Compatibility.LethalConfig.Enabled)
            Compatibility.LethalConfig.AddConfigs(this);
    }

    #region General

    public readonly ConfigEntry<string> LangUsed;

    public enum PermissionLevel { HOST_ONLY, CLIENTS_ONLY, EVERYONE, NO_ONE }
    public enum SortOrder { NONE, NAME_ASC, NAME_DESC, VALUE_ASC, VALUE_DESC }

    #endregion
    #region Chute

    [SyncedEntryField] public SyncedEntry<PermissionLevel> ChutePermission;
    [SyncedEntryField] public SyncedEntry<bool> RequireInOrbit;
    [SyncedEntryField] public SyncedEntry<float> TimeToStore;
    [SyncedEntryField] public SyncedEntry<float> TimeToRetrieve;
    [SyncedEntryField] public SyncedEntry<int> StopAfter;
    [SyncedEntryField] public SyncedEntry<string> Blacklist;

    private void LoadChuteConfig(ConfigFile cfg)
    {
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
    }

    #endregion
    #region Inventory

    [SyncedEntryField] public SyncedEntry<PermissionLevel> InventoryPermission;
    [SyncedEntryField] public SyncedEntry<bool> ActAsSafe;
    [SyncedEntryField] public SyncedEntry<bool> PersistThroughFire;
    [SyncedEntryField] public SyncedEntry<int> MaxItemCount;
    [SyncedEntryField] public SyncedEntry<float> KeepRate;
    public ConfigEntry<SortOrder> InventorySortOrder;
    [SyncedEntryField] public SyncedEntry<bool> KeepRemoveAll;

    private void LoadInventoryConfig(ConfigFile cfg)
    {
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

        InventorySortOrder = cfg.Bind(
            new ConfigDefinition(INVENTORY, "InventorySortOrder"),
            SortOrder.NAME_ASC,
            new ConfigDescription(Lang.Get("DESCRIPTION_INVENTORY_SORT_ORDER"))
        );

        KeepRemoveAll = cfg.BindSyncedEntry(
            new ConfigDefinition(INVENTORY, "KeepRemoveAll"),
            true,
            new ConfigDescription(Lang.Get("DESCRIPTION_KEEP_REMOVE_ALL"))
        );
    }

    #endregion
    #region Terminal

    public ConfigEntry<string> InventoryCommand;
    public ConfigEntry<bool> YesPlease;
    public ConfigEntry<bool> ShowConfirmation;
    public ConfigEntry<bool> ShowTrademark;

    private void LoadTerminalConfig(ConfigFile cfg)
    {
        string TERMINAL = Lang.Get("TERMINAL_SECTION");

        InventoryCommand = cfg.Bind(
            new ConfigDefinition(TERMINAL, "InventoryCommand"),
            "ship",
            new ConfigDescription(Lang.Get("DESCRIPTION_INVENTORY_COMMAND"))
        );

        YesPlease = cfg.Bind(
            new ConfigDefinition(TERMINAL, "YesPlease"),
            false,
            new ConfigDescription(Lang.Get("DESCRIPTION_YES_PLEASE"))
        );

        ShowConfirmation = cfg.Bind(
            new ConfigDefinition(TERMINAL, "ShowConfirmation"),
            true,
            new ConfigDescription(Lang.Get("DESCRIPTION_SHOW_CONFIRMATION"))
        );

        ShowTrademark = cfg.Bind(
            new ConfigDefinition(TERMINAL, "ShowTrademark"),
            true,
            new ConfigDescription(Lang.Get("DESCRIPTION_SHOW_TRADEMARK"))
        );
    }

    #endregion
    #region Network

    [SyncedEntryField] public SyncedEntry<float> InventoryRefreshRate;
    [SyncedEntryField] public SyncedEntry<bool> InventoryUpdateCheckSilencer;
    [SyncedEntryField] public SyncedEntry<bool> ForceUpdateUponAdding;
    [SyncedEntryField] public SyncedEntry<bool> ForceUpdateUponRemoving;

    private void LoadNetworkConfig(ConfigFile cfg)
    {
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
    }

    #endregion
    #region Unlock

    [SyncedEntryField] public SyncedEntry<bool> ChuteIsUnlock;
    [SyncedEntryField] public SyncedEntry<int> ChuteUnlockCost;
    [SyncedEntryField] public SyncedEntry<int> ChuteUnlockID;
    [SyncedEntryField] public SyncedEntry<string> ChuteUnlockName;

    private void LoadUnlockConfig(ConfigFile cfg)
    {
        string UNLOCK = Lang.Get("UNLOCK_SECTION");

        ChuteIsUnlock = cfg.BindSyncedEntry(
            new ConfigDefinition(UNLOCK, "ChuteIsUnlock"),
            true,
            new ConfigDescription(Lang.Get("DESCRIPTION_CHUTE_IS_UNLOCK"))
        );

        ChuteUnlockID = cfg.BindSyncedEntry(
            new ConfigDefinition(UNLOCK, "ChuteUnlockID"),
            901,
            new ConfigDescription(Lang.Get("DESCRIPTION_UNLOCK_ID"))
        );

        ChuteUnlockCost = cfg.BindSyncedEntry(
            new ConfigDefinition(UNLOCK, "ChuteUnlockCost"),
            60,
            new ConfigDescription(Lang.Get("DESCRIPTION_UNLOCK_COST"))
        );
        ChuteUnlockCost.Changed += (_, e) =>
        {
            if (ChuteInteract.UnlockableItem != null)
                Unlockables.UpdateUnlockablePrice(ChuteInteract.UnlockableItem, e.NewValue);
        };

        ChuteUnlockName = cfg.BindSyncedEntry(
            new ConfigDefinition(UNLOCK, "ChuteUnlockName"),
            "ship inventory",
            new ConfigDescription(Lang.Get("DESCRIPTION_UNLOCK_NAME"))
        );
    }

    #endregion
}
