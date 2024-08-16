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

        LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(Blacklist.Entry, new TextInputFieldOptions()
        {
            Name = Constants.NAME_BLACKLIST,
            TrimText = true,
            NumberOfLines = 10,
            RequiresRestart = false
        }));

        // Spawn delay
        SpawnDelay = cfg.BindSyncedEntry(
            new ConfigDefinition("General", "ChuteDelay"),
            0.5f,
            new ConfigDescription(Constants.DESCRIPTION_SPAWN_DELAY)
        );
        
        LethalConfigManager.AddConfigItem(new FloatInputFieldConfigItem(SpawnDelay.Entry, new FloatInputFieldOptions()
        {
            Name = Constants.NAME_SPAWN_DELAY,
            Min = 0,
            Max = float.MaxValue,
            RequiresRestart = false
        }));
        
        // Require in orbit
        RequireInOrbit = cfg.BindSyncedEntry(
            new ConfigDefinition("General", "ChuteInOrbit"),
            false,
            new ConfigDescription(Constants.DESCRIPTION_REQUIRE_IN_ORBIT)
        );
        
        LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(RequireInOrbit.Entry, new BoolCheckBoxOptions
        {
            Name = Constants.NAME_REQUIRES_IN_ORBIT,
            RequiresRestart = false
        }));
        
        // Act as Safe
        ActAsSafe = cfg.BindSyncedEntry(
            new ConfigDefinition("General", "ChuteSafe"),
            false,
            new ConfigDescription(Constants.DESCRIPTION_AS_SAFE)
        );
        
        LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(ActAsSafe.Entry, new BoolCheckBoxOptions
        {
            Name = Constants.NAME_AS_SAFE,
            RequiresRestart = false
        }));
        
        // Stop after
        StopAfter = cfg.BindSyncedEntry(
            new ConfigDefinition("General", "ChuteMaxCapacity"),
            30,
            new ConfigDescription(Constants.DESCRIPTION_STOP_AFTER)
        );
        LethalConfigManager.AddConfigItem(new IntSliderConfigItem(StopAfter.Entry, new IntSliderOptions
        {
            Name = Constants.NAME_STOP_AFTER,
            Min = 1,
            Max = 1_969_420,
            RequiresRestart = false
        }));
        
        ConfigManager.Register(this); 
    }
}