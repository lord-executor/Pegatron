using System.Text.RegularExpressions;

namespace Pegatron.UnitTests.Json
{
	public class JsonPrimitive : JsonValue
    {
		private readonly Regex _escape = new Regex(@"\\([""\\/])", RegexOptions.Compiled);
		public string Value { get; }
		public JsonTokenType ValueType { get; }

		public string Text => ValueType == JsonTokenType.String
			? _escape.Replace(Value.Substring(1, Value.Length - 2), m => m.Groups[1].Value)
			: Value;

		public JsonPrimitive(string value, JsonTokenType valueType)
		{
			Value = value;
			ValueType = valueType;
		}
	}
}
