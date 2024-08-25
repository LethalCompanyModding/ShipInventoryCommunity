using BepInEx.Configuration;
using CSync.Extensions;
using CSync.Lib;
using LethalConfig;
using LethalConfig.ConfigItems;
using LethalConfig.ConfigItems.Options;
using ShipInventory.Helpers;

namespace ShipInventory;

public class Config : SyncedConfig2<Config>
{
    #region Entries

    [SyncedEntryField] public readonly SyncedEntry<string> Blacklist;
    [SyncedEntryField] public readonly SyncedEntry<float> SpawnDelay;
    [SyncedEntryField] public readonly SyncedEntry<bool> RequireInOrbit;
    [SyncedEntryField] public readonly SyncedEntry<int> StopAfter;
    
    [SyncedEntryField] public readonly SyncedEntry<bool> ActAsSafe;
    [SyncedEntryField] public readonly SyncedEntry<int> MaxItemCount;
    
    [SyncedEntryField] public readonly SyncedEntry<bool> ShowConfirmation;

    [SyncedEntryField] public readonly SyncedEntry<float> PanelIdleDelay;
    [SyncedEntryField] public readonly SyncedEntry<bool> ShowShitpost;
    [SyncedEntryField] public readonly SyncedEntry<float> PanelShitpostDelay;
    [SyncedEntryField] public readonly SyncedEntry<float> ShitpostChance;

    #endregion

    public Config(ConfigFile cfg) : base(MyPluginInfo.PLUGIN_GUID)
    {
        #region Chute
        
        Blacklist = cfg.BindSyncedEntry(
            new ConfigDefinition(Constants.CHUTE_SECTION, "ChuteBlacklist"),
            "",
            new ConfigDescription(Constants.DESCRIPTION_BLACKLIST)
        );
        Blacklist.Changed += (_, e) => ItemManager.UpdateBlacklist(e.NewValue);
        ItemManager.UpdateBlacklist(Blacklist.Value);

        SpawnDelay = cfg.BindSyncedEntry(
            new ConfigDefinition(Constants.CHUTE_SECTION, "ChuteDelay"),
            0.5f,
            new ConfigDescription(Constants.DESCRIPTION_SPAWN_DELAY)
        );
        
        RequireInOrbit = cfg.BindSyncedEntry(
            new ConfigDefinition(Constants.CHUTE_SECTION, "ChuteInOrbit"),
            false,
            new ConfigDescription(Constants.DESCRIPTION_REQUIRE_IN_ORBIT)
        );

        StopAfter = cfg.BindSyncedEntry(
            new ConfigDefinition(Constants.CHUTE_SECTION, "ChuteMaxCapacity"),
            30,
            new ConfigDescription(Constants.DESCRIPTION_STOP_AFTER)
        );

        #endregion

        #region Inventory

        ActAsSafe = cfg.BindSyncedEntry(
            new ConfigDefinition(Constants.INVENTORY_SECTION, "ChuteSafe"),
            false,
            new ConfigDescription(Constants.DESCRIPTION_AS_SAFE)
        );
        
        MaxItemCount = cfg.BindSyncedEntry(
            new ConfigDefinition(Constants.INVENTORY_SECTION, "MaxItemCount"),
            1_969_420,
            new ConfigDescription(Constants.DESCRIPTION_MAX_ITEM_COUNT)
        );
        
        #endregion
        
        #region Terminal

        ShowConfirmation = cfg.BindSyncedEntry(
            new ConfigDefinition(Constants.TERMINAL_SECTION, "ShowConfirmation"),
            true,
            new ConfigDescription(Constants.DESCRIPTION_SHOW_CONFIRMATION)
        );

        #endregion

        #region Panel

        PanelIdleDelay = cfg.BindSyncedEntry(
            new ConfigDefinition(Constants.PANEL_SECTION, "PanelIdleDelay"),
            15f,
            new ConfigDescription(Constants.DESCRIPTION_PANEL_IDLE_DELAY)
        );
        
        ShowShitpost = cfg.BindSyncedEntry(
            new ConfigDefinition(Constants.PANEL_SECTION, "ShowShitpost"),
            true,
            new ConfigDescription(Constants.DESCRIPTION_SHOW_SHITPOST)
        );
        
        PanelShitpostDelay = cfg.BindSyncedEntry(
            new ConfigDefinition(Constants.PANEL_SECTION, "PanelShitpostDelay"),
            15f,
            new ConfigDescription(Constants.DESCRIPTION_PANEL_SHITPOST_DELAY)
        );
        
        ShitpostChance = cfg.BindSyncedEntry(
            new ConfigDefinition(Constants.PANEL_SECTION, "ShitpostChance"),
            5f,
            new ConfigDescription(Constants.DESCRIPTION_SHITPOST_CHANCE)
        );
        
        #endregion

        RegisterLethalConfig();
    }

