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
    [SyncedEntryField] public readonly SyncedEntry<bool> ActAsSafe;
    [SyncedEntryField] public readonly SyncedEntry<int> StopAfter;
    [SyncedEntryField] public readonly SyncedEntry<bool> ShowConfirmation;
    [SyncedEntryField] public readonly SyncedEntry<bool> ShowNews;

    #endregion

    public Config(ConfigFile cfg) : base(MyPluginInfo.PLUGIN_GUID)
    {
        // Blacklist
        Blacklist = cfg.BindSyncedEntry(
            new ConfigDefinition("General", "ChuteBlacklist"),
            "",
            new ConfigDescription(Constants.DESCRIPTION_BLACKLIST)
        );
        Blacklist.Changed += (_, e) => ItemManager.UpdateBlacklist(e.NewValue);
        ItemManager.UpdateBlacklist(Blacklist.Value);

        SpawnDelay = cfg.BindSyncedEntry(
            new ConfigDefinition("General", "ChuteDelay"),
            0.5f,
            new ConfigDescription(Constants.DESCRIPTION_SPAWN_DELAY)
        );
        
        RequireInOrbit = cfg.BindSyncedEntry(
            new ConfigDefinition("General", "ChuteInOrbit"),
            false,
            new ConfigDescription(Constants.DESCRIPTION_REQUIRE_IN_ORBIT)
        );
        
        ActAsSafe = cfg.BindSyncedEntry(
            new ConfigDefinition("General", "ChuteSafe"),
            false,
            new ConfigDescription(Constants.DESCRIPTION_AS_SAFE)
        );
        
        StopAfter = cfg.BindSyncedEntry(
            new ConfigDefinition("General", "ChuteMaxCapacity"),
            30,
            new ConfigDescription(Constants.DESCRIPTION_STOP_AFTER)
        );
        
        ShowConfirmation = cfg.BindSyncedEntry(
            new ConfigDefinition("General", "ShowConfirmation"),
            false,
            new ConfigDescription(Constants.DESCRIPTION_SHOW_CONFIRMATION)
        );

        ShowNews = cfg.BindSyncedEntry(
            new ConfigDefinition("General", "ShowNews"),
            false,
            new ConfigDescription(Constants.DESCRIPTION_SHOW_NEWS)
        );
    }

    private void RegisterLethalConfig()
    {
        LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(Blacklist.Entry, new TextInputFieldOptions {
            Name = Constants.NAME_BLACKLIST,
            TrimText = true,
            NumberOfLines = 10,
            RequiresRestart = false
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
        
        LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(ActAsSafe.Entry, new BoolCheckBoxOptions {
            Name = Constants.NAME_AS_SAFE,
            RequiresRestart = false
        }));
        
        LethalConfigManager.AddConfigItem(new IntSliderConfigItem(StopAfter.Entry, new IntSliderOptions {
            Name = Constants.NAME_STOP_AFTER,
            Min = 1,
            Max = 1_969_420,
            RequiresRestart = false
        }));
        
        LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(ShowConfirmation.Entry, new BoolCheckBoxOptions {
            Name = Constants.NAME_SHOW_CONFIRMATION,
            RequiresRestart = false
        }));
        
        LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(ShowNews.Entry, new BoolCheckBoxOptions {
            Name = Constants.NAME_SHOW_NEWS,
            RequiresRestart = false
        }));
        
        ConfigManager.Register(this); 
    }
}