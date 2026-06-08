using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using ShipInventoryUpdated.Dependencies.Newtonsoft;

namespace ShipInventoryUpdated.Objects;

/// <summary>
/// Represents a language package that contains localized strings
/// </summary>
[JsonConverter(typeof(LanguagePackageConverter))]
internal sealed class LanguagePackage
{
	private readonly Dictionary<string, string> _loadedData = new();

	/// <summary>
	/// Adds the given localized value under the given key
	/// </summary>
	public void Add(string key, string value) => _loadedData.Add(key, value);

	/// <summary>
	/// Attempts to get the localized value under the given key
	/// </summary>
	public bool TryGet(string key, [NotNullWhen(true)] out string? value)
	{
		return _loadedData.TryGetValue(key, out value);
	}
}