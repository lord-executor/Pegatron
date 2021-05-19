namespace Pegatron.UnitTests.Json
{
	public class JsonProperty : JsonValue
	{
		public string Name { get; }
		public JsonValue Value { get; }

		public JsonProperty(string name, JsonValue value)
		{
			Name = name;
			Value = value;
		}
	}
}
