using System;
using System.Collections.Generic;
using System.Linq;

namespace Pegatron.UnitTests.Mocks
{
	public class StaticLexer : ILexer<Token>
	{
		private readonly IEnumerable<Token> _tokens;

		public StaticLexer(IEnumerable<Token> tokens)
		{
			_tokens = tokens;
		}

		public IEnumerable<Token> ReadTokens()
		{
			return _tokens;
		}

		public static StaticLexer FromWords(params string[] words)
		{
			return new StaticLexer(words.Select((word, index) => new Token("word")
			{
				Value = word,
				Line = 0,
				Start = (uint)index,
			}).Concat(Enumerable.Repeat(Token.Eos, 1)));
		}
	}
}
