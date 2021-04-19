using FluentAssertions;
using NUnit.Framework;
using Pegatron.Core;
using System.Linq;

namespace Pegatron.UnitTests.Parsing
{
	[TestFixture]
	public class ParserTestSimpleGrammar
	{
		[Test]
		[TestCase("AURRX", "main[A choose[U] R R X]")]
		[TestCase("AVRRRY", "main[A choose[V] R R R Y]")]
		[TestCase("AURRRR_", "main[A choose[U] R R R R _]")]
		[TestCase("AVRRRRR§", "main[A choose[V] R R R R R §]")]
		public void SimpleGrammar_ParseText_ShouldSucceedAndReturnCorrectCst(string text, string expectedReduction)
		{
			var stream = new TokenStream(new CharacterLexer(text));
			var parser = new Parser<CstNode>(SimpleGrammar());

			var root = parser.Parse(stream.Start());

			root.Should().NotBeNull();
			root.Name.Should().Be("main");

			ReduceCst(root).Should().Be(expectedReduction);
		}

		[Test]
		public void SimpleGrammar_ParseTextWithTooMuchInput_ShouldFail()
		{
			var stream = new TokenStream(new CharacterLexer("AURRX+"));
			var parser = new Parser<CstNode>(SimpleGrammar());

			parser.Invoking(p => p.Parse(stream.Start())).Should().Throw<ParserException>().Where(e => e.Id == ParserExceptionId.PartialMatch);
		}

		[Test]
		[TestCase("A")]
		[TestCase("AWRRX")]
		public void SimpleGrammar_ParseTextWithIncorrectInput_ShouldFail(string text)
		{
			var stream = new TokenStream(new CharacterLexer(text));
			var parser = new Parser<CstNode>(SimpleGrammar());

			parser.Invoking(p => p.Parse(stream.Start())).Should().Throw<ParserException>().Where(e => e.Id == ParserExceptionId.ParsingFailed);
		}

		private static IGrammar<CstNode> SimpleGrammar()
		{
			var grammar = new AdHocGrammar();

			// main   := A choose R{2,5} .
			// choose := U | V
			grammar.Sequence("main",
				grammar.Terminal("A"),
				grammar.Choice("choose",
					grammar.Terminal("U"),
					grammar.Terminal("V")
				),
				grammar.Repeat(null, grammar.Terminal("R"), 2, 5),
				grammar.Any(null)
			);

			grammar.StartWith("main");

			return grammar;
		}

		private static string ReduceCst(CstNode node)
		{
			if (node.Name != null)
			{
				var inner = node.Value == null
					? node.Children.Select(c => ReduceCst(c)).StrJoin(" ")
					: node.Value;

				return $"{node.Name}[{inner}]";
			}
			if (node.Value != null)
			{
				return node.Value;
			}

			return node.Children.Select(c => ReduceCst(c)).StrJoin(" ");
		}
	}
}
