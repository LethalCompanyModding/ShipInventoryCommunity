using ShipInventoryUpdated.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using File = System.IO.File;

namespace ShipInventoryUpdated.Helpers;

/// <summary>
/// Helper to handle the localization of the mod
/// </summary>
internal static class Localization
{
    private static LanguagePackage? defaultLanguage;

    /// <summary>
    /// Loads the language package with the given language code
    /// </summary>
    public static LanguagePackage? LoadLanguage(string languageCode)
    {
        string codeBase = Assembly.GetExecutingAssembly().CodeBase;
        string? dllPath = Path.GetDirectoryName(new Uri(codeBase).LocalPath);

        if (dllPath == null)
        {
            Logger.Error("Tried to find the location of the assembly, but it was not found.");
            return null;
        }

        string file = Path.Combine(dllPath, $"{languageCode}.json");

        if (!File.Exists(file))
        {
            Logger.Error($"Tried to load the language package for '{languageCode}', but it was not found.");
            return null;
        }

        string json = File.ReadAllText(file);
        var rawData = JsonConvert.DeserializeObject<JObject>(json);

        if (rawData == null)
        {
            Logger.Error($"Tried to load the language package for '{languageCode}', but it could not be parsed.");
            return null;
        }

        return new LanguagePackage(rawData);
    }

    /// <summary>
    /// Sets the given language package as the default language
    /// </summary>
    public static void SetAsDefault(LanguagePackage? languagePackage) => defaultLanguage = languagePackage;

    /// <summary>
    /// Fetches the localized value at the given key, parsing the parameters in it
    /// </summary>
    public static string Get(string key, Dictionary<string, string>? parameters = null)
    {
        var value = defaultLanguage?.Get(key);

        if (value == null)
            return key;

        if (parameters != null)
        {
            foreach (var (paramKey, paramValue) in parameters)
                value = value.Replace($"{{{paramKey}}}", paramValue);
        }

        return value;
    }
}