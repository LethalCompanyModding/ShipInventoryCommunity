using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using File = System.IO.File;

namespace ShipInventoryUpdated.Helpers;

internal static class Localization
{
    private static LanguagePackage? defaultLanguage;

    /// <summary>
    /// Loads the language package with the given language code
    /// </summary>
    public static LanguagePackage? LoadLanguage(string languageCode)
    {
        string codeBase = Assembly.GetExecutingAssembly().CodeBase;
        UriBuilder uri = new UriBuilder(codeBase);
            
        string? dllPath = Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path));

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
    /// <param name="languagePackage"></param>
    public static void SetAsDefault(LanguagePackage? languagePackage) => defaultLanguage = languagePackage;

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
    
    public class LanguagePackage
    {
        private readonly Dictionary<string, string> loadedData;

        public LanguagePackage(JObject root)
        {
            loadedData = [];

            var stack = new Stack<(JToken, string)>();
            stack.Push((root, ""));

            while (stack.Count > 0)
            {
                var (token, path) = stack.Pop();

                if (token.Type == JTokenType.Object)
                {
                    var obj = (JObject)token;

                    if (!obj.HasValues)
                        continue;

                    foreach (var prop in obj.Properties())
                    {
                        string newPath = string.IsNullOrEmpty(path) ? prop.Name : $"{path}.{prop.Name}";
                        stack.Push((prop.Value, newPath));
                    }
                }
                else if (!string.IsNullOrWhiteSpace(path))
                    loadedData[path] = token.ToString();
            }
        }

        /// <summary>
        /// Fetches the localized value for the given key
        /// </summary>
        public string? Get(string key) => loadedData.ContainsKey(key) ? loadedData[key] : null;
    }
}