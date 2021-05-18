using FluentAssertions;
using NUnit.Framework;
using Pegatron.Core;
using System.Linq;

namespace Pegatron.UnitTests.Parsing
{
	[TestFixture]
	public class ParserSimpleGrammarTest
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

			CstNode.Reduce(root).Should().Be(expectedReduction);
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
				grammar.TerminalType("A"),
				grammar.Choice("choose",
					grammar.TerminalValue("U"),
					grammar.TerminalValue("V")
				),
				grammar.Repeat(null, grammar.TerminalType("R"), 2, 5),
				grammar.Any(null)
			);

			grammar.StartWith("main");

			return grammar;
		}
	}
}
