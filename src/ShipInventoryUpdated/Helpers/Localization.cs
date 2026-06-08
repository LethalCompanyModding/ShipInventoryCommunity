using ShipInventoryUpdated.Objects;
using System.Reflection;
using Newtonsoft.Json;
using File = System.IO.File;

namespace ShipInventoryUpdated.Helpers;

/// <summary>
/// Helper to handle the localization of the mod
/// </summary>
internal static class Localization
{
	/// <summary>
	/// Fetches the path to the directory where this assembly is stored
	/// </summary>
	private static string? GetDirectory()
	{
		var codeBase = Assembly.GetExecutingAssembly().CodeBase;
		var localDir = Path.GetDirectoryName(new Uri(codeBase).LocalPath);

		if (localDir == null)
		{
			Logger.Error("Tried to find the location of the assembly, but it was not found.");
			return null;
		}

		return localDir;
	}

	private static LanguagePackage? _currentLanguage;

	/// <summary>
	/// Loads the <see cref="LanguagePackage"/> from the file with the given language code
	/// </summary>
	public static LanguagePackage? LoadFromFile(string languageCode)
	{
		var localDir = GetDirectory();

		if (localDir == null)
			return null;

		var file = Path.Combine(localDir, "Languages", $"{languageCode}.json");

		if (!File.Exists(file))
		{
			Logger.Error($"Tried to load the language package for '{languageCode}', but it was not found.");
			return null;
		}

		var json = File.ReadAllText(file);
		var package = JsonConvert.DeserializeObject<LanguagePackage>(json);

		if (package == null)
		{
			Logger.Error($"Tried to load the language package for '{languageCode}', but it could not be parsed.");
			return null;
		}

		return package;
	}

	/// <summary>
	/// Sets the current language to the given language code
	/// </summary>
	public static void SetLanguage(string languageCode)
	{
		var language = LoadFromFile(languageCode);
		_currentLanguage = language;
	}

	/// <summary>
	/// Gets the localized value at the given key
	/// </summary>
	public static string Get(string key)
	{
		if (_currentLanguage == null || !_currentLanguage.TryGet(key, out var value))
			return key;

		return value;
	}

	/// <summary>
	/// Gets the localized value at the given key with the given parameters parsed in it
	/// </summary>
	public static string GetParsed(string key, Dictionary<string, string> parameters)
	{
		var value = Get(key);

		foreach ((var paramKey, var paramValue) in parameters)
			value = value.Replace($"{{{paramKey}}}", paramValue);

		return value;
	}
}