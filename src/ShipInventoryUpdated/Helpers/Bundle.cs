using System.Reflection;
using MonoMod.RuntimeDetour;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ShipInventoryUpdated.Helpers;

/// <summary>
/// Helper to load a bundle
/// </summary>
internal static class Bundle
{
	private static AssetBundle? _loadedBundle;
	private static AssetBundle? _loadedItemBundle;

	/// <summary>
	/// Tries to load the bundle with the given name
	/// </summary>
	/// <returns>Success of the load</returns>
	public static bool LoadBundle(string name)
	{
		var path = Assembly.GetExecutingAssembly().Location;
		path = Path.GetDirectoryName(path) ?? "";
		path = Path.Combine(path, name);

		_loadedBundle = AssetBundle.LoadFromFile(path);

		if (_loadedBundle == null)
		{
			Logger.Error($"Failed to load the bundle '{name}'.");
			return false;
		}

		return true;
	}

	/// <summary>
	/// Tries to load the asset of the given name in the current bundle
	/// </summary>
	/// <returns>Asset loaded or null</returns>
	public static T? LoadAsset<T>(string name) where T : Object
	{
		if (_loadedBundle == null)
		{
			Logger.Error($"Tried to load '{name}', but the bundle was not loaded.");
			return null;
		}

		var asset = _loadedBundle.LoadAsset<T>(name);

		if (asset == null)
			Logger.Error($"No asset named '{name}' was found.");

		return asset;
	}

	internal static bool LoadBundleItems(string name)
	{
		var path = Assembly.GetExecutingAssembly().Location;
		path = Path.GetDirectoryName(path) ?? "";
		path = Path.Combine(path, name);

		_loadedItemBundle = AssetBundle.LoadFromFile(path);

		if (_loadedItemBundle == null)
		{
			Logger.Error($"Failed to load the bundle '{name}'.");
			return false;
		}

		return true;
	}

	public static T? LoadItemAsset<T>(string name) where T : Object
	{
		if (_loadedItemBundle == null)
		{
			Logger.Error($"Tried to load '{name}', but the bundle was not loaded.");
			return null;
		}

		var asset = _loadedItemBundle.LoadAsset<T>(name);

		if (asset == null)
			Logger.Error($"No asset named '{name}' was found.");

		return asset;
	}
}