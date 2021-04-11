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
    public class TokenStreamTest
    {
		[Test]
		public void Stream_FromEmptyLexer_ContainsEosToken()
		{
			var stream = new TokenStream(StaticLexer.FromWords());

			var eos = stream.Get(0);
			eos.Should().NotBeNull();
			eos.IsEndOfStream.Should().BeTrue();

			stream.Invoking(s => s.Get(1)).Should().Throw<IndexOutOfRangeException>();
		}

		[Test]
		[TestCase(0)]
		[TestCase(1)]
		[TestCase(10)]
		public void Stream_WithInvalidLexer_ThrowsMissingEosTokenException(int tokenCount)
		{
			var tokens = Enumerable.Repeat(_dummy, tokenCount);
			var stream = new TokenStream(new StaticLexer(tokens));

			if (tokenCount > 0)
			{
				stream.Get(0).Value.Should().Be(_dummy.Value);
				stream.Get(tokenCount - 1).Value.Should().Be(_dummy.Value);
			}

			// the exception is only thrown once the end of the lexer tokens has been reached
			stream.Invoking(s => s.Get(tokenCount)).Should().Throw<LexerException>().Where(e => e.Id == LexerExceptionId.MissingEosToken);
		}

		[Test]
		public void Stream_FromSingleToken_ContainsTokenAndEos()
		{
			var token = "test";
			var stream = new TokenStream(StaticLexer.FromWords(token));

			stream.Get(0).Value.Should().Be(token);
			stream.Get(0).IsEndOfStream.Should().BeFalse();
			stream.Get(1).IsEndOfStream.Should().BeTrue();
		}

		[Test]
		[TestCase(new[] { 0, 1, 2 }, new[] { "first", "second", "third" })]
		[TestCase(new[] { 4, 1, 4 }, new[] { null, "second", null })]
		[TestCase(new[] { 4, 3, 1 }, new[] { null, "fourth", "second" })]
		public void StreamAccess_WithMultipleIndices_EnumeratesSourceAtMostOnce(IEnumerable<int> accessIndices, IEnumerable<string?> expectedTokens)
		{
			var source = new TokenSource();
			var stream = new TokenStream(new StaticLexer(source.GetTokens()));
			var tokens = new List<string?>();
			var maxIndex = 0;

			foreach (var i in accessIndices)
			{
				tokens.Add(stream.Get(i).Value);
				maxIndex = Math.Max(maxIndex, i);
			}

			source.EnumerationCount.Should().Be(1);
			source.MaxStep.Should().Be(maxIndex + 1);
			tokens.Should().BeEquivalentTo(expectedTokens);
		}

		private readonly Token _dummy = new Token("dummy")
		{
			Value = "dummy",
			Line = 0,
			Start = 0,
		};

		/// <summary>
		/// This token source is used to verify that the token stream buffers all of the tokens that it reads and
		/// never reads the lexer tokens more than once.
		/// </summary>
		private class TokenSource
		{
			public int EnumerationCount { get; private set; }
			public int MaxStep { get; private set; }

			public IEnumerable<Token> GetTokens()
			{
				EnumerationCount++;

				MaxStep++;
				yield return new Token("source")
				{
					Value = "first",
					Line = 0,
					Start = 0,
				};

				MaxStep++;
				yield return new Token("source")
				{
					Value = "second",
					Line = 0,
					Start = 20,
				};

				MaxStep++;
				yield return new Token("source")
				{
					Value = "third",
					Line = 1,
					Start = 5,
				};

				MaxStep++;
				yield return new Token("source")
				{
					Value = "fourth",
					Line = 1,
					Start = 42,
				};

				MaxStep++;
				yield return Token.Eos;
			}
		}
	}
}
