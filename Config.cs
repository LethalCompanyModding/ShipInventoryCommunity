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

    public readonly ConfigEntry<string> LangUsed;
    
    [SyncedEntryField] public readonly SyncedEntry<string> Blacklist;
    [SyncedEntryField] public readonly SyncedEntry<float> SpawnDelay;
    [SyncedEntryField] public readonly SyncedEntry<bool> RequireInOrbit;
    [SyncedEntryField] public readonly SyncedEntry<int> StopAfter;
    
    [SyncedEntryField] public readonly SyncedEntry<bool> ActAsSafe;
    [SyncedEntryField] public readonly SyncedEntry<int> MaxItemCount;
    [SyncedEntryField] public readonly SyncedEntry<bool> PersistThroughFire;
    
    [SyncedEntryField] public readonly SyncedEntry<bool> ShowConfirmation;
    [SyncedEntryField] public readonly SyncedEntry<bool> NoSteamID;

    [SyncedEntryField] public readonly SyncedEntry<OverrideMode> OverrideTrigger;
    [SyncedEntryField] public readonly SyncedEntry<bool> LetAsItem;
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
        
        NoSteamID = cfg.BindSyncedEntry(
            new ConfigDefinition(TERMINAL, "NoSteamID"),
            false,
            new ConfigDescription(Lang.Get("DESCRIPTION_NO_STEAM_ID"))
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
        
        LetAsItem = cfg.BindSyncedEntry(
            new ConfigDefinition(DEBUG, "LetAsItem"),
            false,
            new ConfigDescription(string.Format(
                Lang.Get("DESCRIPTION_LET_AS_ITEM"),
                nameof(GrabbableObject)
            ))
        );

        #endregion

        RegisterLethalConfig();
    }

    private void RegisterLethalConfig()
    {
        #region Chute
        
        LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(Blacklist.Entry, new TextInputFieldOptions {
            Name = Lang.Get("NAME_BLACKLIST"),
            TrimText = true,
            NumberOfLines = 10,
            RequiresRestart = false,
        }));
        
        LethalConfigManager.AddConfigItem(new FloatInputFieldConfigItem(SpawnDelay.Entry, new FloatInputFieldOptions {
            Name = Lang.Get("NAME_SPAWN_DELAY"),
            Min = 0,
            Max = float.MaxValue,
            RequiresRestart = false
        }));
        
        LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(RequireInOrbit.Entry, new BoolCheckBoxOptions {
            Name = Lang.Get("NAME_REQUIRES_IN_ORBIT"),
            RequiresRestart = false
        }));

        LethalConfigManager.AddConfigItem(new IntSliderConfigItem(StopAfter.Entry, new IntSliderOptions {
            Name = Lang.Get("NAME_STOP_AFTER"),
            Min = 1,
            Max = 1_969_420,
            RequiresRestart = false
        }));
        
        #endregion

        #region Inventory

        LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(ActAsSafe.Entry, new BoolCheckBoxOptions {
            Name = Lang.Get("NAME_AS_SAFE"),
            RequiresRestart = false
        }));
        
        LethalConfigManager.AddConfigItem(new IntSliderConfigItem(MaxItemCount.Entry, new IntSliderOptions {
            Name = Lang.Get("NAME_MAX_ITEM_COUNT"),
            Min = 1,
            Max = 1_969_420,
            RequiresRestart = false
        }));
        
        LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(PersistThroughFire.Entry, new BoolCheckBoxOptions {
            Name = Lang.Get("NAME_PERSIST_THROUGH_FIRE"),
            RequiresRestart = false
        }));

        #endregion
        
        #region Terminal

        LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(ShowConfirmation.Entry, new BoolCheckBoxOptions {
            Name = Lang.Get("NAME_SHOW_CONFIRMATION"),
            RequiresRestart = false
        }));
        
        LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(NoSteamID.Entry, new BoolCheckBoxOptions {
            Name = Lang.Get("NAME_NO_STEAM_ID"),
            RequiresRestart = false
        }));

        #endregion

        #region Debug
        
        LethalConfigManager.AddConfigItem(new EnumDropDownConfigItem<OverrideMode>(OverrideTrigger.Entry, new EnumDropDownOptions {
            Name = Lang.Get("NAME_OVERRIDE_TRIGGER"),
            RequiresRestart = false
        }));
        
        LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(LetAsItem.Entry, new BoolCheckBoxOptions {
            Name = Lang.Get("NAME_LET_AS_ITEM"),
            RequiresRestart = true
        }));

        #endregion
        
        LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(LangUsed, new TextInputFieldOptions {
            Description = "Language to use for the texts.\n\nThe translation has to be found next to the DLL.",
            RequiresRestart = true
        }));
        
        ConfigManager.Register(this); 
    }
}