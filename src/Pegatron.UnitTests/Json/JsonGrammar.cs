using System;
using System.Collections.Generic;

namespace Pegatron.UnitTests.Json
{
	public class JsonGrammar : Grammar<JsonValue>
	{
		public JsonGrammar()
		{
			this.DefineRule("value := object | array | primitive");
			this.DefineRule("object := '{' '}'");
			this.DefineRule("array := '[' (primitive (',' primitive)?)? ']'");
			this.DefineRule("primitive := T<String> | T<Number> | T<Boolean> | T<Null>");

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
	}
}
