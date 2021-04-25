using FluentAssertions;
using NUnit.Framework;
using Pegatron.Core;
using Pegatron.UnitTests.Mocks;

namespace Pegatron.UnitTests
{
	[TestFixture]
	public class GrammarTest
	{
		[Test]
		public void Grammar_WithoutStartRule_ThrowsExceptionOnStart()
		{
			var grammar = new AdHocGrammar();
			grammar.TerminalType("A", "start");

			grammar.Invoking(g => g.Start()).Should().Throw<GrammarException>().Where(e => e.Id == GrammarExceptionId.StartRuleNotDefined);
		}

		[Test]
		public void Grammar_WithStartRule_ReturnsStartRefOnStart()
		{
			var grammar = new AdHocGrammar();
			grammar.TerminalType("A", "start");
			grammar.StartWith("start");

			var ruleRef = grammar.Start();
			ruleRef.Should().NotBeNull();
			ruleRef.RefName.Should().BeNull();
			ruleRef.Name.Should().Be("start");
		}

		[Test]
		public void EvaluatingGrammar_WithMockRule_RunsMockRule()
		{
			var mockRule = new MockRule("mock", MockRuleBehavior.Success);
			var grammar = new AdHocGrammar();
			grammar.DefineRule("start", mockRule);
			grammar.StartWith("start");
			var lexer = StaticLexer.FromWords();
			var opsMock = new RuleOperationsMock(new TokenStream(lexer).Start());

			var ruleRef = grammar.Start();
			var result = opsMock.Evaluate(ruleRef).Result;

			result.IsSuccess.Should().BeTrue();
			mockRule.DidGrab.Should().BeTrue();
			result.Index.Index.Should().Be(0);
		}

		[Test]
		public void Grammar_ReferencingUndefinedRule_ReturnsUnresolvedRef()
		{
			var grammar = new AdHocGrammar();
			var ruleRef = grammar.Ref("test");

			ruleRef.Name.Should().BeNull();
			((RuleRef<CstNode>)ruleRef).IsResolved.Should().BeFalse();
		}

		[Test]
		public void Grammar_RefAs_SetsRefNameOnRuleRef()
		{
			var grammar = new AdHocGrammar();
			var ruleRef = grammar.Ref("test").As("handle");

			ruleRef.RefName.Should().Be("handle");
			((RuleRef<CstNode>)ruleRef).IsResolved.Should().BeFalse();
		}

		[Test]
		public void Grammar_RefReduceWith_SetsReducerOnRuleRef()
		{
			var grammar = new AdHocGrammar();
			Reducer<CstNode> reducer = (IRule rule, INodeContext<CstNode> page) => { return new CstNode("Dummy", "Dummy"); };
			var ruleRef = grammar.Ref("test").ReduceWith(reducer);

			ruleRef.Reducer.Should().Be(reducer);
			((RuleRef<CstNode>)ruleRef).IsResolved.Should().BeFalse();
		}

		[Test]
		public void StartWith_GrammarWithUnresovedRules_ThrowsException()
		{
			var grammar = new AdHocGrammar();
			grammar.Sequence("start", grammar.Ref("item"));

			grammar.Invoking(g => g.StartWith("start")).Should().Throw<GrammarException>().Where(e => e.Id == GrammarExceptionId.GrammarContainsUnresolvedRule);
		}
	}
}
