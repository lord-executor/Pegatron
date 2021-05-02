using FluentAssertions;
using NUnit.Framework;
using Pegatron.Core;
using Pegatron.Grammars.Peg;
using Pegatron.Grammars.Peg.Ast;
using Pegatron.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Pegatron.UnitTests.Parsing
{
	[TestFixture]
	public class PegGrammarTest
	{
		[Test]
		public void TEST()
		{
			var rule = (ProtoRule)Parse("definition        := T<Identifier>#name ':=' choice#rule");
			var grammar = new DynamicPegGrammar();
			DefineReferencedRulesAsDummies(grammar, rule);
			var root = rule.Create(grammar);

			var expandedRule = "(T<Identifier> #name T<':='> choice #rule)";
			root.Name.Should().Be("definition");
			root.ToDisplayText(DisplayMode.Long).Should().Be(expandedRule);

			var text = $"{root.Name}2 := {root.ToDisplayText(DisplayMode.Long)}";
			rule = (ProtoRule)Parse(text);
			root = rule.Create(grammar);

			root.Name.Should().Be("definition2");
			root.ToDisplayText(DisplayMode.Long).Should().Be(expandedRule);
		}

		[Test]
		public void TEST2()
		{
			var rule = (ProtoRule)Parse("minmax            := '{' T<NUMBER> #min (',' T<NUMBER> #max)? '}'");
			var grammar = new DynamicPegGrammar();
			DefineReferencedRulesAsDummies(grammar, rule);
			var root = rule.Create(grammar);

			root.Name.Should().Be("minmax");
			// note that the #max named ref has been propagated to the top level of the rule through the
			// use of the lift (#!) ref name.
			root.ToDisplayText(DisplayMode.Long).Should().Be("(T<'{'> T<NUMBER> #min (T<','> T<NUMBER> #!){0,1} #max T<'}'>)");

			rule.Should().NotBeNull();
		}

		private INode Parse(string expression)
		{
			var lexer = new Lexer(new StringReader(expression));
			var grammar = new PegGrammar();
			var parser = new Parser<INode>(grammar);

			return parser.Parse(new TokenStream(lexer).Start());
		}

		private void DefineReferencedRulesAsDummies(IGrammarBuilder<INode> grammar, ProtoRule rule)
		{
			foreach (var child in rule.Children)
			{
				if (child.DisplayText == "Ref")
				{
					grammar.DefineRule(child.RuleName, new MockRule(child.RuleName));
				}
				DefineReferencedRulesAsDummies(grammar, child);
			}
		}

		public class DynamicPegGrammar : Grammar<INode>
		{
			public override IEnumerable<INode> DefaultReducer(IRule rule, INodeContext<INode> page)
			{
				return page.GetAll();
			}

			public override IEnumerable<INode> TerminalReducer(IRule rule, IToken token)
			{
				ArgAssert.NotNull(nameof(token.Value), token.Value);
				yield return new Value(token.Value);
			}
		}
	}
}
