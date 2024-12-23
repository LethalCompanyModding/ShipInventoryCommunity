using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace ShipInventory.Helpers;

internal static class Lang
{
    public const string DEFAULT_LANG = "en";
    
    /// <summary>
    /// Loads the given language
    /// </summary>
    public static bool LoadLang(string lang)
    {
        string codeBase = Assembly.GetExecutingAssembly().CodeBase;
        UriBuilder uri = new UriBuilder(codeBase);
            
        string? dllPath = Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path));
            
        if (dllPath == null)
            return false;

        string file = Path.Combine(dllPath, "langs", $"{lang}.json");

        if (!File.Exists(file))
        {
            Logger.Error($"Could not find the file '{file}'.");
            return lang != DEFAULT_LANG && LoadLang(DEFAULT_LANG);
        }

        var json = JObject.Parse(File.ReadAllText(file));
        
        tokens.Clear();
        foreach (var (key, value) in json)
            tokens[key] = value?.ToString() ?? key;

        Logger.Info($"Language '{lang}' loaded!");
        return true;
    }

    #region Strings

    private static readonly Dictionary<string, string> tokens = [];

    public static string Get(string id) => tokens.GetValueOrDefault(id, id);

    #endregion
}