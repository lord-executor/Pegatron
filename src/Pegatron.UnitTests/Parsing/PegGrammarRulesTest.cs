using FluentAssertions;
using NUnit.Framework;
using Pegatron.Core;
using System;

namespace Pegatron.UnitTests.Parsing
{
	[TestFixture]
	public class PegGrammarRulesTest
	{
		[Test]
		public void Definition_NestedRefNamePropagation_ThrowsException()
		{
			Action action = () => CreateParser("main := 'A' ('B' #foo 'C') #bar");

			action.Should().Throw<Exception>().WithMessage("*is already named bar*");
		}

		[Test]
		public void Definition_WithDoubleRefNamePropagation_ThrowsException()
		{
			Action action = () => CreateParser("main := 'A' ('B' #foo 'C' #bar)");

			action.Should().Throw<Exception>().WithMessage("*multiple distinct ref names*");
		}

		[Test]
		public void Definitions_WithMinMaxVariants_GetCorrectMinMaxValues()
		{
			var grammar = new AdHocGrammar();
			grammar.DefineRule("rule1 := 'A'{3}").ToDisplayText(DisplayMode.Definition).Should().Be("rule1 := T<'A'>{3,3}");
			grammar.DefineRule("rule2 := 'A'{3,3}").ToDisplayText(DisplayMode.Definition).Should().Be("rule2 := T<'A'>{3,3}");
			grammar.DefineRule("rule3 := 'A'{3,}").ToDisplayText(DisplayMode.Definition).Should().Be("rule3 := T<'A'>{3,}");
		}

		private Parser<CstNode> CreateParser(params string[] ruleDefinitions)
		{
			var grammar = new AdHocGrammar();
			foreach (var ruleDefinition in ruleDefinitions)
			{
				grammar.DefineRule(ruleDefinition);
			}
			return new Parser<CstNode>(grammar);
		}
	}
}
