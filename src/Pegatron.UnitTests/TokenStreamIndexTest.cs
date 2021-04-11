using FluentAssertions;
using NUnit.Framework;
using Pegatron.Core;
using Pegatron.UnitTests.Mocks;
using System;
using System.Linq;

namespace Pegatron.UnitTests
{
	/// <summary>
	/// These tests are almost the same as in <see cref="TokenStreamTest"/>, they just use the index API instead
	/// of directly accessing the token stream.
	/// </summary>
	[TestFixture]
	public class TokenStreamIndexTest
	{
		[Test]
		[TestCase(-1)]
		[TestCase(-12)]
		[TestCase(-42)]
		public void Index_WithNegativeNumber_Throws(int number)
		{
			var stream = new TokenStream(StaticLexer.FromWords());

			Action action = () => new TokenStreamIndex(stream, number);
			action.Should().Throw<ArgumentOutOfRangeException>();
		}

		[Test]
		public void Index_FromEmptyLexer_ContainsEosToken()
		{
			var index = new TokenStream(StaticLexer.FromWords()).Start();

			var eos = index.Get();
			eos.Should().NotBeNull();
			eos.IsEndOfStream.Should().BeTrue();

			index.Invoking(i => i.Next()).Should().Throw<IndexOutOfRangeException>();
		}

		[Test]
		[TestCase(0)]
		[TestCase(1)]
		[TestCase(10)]
		public void Stream_WithInvalidLexer_ThrowsMissingEosTokenException(int tokenCount)
		{
			var tokens = Enumerable.Repeat(Token.Dummy, tokenCount);
			var index = new TokenStream(new StaticLexer(tokens)).Start();

			for (var i = 0; i < tokenCount; i++)
			{
				index.Index.Should().Be(i);
				index.Get().Value.Should().Be(Token.Dummy.Value);
				index = index.Next();
			}

			index.Invoking(i => i.Get()).Should().Throw<LexerException>().Where(e => e.Id == LexerExceptionId.MissingEosToken);
		}

		[Test]
		public void Stream_FromSingleToken_ContainsTokenAndEos()
		{
			var token = "test";
			var index = new TokenStream(StaticLexer.FromWords(token)).Start();

			index.Get().Value.Should().Be(token);
			index.Get().IsEndOfStream.Should().BeFalse();
			index = index.Next();
			index.Get().IsEndOfStream.Should().BeTrue();
		}
	}
}
