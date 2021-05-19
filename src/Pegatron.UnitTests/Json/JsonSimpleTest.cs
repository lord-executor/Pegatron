using FluentAssertions;
using NUnit.Framework;
using Pegatron.Core;

namespace Pegatron.UnitTests.Json
{
	[TestFixture]
	public class JsonSimpleTest
	{
		[Test]
		[TestCase(@"null", JsonTokenType.Null)]
		[TestCase(@"true", JsonTokenType.Boolean)]
		[TestCase(@"false", JsonTokenType.Boolean)]
		[TestCase(@"""foo""", JsonTokenType.String)]
		[TestCase(@"42", JsonTokenType.Number)]
		[TestCase(@"42.42", JsonTokenType.Number)]
		[TestCase(@"42.4e2", JsonTokenType.Number)]
		public void JsonParse_WithPrimitiveValue_Succeeds(string jsonText, JsonTokenType type)
		{
			var result = Parse(jsonText);

			result.Should().NotBeNull();
			result.Should().BeOfType<JsonPrimitive>();

			var value = (JsonPrimitive)result;
			value.Value.Should().Be(jsonText);
			value.ValueType.Should().Be(type);
		}

		private JsonValue Parse(string text)
		{
			var grammar = new JsonGrammar();
			var parser = new Parser<JsonValue>(grammar);
			var lexer = new JsonLexer(text);

			return parser.Parse(new TokenStream(lexer).Start());
		}
	}
}
