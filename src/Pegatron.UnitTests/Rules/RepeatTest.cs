using FluentAssertions;
using NUnit.Framework;
using Pegatron.Core;
using Pegatron.Core.Rules;
using Pegatron.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pegatron.UnitTests.Rules
{
	[TestFixture]
	public class RepeatTest
	{
		[Test]
		public void ARepeated_ToString_ReturnsCorrectDisplayText()
		{
			var rule = CreateARepeatedRule();

			rule.Name.Should().Be("TEST");
			rule.ToString().Should().Be("A{0,-1}");
		}

		[TestCaseSource(nameof(AnyNumberOfAs))]
		public void ARepeated_GivenArbitraryNumberOfAs_BehavesCorrectly(int count, string text)
		{
			var stream = new TokenStream(new CharacterLexer(text));
			var index = new TokenStreamIndex(stream, 0);
			var context = new RuleContextMock(index);
			var rule = CreateARepeatedRule();

			rule.Grab(context).ToList();

			context.Result.IsSuccess.Should().BeTrue();
			context.Result.Index.Index.Should().Be(count);
			context.Tokens.Count.Should().Be(count);
			context.Tokens.All(t => t.Value == "A").Should().BeTrue();
		}

		[Test]
		[TestCase("", false, "")]
		[TestCase("A", false, "A")]
		[TestCase("AAAAA", true, "AAAAA")]
		[TestCase("AAABAA", true, "AAA")]
		[TestCase("AAAAAAABAA", false, "AAAAAA")]
		public void ARepeated3to5_GivenParseTextAndStart_BehavesCorrectly(string text, bool expectedSuccess, string expectedMatch)
		{
			var stream = new TokenStream(new CharacterLexer(text));
			var index = new TokenStreamIndex(stream, 0);
			var context = new RuleContextMock(index);
			var rule = CreateARepeatedRule(3, 5);

			rule.Grab(context).ToList();

			context.Result.IsSuccess.Should().Be(expectedSuccess);
			context.Result.Index.Index.Should().Be(expectedSuccess ? expectedMatch.Length : 0);
			context.ConcatTokens().Should().Be(expectedMatch);
		}

		private Repeat CreateARepeatedRule(int min = 0, int max = -1)
		{
			var ruleA = new SimpleRef(new Terminal("A", "A"));
			return new Repeat("TEST", ruleA, min, max);
		}

		private static IEnumerable<TestCaseData> AnyNumberOfAs()
		{
			var terminator = new[] { "", "B", "C" };
			return Enumerable.Range(0, 8)
				.Select(i => {
					var count = (int)Math.Pow(i, 2);
					return new TestCaseData(count, Enumerable.Repeat("A", count).StrJoin("") + terminator[(i % 3)]);
				});
		}
	}
}
