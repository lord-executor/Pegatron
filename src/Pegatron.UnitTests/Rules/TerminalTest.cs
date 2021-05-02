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
	public class TerminalTest
	{
		[Test]
		public void TerminalFromTokenType_ToString_ReturnsTokenType()
		{
			var rule = new Terminal("TEST", "TOKEN_TYPE");

			rule.Name.Should().Be("TEST");
			rule.ToDisplayText().Should().Be("TEST");
			rule.ToDisplayText(DisplayMode.Long).Should().Be("T<TOKEN_TYPE>");
		}

		[Test]
		public void TerminalWithPredicateMatcher_ToString_ReturnsDisplayText()
		{
			var rule = new Terminal("TEST", new TokenPredicateMatcher("DISPLAY_TEXT", token => true));

			rule.Name.Should().Be("TEST");
			rule.ToDisplayText().Should().Be("TEST");
			rule.ToDisplayText(DisplayMode.Long).Should().Be("T<DISPLAY_TEXT>");
		}

		[Test]
		public void TerminalWithPredicateMatcherNoDisplayText_ToString_ReturnsDefaultText()
		{
			var rule = new Terminal("TEST", new TokenPredicateMatcher(token => true));

			rule.Name.Should().Be("TEST");
			rule.ToDisplayText().Should().Be("TEST");
			rule.ToDisplayText(DisplayMode.Long).Should().Be("T<?>");
		}

		[Test]
		[TestCase("", new int[] { })]
		[TestCase("ABBA", new[] { 0, 3 })]
		[TestCase("BAAB", new[] { 1, 2 })]
		[TestCase("AAA", new[] { 0, 1, 2 })]
		[TestCase("BB", new int[] { })]
		public void Terminal_SucceedsAndFails_Correctly(string text, int[] successIndices)
		{
			var successSet = new HashSet<int>(successIndices);
			var index = new TokenStream(new CharacterLexer(text)).Start();
			var rule = new Terminal("TEST", "A");
			RuleOperationsMock opsMock;

			while (!index.Get().IsEndOfStream)
			{
				opsMock = index.OperationsMock().Evaluate(rule);

				if (successSet.Contains(index.Index))
				{
					opsMock.Result.IsSuccess.Should().BeTrue();
					opsMock.Result.Index.Index.Should().Be(index.Index + 1);
					opsMock.Tokens.Count.Should().Be(1);
					opsMock.Tokens.All(t => t.Value == "A").Should().BeTrue();
				}
				else
				{
					opsMock.Result.IsSuccess.Should().BeFalse();
					opsMock.Result.Index.Index.Should().Be(index.Index);
					opsMock.Tokens.Count.Should().Be(0);
				}

				index = index.Next();
			}

			opsMock = index.OperationsMock().Evaluate(rule);
			opsMock.Result.IsSuccess.Should().BeFalse();
			opsMock.Tokens.Count.Should().Be(0);
		}

		[Test]
		[TestCase("", "")]
		[TestCase("AbBa", "AB")]
		[TestCase("BAab","BA")]
		[TestCase("AaBCa", "ABC")]
		[TestCase("ABCabcABC", "ABCABC")]
		public void TerminalWithCustomMatcher_SucceedsAndFails_Correctly(string text, string expectedResult)
		{
			var index = new TokenStream(new CharacterLexer(text)).Start();
			var rule = new Terminal("TEST", new TokenPredicateMatcher("UPPER", t => Char.IsUpper((t.Value ?? "_"), 0)));
			var result = new List<string>();

			while (!index.Get().IsEndOfStream)
			{
				var opsMock = index.OperationsMock().Evaluate(rule);

				if (opsMock.Result.IsSuccess)
				{
					result.Add(opsMock.Tokens.Single().Value!);
				}

				index = index.Next();
			}

			result.StrJoin(String.Empty).Should().Be(expectedResult);
		}
	}
}
