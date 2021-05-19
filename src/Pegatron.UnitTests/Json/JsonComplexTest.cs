using FluentAssertions;
using NUnit.Framework;
using Pegatron.Core;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Pegatron.UnitTests.Json
{
	[TestFixture]
	public class JsonComplexTest
	{
		[Test]
		public void PersonsDataSet_ParsesSuccessfully_WithReasonablePerformance()
		{
			JsonValue? result = null;
			Action parse = () => result = Parse(GetResourceFile("Pegatron.UnitTests.Json.sample_data.persons.json"));

			parse.ExecutionTime().Should().BeLessThan(TimeSpan.FromSeconds(1));

			result.Should().NotBeNull();
			result.Should().BeOfType<JsonArray>();

			var value = (JsonArray)result!;
			value.Count.Should().Be(1000);
		}

		[Test]
		public void FileTreeDataSet_ParsesSuccessfully_AggregateFileSizeIsCorrect()
		{
			JsonValue? result = null;
			Action parse = () => result = Parse(GetResourceFile("Pegatron.UnitTests.Json.sample_data.file-tree.json"));

			parse.ExecutionTime().Should().BeLessThan(TimeSpan.FromSeconds(1));

			result.Should().NotBeNull();
			result.Should().BeOfType<JsonObject>();

			var fileSizeVisitor = new FileSizeVisitor();
			fileSizeVisitor.Visit(result!).Should().Be(8_158_485L);
		}

		private JsonValue Parse(TextReader reader)
		{
			var grammar = new JsonGrammar();
			var parser = new Parser<JsonValue>(grammar);
			var lexer = new JsonLexer(reader);

			return parser.Parse(new TokenStream(lexer).Start());
		}

		private TextReader GetResourceFile(string resourceName)
		{
			var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
			if (stream == null)
			{
				throw new Exception("Unable to open requested resource file");
			}
			return new StreamReader(stream);
		}

		private class FileSizeVisitor : IJsonVisitor<long>
		{
			public long Array(JsonArray value)
			{
				return value.Select(c => this.Visit(c)).Sum();
			}

			public long Object(JsonObject value)
			{
				var size = long.Parse((value["Size"] as JsonPrimitive)?.Text ?? "0");
				size += this.Visit(value["Children"] ?? throw new Exception("Missing 'Children' property"));
				return size;
			}

			public long Primitive(JsonPrimitive value)
			{
				return 0;
			}
		}
	}
}
