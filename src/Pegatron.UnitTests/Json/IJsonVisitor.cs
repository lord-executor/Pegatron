using Pegatron.UnitTests.Json;
using System;

namespace Pegatron.UnitTests.Json
{
	public interface IJsonVisitor<TResult>
	{
		TResult Object(JsonObject value);
		TResult Array(JsonArray value);
		TResult Primitive(JsonPrimitive value);
	}

	public static class JsonVisitorExtensions
	{
		public static TResult Visit<TResult>(this IJsonVisitor<TResult> visitor, JsonValue value)
		{
			switch (value)
			{
				case JsonObject jsonObject:
					return visitor.Object(jsonObject);

				case JsonArray jsonArray:
					return visitor.Array(jsonArray);

				case JsonPrimitive jsonPrimitive:
					return visitor.Primitive(jsonPrimitive);

				default:
					throw new ArgumentException($"Unexpected JSON type {value.Type} in evaluation tree");
			}
		}
	}
}
