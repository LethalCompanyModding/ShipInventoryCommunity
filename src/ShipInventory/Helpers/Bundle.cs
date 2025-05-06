using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ShipInventoryUpdated.Helpers;

/// <summary>
///     Helper to load a bundle
/// </summary>
internal static class Bundle
{
    private static AssetBundle? loadedBundle;

    /// <summary>
    ///     Tries to load the bundle with the given name
    /// </summary>
    /// <returns>Success of the load</returns>
    public static bool LoadBundle(string name)
    {
        try
        {
            string? path = Assembly.GetExecutingAssembly().Location;
            path = Path.GetDirectoryName(path);
            path = Path.Combine(path, name);
            loadedBundle = AssetBundle.LoadFromFile(path);
        }
        catch (Exception e)
        {
            Logger.Error($"Error while loading the bundle '{name}': {e.Message}");
            loadedBundle = null;
        }
       
        if (loadedBundle == null)
        {
            Logger.Error("Failed to load custom assets.");
            return false;
        }

        return true;
    }

    /// <summary>
    ///     Tries to load the asset of the given name in the current bundle
    /// </summary>
    /// <returns>Asset loaded or null</returns>
    public static T? LoadAsset<T>(string name) where T : Object
    {
        if (loadedBundle == null)
        {
            Logger.Error("Tried to load asset from unloaded bundle.");
            return null;
        }

        var asset = loadedBundle.LoadAsset<T>(name);

        if (asset == null)
            Logger.Error($"No asset named '{name}' was found.");

        return asset;
    }
}