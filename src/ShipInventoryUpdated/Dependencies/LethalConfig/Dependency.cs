using System.Runtime.CompilerServices;
using LethalConfig;
using LethalConfig.ConfigItems;
using LethalConfig.ConfigItems.Options;
using ShipInventoryUpdated.Configurations;
using ShipInventoryUpdated.Helpers;
using UnityEngine;
using Logger = ShipInventoryUpdated.Helpers.Logger;

namespace ShipInventoryUpdated.Dependencies.LethalConfig;

// https://github.com/AinaVT/LethalConfig
internal static class Dependency
{
	public static bool Enabled => Helpers.Dependencies.IsEnabled(PluginInfo.Guid);

	[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
	public static void ApplyConfiguration(Configuration? configuration)
	{
		ApplyInformation();

		if (configuration == null)
		{
			Logger.Info($"Tried to load the configurations into '{nameof(LethalConfig)}', but none were given.");
			return;
		}

		LethalConfigManager.SkipAutoGen();
		ApplyUnlockConfiguration(configuration.Unlock);
		ApplyChuteConfiguration(configuration.Chute);
		ApplyTerminalConfiguration(configuration.Terminal);
		ApplyInventoryConfiguration(configuration.Inventory);

		#if DEBUG
		LethalConfigManager.AddConfigItem(new GenericButtonConfigItem(
			"DEBUG",
			"Reload Localization",
			"Reloads the current localization",
			"Reload",
			Localization.ReloadDefault
		));
		#endif
	}

	/// <summary>
	/// Applies the general information for the mod
	/// </summary>
	private static void ApplyInformation()
	{
		var modIcon = Bundle.LoadAsset<Texture2D>("mod-icon");

		if (modIcon != null)
		{
			var sprite = Sprite.Create(
				modIcon,
				new Rect(0f,
					0f,
					modIcon.width,
					modIcon.height),
				new Vector2(0.5f, 0.5f)
			);
			LethalConfigManager.SetModIcon(sprite);
		}

		LethalConfigManager.SetModDescription(Localization.Get("mod.description"));
	}

	/// <summary>
	/// Applies all the configurations for <see cref="ChuteConfig"/>
	/// </summary>
	private static void ApplyChuteConfiguration(ChuteConfig config)
	{
		LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(
			config.Blacklist,
			new TextInputFieldOptions
			{
				Name = Localization.Get("configuration.chute.blacklist.name"),
				NumberOfLines = 5,
				TrimText = true,
				RequiresRestart = false
			}
		));
		
		LethalConfigManager.AddConfigItem(new FloatInputFieldConfigItem(
			config.StoreSpeed,
			new FloatInputFieldOptions
			{
				Name = Localization.Get("configuration.chute.storeSpeed.name"),
				Min = 0,
				Max = float.MaxValue,
				RequiresRestart = true
			}
		));
	}

	/// <summary>
	/// Applies all the configurations for <see cref="InventoryConfig"/>
	/// </summary>
	private static void ApplyInventoryConfiguration(InventoryConfig config)
	{
		LethalConfigManager.AddConfigItem(new IntInputFieldConfigItem(
			config.MaxItemCount,
			new IntInputFieldOptions
			{
				Name = Localization.Get("configuration.inventory.maxItemCount.name"),
				Min = 1,
				Max = int.MaxValue,
				RequiresRestart = false
			}
		));
		
		LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(
			config.ClearOnWipe,
			new BoolCheckBoxOptions
			{
				Name = Localization.Get("configuration.inventory.clearOnWipe.name"),
				RequiresRestart = false
			}
		));
		
		LethalConfigManager.AddConfigItem(new FloatInputFieldConfigItem(
			config.RetrieveSpeed,
			new FloatInputFieldOptions
			{
				Name = Localization.Get("configuration.inventory.retrieveSpeed.name"),
				Min = 0,
				Max = float.MaxValue,
				RequiresRestart = true
			}
		));
	}

	/// <summary>
	/// Applies all the configurations for <see cref="UnlockConfig"/>
	/// </summary>
	private static void ApplyUnlockConfiguration(UnlockConfig config)
	{
		LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(
			config.UnlockName,
			new TextInputFieldOptions
			{
				Name = Localization.Get("configuration.unlock.unlockName.name"),
				NumberOfLines = 1,
				CharacterLimit = 64,
				TrimText = true,
				RequiresRestart = false
			}
		));

		LethalConfigManager.AddConfigItem(new IntInputFieldConfigItem(
			config.UnlockCost,
			new IntInputFieldOptions
			{
				Name = Localization.Get("configuration.unlock.unlockCost.name"),
				Min = 0,
				Max = 999_999_999,
				RequiresRestart = false
			}
		));

		LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(
			config.IsChuteUnlocked,
			new BoolCheckBoxOptions
			{
				Name = Localization.Get("configuration.unlock.isUnlockable.name"),
				RequiresRestart = true
			}
		));
	}

	/// <summary>
	/// Applies all the configurations for <see cref="TerminalConfig"/>
	/// </summary>
	private static void ApplyTerminalConfiguration(TerminalConfig config)
	{
		LethalConfigManager.AddConfigItem(new TextInputFieldConfigItem(
			config.InventoryCommand,
			new TextInputFieldOptions
			{
				Name = Localization.Get("configuration.terminal.command.name"),
				NumberOfLines = 1,
				CharacterLimit = 64,
				TrimText = true,
				RequiresRestart = true
			}
		));

		LethalConfigManager.AddConfigItem(new EnumDropDownConfigItem<TerminalConfig.SortOrder>(
			config.InventorySortOrder,
			new EnumDropDownOptions
			{
				Name = Localization.Get("configuration.terminal.inventorySortOrder.name"),
				RequiresRestart = false
			}
		));

		LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(
			config.AutomaticPositiveAnswer,
			new BoolCheckBoxOptions
			{
				Name = Localization.Get("configuration.terminal.automaticPositive.name",
					new Dictionary<string, string>
					{
						["positiveAnswer"] = Localization.Get("application.answers.positive")
					}),
				RequiresRestart = false
			}
		));

		LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(
			config.ShowConfirmation,
			new BoolCheckBoxOptions
			{
				Name = Localization.Get("configuration.terminal.showConfirmation.name"),
				RequiresRestart = false
			}
		));

		LethalConfigManager.AddConfigItem(new BoolCheckBoxConfigItem(
			config.ShowTrademark,
			new BoolCheckBoxOptions
			{
				Name = Localization.Get("configuration.terminal.showTrademark.name"),
				RequiresRestart = false
			}
		));
	}
}