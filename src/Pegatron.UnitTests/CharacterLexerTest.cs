using FluentAssertions;
using NUnit.Framework;
using Pegatron.Core;
using Pegatron.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pegatron.UnitTests
{
	[TestFixture]
	public class CharacterLexerTest
	{
		[Test]
		public void ReadTokens_FromEmptyString_IsEmpty()
		{
			var lexer = new CharacterLexer(String.Empty);

			lexer.ReadTokens().ToList().Should().ContainSingle()
				.Which.IsEndOfStream.Should().BeTrue();
		}

		[Test]
		public void ReadTokens_FromWord_ReturnsTokenForEachCharacter()
		{
			var word = "Hello World!";
			var lexer = new CharacterLexer(word);

			var tokens = lexer.ReadTokens().ToList();

			tokens.Count.Should().Be(word.Length + 1);
			tokens.Select(t => t.Value).StrJoin(String.Empty).Should().Be(word);
		}

		[Test]
		public void ReadTokens_WithTextContainingNewlines_NormalizesNewlines()
		{
			var word = "A\nB\r\nC\rD";
			var lexer = new CharacterLexer(word);

			var newlines = lexer.ReadTokens().Where(t => t.Type == nameof(Environment.NewLine)).ToList();

			newlines.Count.Should().Be(3);
			newlines.All(t => t.Value == "\n").Should().BeTrue();
		}
	}
}
