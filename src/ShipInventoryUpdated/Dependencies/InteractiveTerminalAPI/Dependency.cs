using System.Runtime.CompilerServices;
using InteractiveTerminalAPI.UI;
using ShipInventoryUpdated.Configurations;
using ShipInventoryUpdated.Helpers;

namespace ShipInventoryUpdated.Dependencies.InteractiveTerminalAPI;

// https://github.com/WhiteSpike/InteractiveTerminalAPI
internal static class Dependency
{
	public static bool Enabled => Helpers.Dependencies.IsEnabled("WhiteSpike.InteractiveTerminalAPI");

	[MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
	public static void RegisterApplication()
	{
		var entry = Configuration.Instance?.Terminal.InventoryCommand;

		if (entry == null)
		{
			Logger.Error($"Tried to access the configuration for '{nameof(TerminalConfig.InventoryCommand)}', but it was not defined.");
			return;
		}

		InteractiveTerminalManager.RegisterApplication<ShipApplication>(entry.Value, true);
	}
}