using FluentAssertions;
using NUnit.Framework;
using Pegatron.Core;
using Pegatron.Core.Rules;
using Pegatron.UnitTests.Mocks;
using System.Linq;

namespace Pegatron.UnitTests.Rules
{
	[TestFixture]
	public class AndNotTest
	{
		[Test]
		public void NotA_ToString_ReturnsCorrectDisplayText()
		{
			var rule = CreateNotARule();

			rule.Name.Should().Be("TEST");
			rule.ToDisplayText().Should().Be("TEST");
			rule.ToString().Should().Be("!T<A>");
		}

		[Test]
		public void AndA_ToString_ReturnsCorrectDisplayText()
		{
			var rule = CreateAndARule();

			rule.Name.Should().Be("TEST");
			rule.ToString().Should().Be("&T<A>");
		}

		[Test]
		[TestCase("", false)]
		[TestCase("A", true)]
		[TestCase("B", false)]
		[TestCase("C", false)]
		[TestCase("AA", true)]
		[TestCase("BAD", false)]
		public void AndANotB_GivenParseTextAndStart_BehaveCorrectlyAndInverse(string text, bool expectedSuccess)
		{
			var stream = new TokenStream(new CharacterLexer(text));
			var index = new TokenStreamIndex(stream, 0);
			var ruleAnd = CreateAndARule();
			var ruleNot = CreateNotARule();

			AssertLookahead(ruleAnd, index, expectedSuccess);
			AssertLookahead(ruleNot, index, !expectedSuccess);
		}

		private void AssertLookahead(IRule rule, TokenStreamIndex index, bool expectedSuccess)
		{
			var opsMock = index.OperationsMock().Evaluate(rule);

			opsMock.Result.IsSuccess.Should().Be(expectedSuccess);
			opsMock.Result.Index.Index.Should().Be(0);
			if (expectedSuccess && rule is And)
			{
				opsMock.ConcatTokens().Should().Be("A");
			}
		}

		private Not CreateNotARule()
		{
			var ruleA = new SimpleRef(new Terminal("A"));
			return new Not("TEST", ruleA);
		}

		private And CreateAndARule()
		{
			var ruleA = new SimpleRef(new Terminal("A"));
			return new And("TEST", ruleA);
		}
	}
}
