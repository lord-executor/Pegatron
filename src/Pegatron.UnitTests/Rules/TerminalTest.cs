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
			rule.ToString().Should().Be("TOKEN_TYPE");
		}

		[Test]
		public void TerminalWithCustomMatcher_ToString_ReturnsDisplayText()
		{
			var rule = new Terminal("TEST", "DISPLAY_TEXT", token => true);

			rule.Name.Should().Be("TEST");
			rule.ToString().Should().Be("DISPLAY_TEXT");
		}

		[Test]
		public void TerminalWithCustomMatcherNoDisplayText_ToString_ReturnsDefaultText()
		{
			var rule = new Terminal("TEST", null, token => true);

			rule.Name.Should().Be("TEST");
			rule.ToString().Should().Contain(nameof(Terminal));
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

			while (!index.Get().IsEndOfStream)
			{
				var context = new RuleContextMock(index);
				rule.Grab(context).ToList();

				if (successSet.Contains(index.Index))
				{
					context.Result.IsSuccess.Should().BeTrue();
					context.Tokens.Count.Should().Be(1);
					context.Tokens.All(t => t.Value == "A").Should().BeTrue();
				}
				else
				{
					context.Result.IsSuccess.Should().BeFalse();
					context.Tokens.Count.Should().Be(0);
				}

				index = index.Next();
			}

			var eosContext = new RuleContextMock(index);
			rule.Grab(eosContext).ToList();
			eosContext.Result.IsSuccess.Should().BeFalse();
			eosContext.Tokens.Count.Should().Be(0);
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
			var rule = new Terminal("TEST", "UPPER", t => Char.IsUpper((t.Value ?? "_"), 0));
			var result = new List<string>();

			while (!index.Get().IsEndOfStream)
			{
				var context = new RuleContextMock(index);
				rule.Grab(context).ToList();

				if (context.Result.IsSuccess)
				{
					result.Add(context.Tokens.Single().Value!);
				}

				index = index.Next();
			}

			result.StrJoin(String.Empty).Should().Be(expectedResult);
		}
	}
}
