using System;
using System.Collections.Generic;
using System.Linq;

namespace Pegatron.UnitTests.Json
{
	public class JsonGrammar : Grammar<JsonValue>
	{
		public JsonGrammar()
		{
			this.DefineRule("value := object | array | primitive");
			this.DefineRule("object := '{' properties? #props '}'")
				.ReduceWith(ObjectReducer);
			this.DefineRule("array := '[' (primitive #values (',' primitive #values)?)? ']'")
				.ReduceWith(ArrayReducer);
			this.DefineRule("primitive := T<String> | T<Number> | T<Boolean> | T<Null>");
			this.DefineRule("properties := property #props (',' properties #props)?")
				.ReduceWith(PropertiesReducer);
			this.DefineRule("property := T<String> #name ':' value #value")
				.ReduceWith(PropertyReducer);

			StartWith("value");
		}

		public override IEnumerable<JsonValue> DefaultReducer(IRule rule, INodeContext<JsonValue> page)
		{
			if (page.HasLift())
			{
				return page.GetLift();
			}

			return page.GetAll();
		}

		public override IEnumerable<JsonValue> TerminalReducer(IRule rule, IToken token)
		{
			yield return new JsonPrimitive(token.Value ?? string.Empty, Enum.Parse<JsonTokenType>(token.Type));
		}

		private JsonValue ObjectReducer(IRule rule, INodeContext<JsonValue> page)
		{
			return new JsonObject(page.Get("props").As<JsonProperty>().ToList());
		}

		private JsonValue ArrayReducer(IRule rule, INodeContext<JsonValue> page)
		{
			return new JsonArray(page.Get("values").ToList());
		}

		private IEnumerable<JsonValue> PropertiesReducer(IRule rule, INodeContext<JsonValue> page)
		{
			return page.Get("props").As<JsonProperty>();
		}

		private JsonValue PropertyReducer(IRule rule, INodeContext<JsonValue> page)
		{
			var name = page.Get("name").Single<JsonPrimitive>().Text;
			var value = page.Get("value").Single();

			return new JsonProperty(name, value);
		}
	}
}
