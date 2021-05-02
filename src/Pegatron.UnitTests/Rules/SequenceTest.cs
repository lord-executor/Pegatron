using FluentAssertions;
using NUnit.Framework;
using Pegatron.Core;
using Pegatron.Core.Rules;
using Pegatron.UnitTests.Mocks;
using System.Linq;

namespace Pegatron.UnitTests.Rules
{
	[TestFixture]
	public class SequenceTest
	{
		[Test]
		public void AbbaSequence_ToString_ReturnsCorrectDisplayText()
		{
			var rule = CreateAbbaRule();

			rule.Name.Should().Be("TEST");
			rule.ToDisplayText().Should().Be("TEST");
			rule.ToDisplayText(DisplayMode.Long).Should().Be("(A B B A)");
		}

		[Test]
		[TestCase("ABBA", 0, "ABBA")]
		[TestCase("ABBA", 1, "")]
		[TestCase("ABB", 0, "ABB")]
		[TestCase("ABAB", 0, "AB")]
		[TestCase("ABABBA", 0, "AB")]
		[TestCase("ABABBA", 2, "ABBA")]
		[TestCase("ABBABBA", 0, "ABBA")]
		public void AbbaSequence_GivenParseTextAndStart_BehavesCorrectly(string text, int start, string expectedMatch)
		{
			var stream = new TokenStream(new CharacterLexer(text));
			var index = new TokenStreamIndex(stream, start);
			var rule = CreateAbbaRule();

			var opsMock = index.OperationsMock().Evaluate(rule);

			opsMock.Result.IsSuccess.Should().Be(expectedMatch.Length == 4);
			opsMock.Result.Index.Index.Should().Be(opsMock.Result.IsSuccess ? start + expectedMatch.Length : start);
			opsMock.ConcatTokens().Should().Be(expectedMatch);
		}

		private Sequence CreateAbbaRule()
		{
			var ruleA = new SimpleRef(new Terminal("A", "A"));
			var ruleB = new SimpleRef(new Terminal("B", "B"));
			return new Sequence("TEST", EnumSequence.Of(ruleA, ruleB, ruleB, ruleA));
		}
	}
}
