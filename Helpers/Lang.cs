using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace ShipInventory.Helpers;

public class Lang
{
    private static string? PATH;
    private static JObject? LANG;

    public static bool LoadLang(string lang) {
        if (PATH == null)
        {
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            PATH = Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path));
        }
        
        if (PATH == null)
            return false;

        string fileName = $"lang-{lang}.json";
        string file = Path.Combine(PATH, fileName);

        if (!File.Exists(file))
        {
            Logger.Error($"The file '{fileName}' was not found at '{PATH}'.");
            return lang != "en" && LoadLang("en");
        }

        string output = File.ReadAllText(file);
        LANG = JObject.Parse(output);
        
        Logger.Info($"Lang '{lang}' loaded!");
        return true;
    }

    public static string Get(string id) => LANG?.GetValue(id)?.ToString() ?? id;
}