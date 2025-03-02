using UnityEngine;

namespace ShipInventory.Helpers;

/// <summary>
/// Helper to load a bundle
/// </summary>
internal static class Bundle
{
    private static AssetBundle? loadedBundle;

    /// <summary>
    /// Tries to load the bundle with the given name
    /// </summary>
    /// <returns>Success of the load</returns>
    public static bool LoadBundle(string name)
    {
        var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(name);

        if (stream == null)
        {
            Logger.Error($"No bundle named '{name}'.");
            return false;
        }

        var bundle = AssetBundle.LoadFromStream(stream);

        if (bundle == null)
        {
            Logger.Error($"Failed to load the bundle '{name}'.");
            return false;
        }

        loadedBundle = bundle;
        return true;
    }

    /// <summary>
    /// Tries to load the asset of the given name in the current bundle
    /// </summary>
    /// <returns>Asset loaded or null</returns>
    public static T? LoadAsset<T>(string name) where T : Object
    {
        if (loadedBundle == null)
        {
            Logger.Error($"Tried to load '{name}', but the bundle was not loaded.");
            return null;
        }

        var asset = loadedBundle.LoadAsset<T>(name);

        if (asset == null)
        {
            Logger.Error($"No asset named '{name}' was found.");
        }

        return asset;
    }
}
