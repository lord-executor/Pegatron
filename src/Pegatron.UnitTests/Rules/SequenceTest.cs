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
			rule.ToString().Should().Be("A B B A");
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
			var context = new RuleContextMock(index);
			var rule = CreateAbbaRule();

			rule.Grab(context).ToList();

			context.Result.IsSuccess.Should().Be(expectedMatch.Length == 4);
			context.Result.Index.Index.Should().Be(context.Result.IsSuccess ? start + expectedMatch.Length : start);
			context.ConcatTokens().Should().Be(expectedMatch);
		}

		private Sequence CreateAbbaRule()
		{
			var ruleA = new SimpleRef(new Terminal("A", "A"));
			var ruleB = new SimpleRef(new Terminal("B", "B"));
			return new Sequence("TEST", ruleA, ruleB, ruleB, ruleA);
		}
	}
}
