using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ShipInventoryUpdated.Objects;

namespace ShipInventoryUpdated.Dependencies.Newtonsoft;

/// <summary>
/// Converts a JSON content to a <see cref="LanguagePackage"/>
/// </summary>
internal class LanguagePackageConverter : JsonConverter<LanguagePackage>
{
	/// <inheritdoc />
	public override bool CanWrite => false;

	/// <inheritdoc />
	public override void WriteJson(JsonWriter writer, LanguagePackage? value, JsonSerializer serializer)
	{
		throw new NotSupportedException();
	}

	/// <inheritdoc />
	public override LanguagePackage ReadJson(
		JsonReader       reader,
		Type             objectType,
		LanguagePackage? existingValue,
		bool             hasExistingValue,
		JsonSerializer   serializer
	)
	{
		var package = new LanguagePackage();

		var root = JObject.Load(reader);

		var stack = new Stack<(JToken, string)>();
		stack.Push((root, ""));

		while (stack.Count > 0)
		{
			(var token, var path) = stack.Pop();

			if (token is JObject jObject)
			{
				foreach (var prop in jObject.Properties())
				{
					string newPath;

					if (string.IsNullOrWhiteSpace(path))
						newPath = prop.Name;
					else
						newPath = path + "." + prop.Name;

					stack.Push((prop.Value, newPath));
				}

				continue;
			}

			var value = token.Value<string>();

			if (value == null)
				continue;

			package.Add(path, value);
		}

		return package;
	}
}