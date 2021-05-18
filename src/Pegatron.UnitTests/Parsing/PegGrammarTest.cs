using FluentAssertions;
using NUnit.Framework;
using Pegatron.Core;

namespace Pegatron.UnitTests.Parsing
{
	[TestFixture]
	public class PegGrammarTest
	{
		[Test]
		public void RuleDefinition_WithRecursion_IdentifiesMatchingAAndB()
		{
			var parser = CreateParser("main", "main := 'A' main? 'B'");
			var result = parser.Parse(new TokenStream(new CharacterLexer("AAABBB")).Start());
			CstNode.Reduce(result).Should().Be("main[A main[A main[A B] B] B]");

			parser.Invoking(p => p.Parse(new TokenStream(new CharacterLexer("AAABB")).Start())).Should().Throw<ParserException>().Where(e => e.Id == ParserExceptionId.ParsingFailed);
			parser.Invoking(p => p.Parse(new TokenStream(new CharacterLexer("AABBB")).Start())).Should().Throw<ParserException>().Where(e => e.Id == ParserExceptionId.PartialMatch);
		}

		[Test]
		public void RuleDefinition_WithPositiveLookahead_ResetsForNextMatch()
		{
			var parser = CreateParser("main",
				"main := &'A' secondary | .*",
				"secondary := 'A' 'B' 'C'"
			);
			var result = parser.Parse(new TokenStream(new CharacterLexer("ABC")).Start());
			CstNode.Reduce(result).Should().Be("main[A secondary[A B C]]");

			result = parser.Parse(new TokenStream(new CharacterLexer("BCA")).Start());
			CstNode.Reduce(result).Should().Be("main[B C A]");

			result = parser.Parse(new TokenStream(new CharacterLexer("AABC")).Start());
			CstNode.Reduce(result).Should().Be("main[A A B C]");
		}

		[Test]
		public void RuleDefinition_WithNegativeLookahead_ResetsForNextMatch()
		{
			var parser = CreateParser("main",
				"main := !'A' secondary | .*",
				"secondary := . ('B' | 'C') .*"
			);

			var result = parser.Parse(new TokenStream(new CharacterLexer("ABC")).Start());
			CstNode.Reduce(result).Should().Be("main[A B C]");

			result = parser.Parse(new TokenStream(new CharacterLexer("BCA")).Start());
			CstNode.Reduce(result).Should().Be("main[secondary[B C A]]");

			result = parser.Parse(new TokenStream(new CharacterLexer("AABC")).Start());
			CstNode.Reduce(result).Should().Be("main[A A B C]");
		}

		private Parser<CstNode> CreateParser(string startWith, params string[] ruleDefinitions)
		{
			var grammar = new AdHocGrammar();
			foreach (var ruleDefinition in ruleDefinitions)
			{
				grammar.DefineRule(ruleDefinition);
			}

			grammar.StartWith(startWith);
			return new Parser<CstNode>(grammar);
		}
	}
}
