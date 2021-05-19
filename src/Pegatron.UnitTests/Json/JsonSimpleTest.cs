using FluentAssertions;
using NUnit.Framework;
using Pegatron.Core;
using System;
using System.Linq;

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

		[Test]
		[TestCase(@"[null]", JsonTokenType.Null)]
		[TestCase(@"[true]", JsonTokenType.Boolean)]
		[TestCase(@"[""foo""]", JsonTokenType.String)]
		[TestCase(@"[42]", JsonTokenType.Number)]
		public void JsonParse_WithSingleItemArray_Succeeds(string jsonText, JsonTokenType type)
		{
			var result = Parse(jsonText);

			result.Should().NotBeNull();
			result.Should().BeOfType<JsonArray>();

			var array = (JsonArray)result;
			array.Count.Should().Be(1);
			array.Should().AllBeOfType<JsonPrimitive>();
			array.OfType<JsonPrimitive>().All(p => p.ValueType == type);
		}

		[Test]
		[TestCase(@"[null,null]", JsonTokenType.Null)]
		[TestCase(@"[true,false]", JsonTokenType.Boolean)]
		[TestCase(@"[""foo"", ""bar""]", JsonTokenType.String)]
		[TestCase(@"[42, 42.4e2]", JsonTokenType.Number)]
		public void JsonParse_WithArrayOfPrimitives_Succeeds(string jsonText, JsonTokenType type)
		{
			var result = Parse(jsonText);

			result.Should().NotBeNull();
			result.Should().BeOfType<JsonArray>();

			var array = (JsonArray)result;
			array.Count.Should().Be(2);
			array.Should().AllBeOfType<JsonPrimitive>();
			array.OfType<JsonPrimitive>().All(p => p.ValueType == type);
		}

		[Test]
		public void JsonParse_EmptyObject_Succeeds()
		{
			var result = Parse("{}");

			result.Should().NotBeNull();
			result.Should().BeOfType<JsonObject>();

		}

		[Test]
		[TestCase(@"{""value"" : null, ""type"": ""Null""}")]
		[TestCase(@"{""value"" : true, ""type"": ""Boolean""}")]
		[TestCase(@"{""value"" : -5, ""type"": ""Number""}")]
		[TestCase(@"{""value"" : ""foo \"" bar"", ""type"": ""String""}")]
		public void JsonParse_DescriptiveObject_Succeeds(string jsonText)
		{
			var result = Parse(jsonText);

			result.Should().NotBeNull();
			result.Should().BeOfType<JsonObject>();

			var obj = (JsonObject)result;
			obj.Count.Should().Be(2);
			obj["value"].Should().BeOfType<JsonPrimitive>();
			(obj["value"] as JsonPrimitive)?.ValueType.Should().Be(Enum.Parse<JsonTokenType>((obj["type"] as JsonPrimitive)?.Text ?? string.Empty));
		}

		[Test]
		[TestCase(@"{
	""wrap"" : ""({0})"",
	""children"": [
		{ ""value"": 42 },
		{ ""wrap"": ""->{0}<-"", ""children"": [
			{ ""value"": ""foo"", ""wrap"": ""%{0}%"" },
			{ ""value"": ""bar"" }
		]}
	]}", "(42,->%foo%,bar<-)")]
		[TestCase(@"[{""value"":""hello""},{""value"":""world""},{""wrap"":""##{0}##"",""children"":null}]", "hello,world,##null##")]
		public void JsonParse_NestedStructure_ReducesCorrectly(string jsonText, string expectedResult)
		{
			var result = Parse(jsonText);
			var visitor = new WrappingVisitor();

			result.Should().NotBeNull();

			var serialized = visitor.Visit(result);
			serialized.Should().Be(expectedResult);
		}



		private JsonValue Parse(string text)
		{
			var grammar = new JsonGrammar();
			var parser = new Parser<JsonValue>(grammar);
			var lexer = new JsonLexer(text);

			return parser.Parse(new TokenStream(lexer).Start());
		}

		private class WrappingVisitor : IJsonVisitor<string>
		{
			public string Array(JsonArray value)
			{
				return value.Select(c => this.Visit(c)).StrJoin(",");
			}

			public string Object(JsonObject value)
			{
				var result = string.Empty;

				if (value.Has("value"))
				{
					result = this.Visit(value["value"]!);
				}
				else
				{
					result = this.Visit(value["children"]!);
				}

				if (value.Has("wrap"))
				{
					result = string.Format((value["wrap"] as JsonPrimitive)?.Text ?? string.Empty, result);
				}

				return result;
			}

			public string Primitive(JsonPrimitive value)
			{
				return value.Text;
			}
		}
	}
}
