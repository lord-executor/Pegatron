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
		[TestCase(0, -1, "{0,}")]
		[TestCase(0, 5, "{0,5}")]
		[TestCase(3, 12, "{3,12}")]
		public void ARepeated_ToString_ReturnsCorrectDisplayText(int min, int max, string expectedSuffix)
		{
			var rule = CreateARepeatedRule(min, max);

			rule.Name.Should().Be("TEST");
			rule.ToDisplayText().Should().Be("TEST");
			rule.ToString().Should().Be("A" + expectedSuffix);
		}

		[TestCaseSource(nameof(AnyNumberOfAs))]
		public void ARepeated_GivenArbitraryNumberOfAs_BehavesCorrectly(int count, string text)
		{
			var stream = new TokenStream(new CharacterLexer(text));
			var index = new TokenStreamIndex(stream, 0);
			var rule = CreateARepeatedRule();

			var opsMock = index.OperationsMock().Evaluate(rule);

			opsMock.Result.IsSuccess.Should().BeTrue();
			opsMock.Result.Index.Index.Should().Be(count);
			opsMock.Tokens.Count.Should().Be(count);
			opsMock.Tokens.All(t => t.Value == "A").Should().BeTrue();
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
			var rule = CreateARepeatedRule(3, 5);

			var opsMock = index.OperationsMock().Evaluate(rule);

			opsMock.Result.IsSuccess.Should().Be(expectedSuccess);
			opsMock.Result.Index.Index.Should().Be(expectedSuccess ? expectedMatch.Length : 0);
			opsMock.ConcatTokens().Should().Be(expectedMatch);
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
