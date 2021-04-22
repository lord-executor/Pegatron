using FluentAssertions;
using NUnit.Framework;
using Pegatron.Core;
using Pegatron.UnitTests.Mocks;
using System;
using System.Collections.Generic;

namespace Pegatron.UnitTests
{
	[TestFixture]
	public class RuleRefTest
	{
		[Test]
		public void RuleRef_ParameterlessConstructor_CreatesUnresolvedRef()
		{
			var ruleRef = new RuleRef<string>();

			ruleRef.IsResolved.Should().BeFalse();
			ruleRef.Name.Should().BeNull();
			ruleRef.RefName.Should().BeNull();
			ruleRef.Reducer.Should().BeNull();
		}

		[Test]
		public void RuleRef_As_SetsRefName()
		{
			var ruleRef = new RuleRef<string>();

			ruleRef.As(nameof(RuleRef_As_SetsRefName));

			ruleRef.RefName.Should().Be(nameof(RuleRef_As_SetsRefName));
		}

		[Test]
		public void RuleRef_ReduceWith_SetsReducer()
		{
			var ruleRef = new RuleRef<string>();
			Reducer<string> reducer = (IRule rule, INodeContext<string> page) => { return String.Empty; };

			ruleRef.ReduceWith(reducer);

			ruleRef.Reducer.Should().Be(reducer);
		}

		[Test]
		public void Grab_UnresolvedRuleRef_ThrowsException()
		{
			var ruleRef = new RuleRef<string>();
			var index = new TokenStreamIndex(new TokenStream(StaticLexer.FromWords()), 0);

			ruleRef.Invoking(r => index.OperationsMock().Evaluate(r)).Should().Throw<InvalidOperationException>();
		}

		[Test]
		public void Clone_UnresolvedRuleRef_ThrowsException()
		{
			var ruleRef = new RuleRef<string>();

			ruleRef.Invoking(r => r.CloneRule()).Should().Throw<InvalidOperationException>();
		}

		[Test]
		public void RuleRef_WithRule_WrapsTargetRule()
		{
			var ruleRef = new RuleRef<string>(new MockRule(nameof(MockRule)));

			ruleRef.IsResolved.Should().BeTrue();
			ruleRef.Name.Should().Be(nameof(MockRule));
		}

		[Test]
		public void RuleRef_GrabWithRule_GrabsTargetRule()
		{
			var mockRule = new MockRule(nameof(MockRule));
			var ruleRef = new RuleRef<string>(mockRule);

			var index = new TokenStreamIndex(new TokenStream(StaticLexer.FromWords()), 0);
			index.OperationsMock().Evaluate(ruleRef);

			mockRule.DidGrab.Should().BeTrue();
		}

		[Test]
		public void RuleRef_CloneWithRule_PointsToSameTargetRuleAndReducer()
		{
			var ruleName = "TEST";
			var mockRule = new MockRule(ruleName);
			var ruleRef = new RuleRef<string>(mockRule);
			Reducer<string> reducer = (IRule rule, INodeContext<string> page) => { return String.Empty; };

			ruleRef
				.As(nameof(RuleRef_CloneWithRule_PointsToSameTargetRuleAndReducer))
				.ReduceWith(reducer);

			var clone = ruleRef.CloneRule();

			clone.RefName.Should().BeNull();
			clone.Name.Should().Be(ruleName);
			clone.Reducer.Should().Be(reducer);
		}

		private class MockRule : IRule
		{
			public string? Name { get; }
			public bool DidGrab { get; private set; }

			public MockRule(string name)
			{
				Name = name;
			}

			public IEnumerable<RuleOperation> Grab(IRuleContext ctx)
			{
				DidGrab = true;
				yield break;
			}
		}
	}
}
