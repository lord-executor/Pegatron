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
	public class ChoiceTest
	{
		[Test]
		public void AOrB_ToString_ReturnsCorrectDisplayText()
		{
			var rule = CreateAOrBRule();

			rule.Name.Should().Be("TEST");
			rule.ToString().Should().Be("A | B");
		}

		[Test]
		[TestCase("", 0, "")]
		[TestCase("A", 0, "A")]
		[TestCase("B", 0, "B")]
		[TestCase("C", 0, "")]
		[TestCase("XXABXX", 2, "A")]
		[TestCase("XXABXX", 3, "B")]
		public void AbbaSequence_GivenParseTextAndStart_BehavesCorrectly(string text, int start, string expectedMatch)
		{
			var stream = new TokenStream(new CharacterLexer(text));
			var index = new TokenStreamIndex(stream, start);
			var context = new RuleContextMock(index);
			var rule = CreateAOrBRule();

			rule.Grab(context).ToList();

			context.Result.IsSuccess.Should().Be(expectedMatch.Length == 1);
			context.ConcatTokens().Should().Be(expectedMatch);
		}

		private Choice CreateAOrBRule()
		{
			var ruleA = new SimpleRef(new Terminal("A", "A"));
			var ruleB = new SimpleRef(new Terminal("B", "B"));
			return new Choice("TEST", ruleA, ruleB);
		}
	}
}
