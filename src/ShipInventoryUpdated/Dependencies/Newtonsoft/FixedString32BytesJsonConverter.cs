using Newtonsoft.Json;
using Unity.Collections;

namespace ShipInventoryUpdated.Dependencies.Newtonsoft;

internal class FixedString32BytesJsonConverter : JsonConverter<FixedString32Bytes>
{
	/// <inheritdoc/>
	public override void WriteJson(JsonWriter writer, FixedString32Bytes value, JsonSerializer serializer)
	{
		writer.WriteValue(value.ToString());
	}

	/// <inheritdoc/>
	public override FixedString32Bytes ReadJson(
		JsonReader         reader,
		Type               objectType,
		FixedString32Bytes existingValue,
		bool               hasExistingValue,
		JsonSerializer     serializer
	)
	{
		var value = reader.Value as string;
		return new FixedString32Bytes(value ?? null);
	}
}