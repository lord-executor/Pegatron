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
			rule.ToDisplayText().Should().Be("TEST");
			rule.ToDisplayText(DisplayMode.Long).Should().Be("(T<A> | T<B>)");
		}

		[Test]
		[TestCase("", 0, "")]
		[TestCase("A", 0, "A")]
		[TestCase("B", 0, "B")]
		[TestCase("C", 0, "")]
		[TestCase("XXABXX", 2, "A")]
		[TestCase("XXABXX", 3, "B")]
		public void AOrB_GivenParseTextAndStart_BehavesCorrectly(string text, int start, string expectedMatch)
		{
			var stream = new TokenStream(new CharacterLexer(text));
			var index = new TokenStreamIndex(stream, start);			
			var rule = CreateAOrBRule();

			var opsMock = index.OperationsMock().Evaluate(rule);

			opsMock.Result.IsSuccess.Should().Be(expectedMatch.Length == 1);
			opsMock.Result.Index.Index.Should().Be(opsMock.Result.IsSuccess ? start + 1 : start);
			opsMock.ConcatTokens().Should().Be(expectedMatch);
		}

		private Choice CreateAOrBRule()
		{
			var ruleA = new SimpleRef(new Terminal("A"));
			var ruleB = new SimpleRef(new Terminal("B"));
			return new Choice("TEST", EnumSequence.Of(ruleA, ruleB));
		}
	}
}
