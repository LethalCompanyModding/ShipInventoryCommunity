using Newtonsoft.Json.Linq;

namespace ShipInventoryUpdated.Objects;

/// <summary>
/// Represents a language package that contains localized strings
/// </summary>
internal sealed class LanguagePackage
{
	/// <summary>
	/// Language code of this package
	/// </summary>
	public readonly string Language;

	private readonly Dictionary<string, string> _loadedData;

	internal LanguagePackage(string language, JObject node)
	{
		Language = language;
		_loadedData = [];
		ParseTree(node);
	}

	/// <summary>
	/// Compiles the localized strings into their IDs from the given root
	/// </summary>
	private void ParseTree(JObject root)
	{
		var stack = new Stack<(JToken, string)>();
		stack.Push((root, ""));

		while (stack.Count > 0)
		{
			(var token, var path) = stack.Pop();

			if (token.Type == JTokenType.Object)
			{
				var obj = (JObject)token;

				if (!obj.HasValues)
					continue;

				foreach (var prop in obj.Properties())
				{
					var newPath = string.IsNullOrEmpty(path) ? prop.Name : $"{path}.{prop.Name}";
					stack.Push((prop.Value, newPath));
				}
			} else if (!string.IsNullOrWhiteSpace(path))
				_loadedData[path] = token.ToString();
		}
	}

	/// <summary>
	/// Fetches the localized value for the given key
	/// </summary>
	public string? Get(string key) => _loadedData.ContainsKey(key) ? _loadedData[key] : null;
}