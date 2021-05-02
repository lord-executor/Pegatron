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
		[TestCaseSource("PegGrammarRules")]
		public void DefinitionText_WithPegGrammar_CanParseItself(string ruleText, string name, string expandedRuleText)
		{
			var grammar = new DynamicPegGrammar();
			var rule = (ProtoRule)Parse(ruleText);
			DefineReferencedRulesAsDummies(grammar, rule, new HashSet<string>());
			var root = rule.Create(grammar);

			root.Name.Should().Be(name);
			root.ToDisplayText(DisplayMode.Long).Should().Be(expandedRuleText);

			// now we parse the long form to see if we still get the same result
			//var text = $"{name}2 := {root.ToDisplayText(DisplayMode.Long)}";
			//rule = (ProtoRule)Parse(text);
			//root = rule.Create(grammar);

			//root.Name.Should().Be($"{name}2");
			//root.ToDisplayText(DisplayMode.Long).Should().Be(expandedRuleText);
		}

		private INode Parse(string expression)
		{
			var lexer = new Lexer(new StringReader(expression));
			var grammar = new PegGrammar();
			var parser = new Parser<INode>(grammar);

			return parser.Parse(new TokenStream(lexer).Start());
		}

		private void DefineReferencedRulesAsDummies(IGrammarBuilder<INode> grammar, ProtoRule rule, HashSet<string> defined)
		{
			foreach (var child in rule.Children)
			{
				if (child.DisplayText == "Ref" && child.RuleName != null && !defined.Contains(child.RuleName))
				{
					defined.Add(child.RuleName);
					grammar.DefineRule(child.RuleName, new MockRule(child.RuleName));
				}
				DefineReferencedRulesAsDummies(grammar, child, defined);
			}
		}

		private static IEnumerable<TestCaseData> PegGrammarRules()
		{
			yield return new TestCaseData(PegGrammar.Definition.DefinitionText, "definition", "(T<Identifier> #name T<':='> choice #rule)");
			yield return new TestCaseData(PegGrammar.Choice.DefinitionText, "choice", "(sequence (T<'|'> sequence #!){0,})");
			yield return new TestCaseData(PegGrammar.Sequence.DefinitionText, "sequence", "namedAtom{1,}");
			yield return new TestCaseData(PegGrammar.NamedAtom.DefinitionText, "namedAtom", "");
			yield return new TestCaseData(PegGrammar.AtomExpression.DefinitionText, "atomExpression", "");
			yield return new TestCaseData(PegGrammar.RangeDef.DefinitionText, "range", "");
			yield return new TestCaseData(PegGrammar.MinMaxDef.DefinitionText, "minmax", "(T<'{'> T<Number> #min (T<','> T<Number> #!){0,1} #max T<'}'>)");
			yield return new TestCaseData(PegGrammar.Atom.DefinitionText, "atom", "");
			yield return new TestCaseData(PegGrammar.RuleRefDef.DefinitionText, "ruleRef", "T<Identifier>");
			yield return new TestCaseData(PegGrammar.Terminal.DefinitionText, "terminal", "");
			yield return new TestCaseData(PegGrammar.TerminalType.DefinitionText, "terminalType", "");
			yield return new TestCaseData(PegGrammar.TerminalLiteral.DefinitionText, "terminalLiteral", "(T<Literal> | (T<'T'> T<'<'> T<Literal> #! T<'>'>))");
			yield return new TestCaseData(PegGrammar.TerminalAny.DefinitionText, "terminalAny", "(T<'T'> T<'<'> T<Identifier> #type T<'>'>)");
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
