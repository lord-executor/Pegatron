namespace Pegatron.UnitTests.Json
{
	public class JsonPrimitive : JsonValue
    {
		public string Value { get; }
		public JsonTokenType ValueType { get; }

		public JsonPrimitive(string value, JsonTokenType valueType)
		{
			Value = value;
			ValueType = valueType;
		}
	}
}
