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
	public class TokenStreamSpanTest
	{
		[Test]
		public void AnySpan_OfTokenStream_MatchesRangeInOriginalSource()
		{
			var stream = CreateStream();

			for (var start = 0; start < _words.Count; start++)
			{
				for (var end = start + 1; end < _words.Count + 1; end++)
				{
					var span = new TokenStreamIndex(stream, start).Until(new TokenStreamIndex(stream, end));
					span.Select(t => t.Value).Should().BeEquivalentTo(_words.Skip(start).Take(end - start));
				}
			}
		}

		[Test]
		[TestCase(5, 15)]
		[TestCase(12, 14)]
		[TestCase(15, 42)]
		public void InvalidRangeSpan_WithValidStream_ThrowsIndexOutOfRangeException(int start, int end)
		{
			var stream = CreateStream();
			var span = new TokenStreamIndex(stream, start).Until(new TokenStreamIndex(stream, end));

			Action action = () => span.ToList();
			action.Should().Throw<IndexOutOfRangeException>();
		}

		[Test]
		[TestCase(2, 1)]
		[TestCase(5, 0)]
		public void Span_WithEndAtOrBeforeStart_ThrowsIndexOutOfRangeException(int start, int end)
		{
			var stream = CreateStream();

			Action action = () => new TokenStreamIndex(stream, start).Until(new TokenStreamIndex(stream, end));
			action.Should().Throw<IndexOutOfRangeException>();
		}

		[Test]
		public void EmptySpan_Contains_NoTokens()
		{
			var stream = CreateStream();
			var spans = new[] {
				TokenStreamSpan.Empty,
				stream.Start().Until(new TokenStreamIndex(stream, 0)),
				new TokenStreamIndex(stream, 20).Until(new TokenStreamIndex(stream, 20)),
			};

			foreach (var empty in spans)
			{
				empty.Should().BeEmpty();
			}
		}

		private readonly IList<string> _words = new List<string>
		{
			"Karmus Broodblade",
			"Barkom Loudheart",
			"Emmun Maddelver",
			"Therren Stoutbreath",
			"Melnar Stoutbeard",
			"Garrum Bronzestorm",
			"Armadin Renkenem",
			"Benthrum Rozzort",
			"Ebbrek Cezzan",
			"Bhalkam Graldan",
			"Darnir Godevur",
			"Thyrigg Debragann",
		};

		private TokenStream CreateStream()
		{
			var lexer = StaticLexer.FromWords(_words);
			return new TokenStream(lexer);
		}
	}
}
