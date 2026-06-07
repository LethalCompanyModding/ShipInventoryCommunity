using BepInEx.Configuration;
using ShipInventoryUpdated.Helpers;

namespace ShipInventoryUpdated.Configurations;

/// <summary>
/// Class that holds the configurations related to debug features
/// </summary>
internal class DebugConfig
{
	private const string GUID_ = MyPluginInfo.PLUGIN_GUID + "." + nameof(DebugConfig);
	private const string SECTION = "Debug";

	public DebugConfig(ConfigFile cfg)
	{
	}

	// ReSharper disable once MemberCanBeMadeStatic.Global
	/// <summary>
	/// Reloads the current loaded language
	/// </summary>
	public void ReloadLanguage()
	{
		#if DEBUG
		Localization.ReloadDefault();
		#endif
	}
}