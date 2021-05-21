using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Pegatron
{
	public class RegexPriorityLexer<TToken> : ILexer<TToken>
		where TToken : IToken
	{
		private readonly TextReader _reader;
		private readonly IList<(Regex exp, Func<uint, Match, TToken> factory)> _matchers;
		private readonly TToken _eosToken;

		public RegexPriorityLexer(TextReader reader, IList<(Regex exp, Func<uint, Match, TToken> factory)> matchers, TToken eosToken)
		{
			_reader = reader;
			_matchers = matchers;
			_eosToken = eosToken;
		}

		public IEnumerable<TToken> ReadTokens()
		{
			var line = _reader.ReadLine();
			uint lineCount = 0;
			int index;

			while (line != null)
			{
				lineCount++;
				index = 0;
				while (index < line.Length)
				{
					var startIndex = index;
					foreach (var matcher in _matchers)
					{
						var m = matcher.exp.Match(line, index);
						if (m.Success)
						{
							var token = matcher.factory(lineCount, m);
							yield return token;

							index = m.Index + m.Length;
							break;
						}
					}
					if (index == startIndex)
					{
						throw new LexerException(LexerExceptionId.UnrecognizedInput, lineCount, index, line);
					}
				}

				line = _reader.ReadLine();
			}

			yield return _eosToken;
		}
	}
}
