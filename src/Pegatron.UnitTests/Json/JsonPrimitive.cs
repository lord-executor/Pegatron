using System.Text.RegularExpressions;

namespace Pegatron.UnitTests.Json
{
	public class JsonPrimitive : JsonValue
    {
		public string Value { get; }
		public JsonTokenType ValueType { get; }

		public string Text => ValueType == JsonTokenType.String
			? Value.Substring(1, Value.Length - 2)
			: Value;

		public JsonPrimitive(string value, JsonTokenType valueType)
		{
			Value = value;
			ValueType = valueType;
		}
	}
}