    private void RegisterLethalConfig()
    {
        #region Chute
        
        LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(Blacklist.Entry, new TextInputFieldOptions {
            Name = Constants.NAME_BLACKLIST,
            TrimText = true,
            NumberOfLines = 10,
            RequiresRestart = false,
        }));
        
        LethalConfigManager.AddConfigItem(new FloatInputFieldConfigItem(SpawnDelay.Entry, new FloatInputFieldOptions {
            Name = Constants.NAME_SPAWN_DELAY,
            Min = 0,
            Max = float.MaxValue,
            RequiresRestart = false
        }));
        
        LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(RequireInOrbit.Entry, new BoolCheckBoxOptions {
            Name = Constants.NAME_REQUIRES_IN_ORBIT,
            RequiresRestart = false
        }));

        LethalConfigManager.AddConfigItem(new IntSliderConfigItem(StopAfter.Entry, new IntSliderOptions {
            Name = Constants.NAME_STOP_AFTER,
            Min = 1,
            Max = 1_969_420,
            RequiresRestart = false
        }));

        #endregion

        #region Inventory

        LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(ActAsSafe.Entry, new BoolCheckBoxOptions {
            Name = Constants.NAME_AS_SAFE,
            RequiresRestart = false
        }));
        
        LethalConfigManager.AddConfigItem(new IntSliderConfigItem(MaxItemCount.Entry, new IntSliderOptions {
            Name = Constants.NAME_MAX_ITEM_COUNT,
            Min = 1,
            Max = 1_969_420,
            RequiresRestart = false
        }));

        #endregion
        
        #region Terminal

        LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(ShowConfirmation.Entry, new BoolCheckBoxOptions {
            Name = Constants.NAME_SHOW_CONFIRMATION,
            RequiresRestart = false
        }));

        #endregion

        #region Panel

        LethalConfigManager.AddConfigItem(new FloatSliderConfigItem(PanelIdleDelay.Entry, new FloatSliderOptions
        {
            Name = Constants.NAME_PANEL_IDLE_DELAY,
            Min = 0.1f,
            Max = 300,
            RequiresRestart = false
        }));
        
        LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(ShowShitpost.Entry, new BoolCheckBoxOptions
        {
            Name = Constants.NAME_SHOW_SHITPOST,
            RequiresRestart = false
        }));
        
        LethalConfigManager.AddConfigItem(new FloatSliderConfigItem(PanelShitpostDelay.Entry, new FloatSliderOptions
        {
            Name = Constants.NAME_PANEL_SHITPOST_DELAY,
            Min = 0.1f,
            Max = 300,
            RequiresRestart = false
        }));
        
        LethalConfigManager.AddConfigItem(new FloatSliderConfigItem(ShitpostChance.Entry, new FloatSliderOptions
        {
            Name = Constants.NAME_SHITPOST_CHANCE,
            Min = 0f,
            Max = 100f,
            RequiresRestart = false
        }));

        #endregion
        
        ConfigManager.Register(this); 
    }
}