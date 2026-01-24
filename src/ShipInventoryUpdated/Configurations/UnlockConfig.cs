using BepInEx.Configuration;
using CSync.Extensions;
using CSync.Lib;
using ShipInventoryUpdated.Helpers;

namespace ShipInventoryUpdated.Configurations;

/// <summary>
/// Class that holds the configurations related to the unlocking of the inventory
/// </summary>
internal class UnlockConfig : SyncedConfig2<UnlockConfig>
{
	private const string GUID_ = MyPluginInfo.PLUGIN_GUID + "." + nameof(UnlockConfig);
	private const string SECTION = "Unlock";

	public readonly ConfigEntry<string> UnlockName;
	[SyncedEntryField] public readonly SyncedEntry<int> UnlockCost;
	[SyncedEntryField] public readonly SyncedEntry<bool> IsChuteUnlocked;

	public UnlockConfig(ConfigFile cfg) : base(GUID_)
	{
		UnlockName = cfg.Bind(
			new ConfigDefinition(SECTION, "ChuteUnlockName"),
			"ship inventory",
			new ConfigDescription(Localization.Get("configuration.unlock.unlockName.description"))
		);

		UnlockCost = cfg.BindSyncedEntry(
			new ConfigDefinition(SECTION, "ChuteUnlockCost"),
			60,
			new ConfigDescription(Localization.Get("configuration.unlock.unlockCost.description"))
		);

		IsChuteUnlocked = cfg.BindSyncedEntry(
			new ConfigDefinition(SECTION, "ChuteIsUnlock"),
			false,
			new ConfigDescription(Localization.Get("configuration.unlock.isUnlockable.description"))
		);

		UnlockName.SettingChanged += (_, _) => Patches.Terminal_Patches.AssignNewCommand(UnlockName.Value);
		
		Patches.Terminal_Patches.AssignNewCost(UnlockCost.Value);
		UnlockCost.Changed += (_, _) => Patches.Terminal_Patches.AssignNewCost(UnlockCost.Value);
		
		ConfigManager.Register(this);
	}
}