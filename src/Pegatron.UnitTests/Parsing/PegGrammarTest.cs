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
		public void PegDefinition_RegisterDefinition_SerializesToLongForm(IGrammarRule<INode> ruleDef, string ruleText, string name, string expandedRuleText)
		{
			var grammar = new DynamicPegGrammar();
			ruleDef.Register(grammar);
			grammar.DefineUnresolvedRules();
			grammar.StartWith(name);
			var root = grammar.Ref(name);

			root.Name.Should().Be(name);
			root.ToDisplayText(DisplayMode.Long).Should().Be(expandedRuleText);
		}

		[Test]
		[TestCaseSource("PegGrammarRules")]
		public void PegGrammar_ParseShortForm_SerializesToLongForm(IGrammarRule<INode> ruleDef, string ruleText, string name, string expandedRuleText)
		{
			var grammar = new DynamicPegGrammar();
			var rule = (ProtoRule)Parse(ruleText);
			DefineReferencedRulesAsDummies(grammar, rule, new HashSet<string>());
			var root = rule.Create(grammar);

			root.Name.Should().Be(name);
			root.ToDisplayText(DisplayMode.Long).Should().Be(expandedRuleText);
		}

		[Test]
		[TestCaseSource("PegGrammarRules")]
		public void PegGrammar_ParseLongForm_SerializesToLongForm(IGrammarRule<INode> ruleDef, string ruleText, string name, string expandedRuleText)
		{
			var grammar = new DynamicPegGrammar();
			var rule = (ProtoRule)Parse($"{name} := {expandedRuleText}");
			DefineReferencedRulesAsDummies(grammar, rule, new HashSet<string>());
			var root = rule.Create(grammar);

			root.Name.Should().Be(name);
			root.ToDisplayText(DisplayMode.Long).Should().Be(expandedRuleText);
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
			yield return new TestCaseData(
				new Definition(),
				Definition.DefinitionText,
				"definition",
				"(T<Identifier> #name T<':='> choice #rule)"
			);
			yield return new TestCaseData(
				new Choice(),
				Choice.DefinitionText,
				"choice",
				"(sequence (T<'|'> sequence #!){0,})"
			);
			yield return new TestCaseData(
				new Sequence(),
				Sequence.DefinitionText,
				"sequence",
				"namedAtom{1,}"
			);
			yield return new TestCaseData(
				new NamedAtom(),
				NamedAtom.DefinitionText,
				"namedAtom",
				"(atomExpression #atom (T<'#!'> | (T<'#'> T<Identifier>)){0,1} #name)"
			);
			yield return new TestCaseData(
				new AtomExpression(),
				AtomExpression.DefinitionText,
				"atomExpression",
				"((T<'&'> | T<'!'>){0,1} #prefix atom range{0,1} #suffix)"
			);
			yield return new TestCaseData(
				new RangeDef(),
				RangeDef.DefinitionText,
				"range",
				"(T<'*'> | T<'+'> | T<'?'> | minmax)"
			);
			yield return new TestCaseData(
				new MinMaxDef(),
				MinMaxDef.DefinitionText,
				"minmax",
				"(T<'{'> T<Number> #min T<','>{0,1} #sep T<Number>{0,1} #max T<'}'>)"
			);
			yield return new TestCaseData(
				new Atom(),
				Atom.DefinitionText,
				"atom",
				"(ruleRef | terminal | (T<'('> choice #! T<')'>))"
			);
			yield return new TestCaseData(
				new RuleRefDef(),
				RuleRefDef.DefinitionText,
				"ruleRef",
				"T<Identifier>"
			);
			yield return new TestCaseData(
				new Terminal(),
				Terminal.DefinitionText,
				"terminal",
				"(terminalType | terminalLiteral | terminalAny)"
			);
			yield return new TestCaseData(
				new TerminalType(),
				TerminalType.DefinitionText,
				"terminalType",
				"(T<'T'> T<'<'> T<Identifier> #type T<'>'>)"
			);
			yield return new TestCaseData(
				new TerminalLiteral(),
				TerminalLiteral.DefinitionText,
				"terminalLiteral",
				"(T<Literal> | (T<'T'> T<'<'> T<Literal> #! T<'>'>))"
			);
			yield return new TestCaseData(
				new TerminalAny(),
				TerminalAny.DefinitionText,
				"terminalAny",
				"T<'.'>"
			);
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

			public void DefineUnresolvedRules()
			{
				foreach (var key in _unresolvedRules.Keys)
				{
					DefineRule(key, new MockRule(key));
				}
			}
		}
	}
}
