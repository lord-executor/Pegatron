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
	public class AnyTerminalTest
	{
		[Test]
		public void AnyTerminal_ToString_ReturnsCorrectDisplayText()
		{
			var rule = new AnyTerminal("TEST");

			rule.Name.Should().Be("TEST");
			rule.ToString().Should().Be(".");
		}

		[Test]
		[TestCase("")]
		[TestCase("ABBA")]
		[TestCase("DEADBEEF")]
		[TestCase(".🤣$£𝜋")]
		public void AnyTerminal_WithAnyCharacter_Succeeds(string text)
		{
			var stream = new TokenStream(new CharacterLexer(text));
			var rule = new AnyTerminal("TEST");
			TokenStreamIndex index;
			RuleContextMock context;

			for (int i = 0; i < text.Length; i++)
			{
				index = new TokenStreamIndex(stream, i);
				context = new RuleContextMock(index);

				rule.Evaluate(context);

				context.Result.IsSuccess.Should().BeTrue();
				context.Result.Index.Index.Should().Be(i + 1);
				context.ConcatTokens().Should().Be(text.Substring(i, 1));
			}

			index = new TokenStreamIndex(stream, text.Length);
			context = new RuleContextMock(index);

			rule.Evaluate(context);

			context.Result.IsSuccess.Should().BeFalse();
			context.Result.Index.Index.Should().Be(index.Index);
			context.ConcatTokens().Should().Be(String.Empty);
		}
	}
}
