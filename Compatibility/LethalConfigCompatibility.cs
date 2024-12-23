using System.Runtime.CompilerServices;
using CSync.Lib;
using LethalConfig;
using LethalConfig.ConfigItems;
using LethalConfig.ConfigItems.Options;
using ShipInventory.Helpers;
using UnityEngine;

namespace ShipInventory.Compatibility;

public static class LethalConfigCompatibility
{
    public const string LETHAL_CONFIG = "ainavt.lc.lethalconfig";
    private static bool? _enabled;

    public static bool enabled
    {
        get
        {
            _enabled ??= BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(LETHAL_CONFIG);
            return _enabled.Value;
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    public static void AddConfigs(Config config)
    {
        LethalConfigManager.SetModIcon(Bundle.LoadAsset<Sprite>(Constants.MOD_ICON));
        LethalConfigManager.SetModDescription(
            "Adds an inventory to the ship, allowing it to store items and retrieve them.");

        #region Chute

        LethalConfigManager.AddConfigItem(new EnumDropDownConfigItem<Config.PermissionLevel>(
            config.ChutePermission.Entry, new EnumDropDownOptions
            {
                Name = Lang.Get("NAME_CHUTE_PERMISSION"),
            }));

        LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(config.RequireInOrbit.Entry,
            new BoolCheckBoxOptions
            {
                Name = Lang.Get("NAME_REQUIRES_IN_ORBIT"),
            }));

        LethalConfigManager.AddConfigItem(new FloatInputFieldConfigItem(config.TimeToStore.Entry,
            new FloatInputFieldOptions
            {
                Name = Lang.Get("NAME_TIME_TO_STORE"),
                Min = 0,
                Max = float.MaxValue,
                RequiresRestart = true
            }));

        LethalConfigManager.AddConfigItem(new FloatInputFieldConfigItem(config.TimeToRetrieve.Entry,
            new FloatInputFieldOptions
            {
                Name = Lang.Get("NAME_TIME_TO_RETRIEVE"),
                Min = 0,
                Max = float.MaxValue,
            }));

        LethalConfigManager.AddConfigItem(new IntSliderConfigItem(config.StopAfter.Entry, new IntSliderOptions
        {
            Name = Lang.Get("NAME_STOP_AFTER"),
            Min = 1,
            Max = 1_000,
        }));

        LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(config.Blacklist.Entry, new TextInputFieldOptions
        {
            Name = Lang.Get("NAME_BLACKLIST"),
            TrimText = true,
            NumberOfLines = 10,
        }));

        #endregion

        #region Inventory

        LethalConfigManager.AddConfigItem(new EnumDropDownConfigItem<Config.PermissionLevel>(
            config.InventoryPermission.Entry, new EnumDropDownOptions
            {
                Name = Lang.Get("NAME_INVENTORY_PERMISSION"),
            }));

        LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(config.ActAsSafe.Entry, new BoolCheckBoxOptions
        {
            Name = Lang.Get("NAME_AS_SAFE"),
        }));

        LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(config.PersistThroughFire.Entry,
            new BoolCheckBoxOptions
            {
                Name = Lang.Get("NAME_PERSIST_THROUGH_FIRE"),
            }));

        LethalConfigManager.AddConfigItem(new IntSliderConfigItem(config.MaxItemCount.Entry, new IntSliderOptions
        {
            Name = Lang.Get("NAME_MAX_ITEM_COUNT"),
            Min = 1,
            Max = 10_000,
        }));

        #endregion

        #region Terminal

        LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(config.InventoryCommand.Entry,
            new TextInputFieldOptions
            {
                Name = Lang.Get("NAME_INVENTORY_COMMAND"),
                TrimText = true,
                NumberOfLines = 1,
                RequiresRestart = true,
            }));

        LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(config.YesPlease.Entry, new BoolCheckBoxOptions
        {
            Name = Lang.Get("NAME_YES_PLEASE"),
        }));

        LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(config.ShowConfirmation.Entry,
            new BoolCheckBoxOptions
            {
                Name = Lang.Get("NAME_SHOW_CONFIRMATION"),
            }));

        LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(config.ShowTrademark.Entry, new BoolCheckBoxOptions
        {
            Name = Lang.Get("NAME_SHOW_TRADEMARK"),
        }));

        #endregion

        LethalConfigManager.SkipAutoGenFor(config.LangUsed);

        ConfigManager.Register(config);
    }
}