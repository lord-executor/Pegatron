using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Pegatron.Grammars.Peg
{
	public class Lexer : ILexer<Token>
	{
		private readonly TextReader _reader;
		private readonly IList<(Regex exp, Func<uint, Match, Token> factory)> _matchers;
		private readonly Regex _escapes = new Regex(@"\\(.)");

		public Lexer(TextReader reader)
		{
			_reader = reader;

			_matchers = new List<(Regex exp, Func<uint, Match, Token> factory)>
			{
				(new Regex(@"\G\s*(T)(?=<)"), (line, match) => TokenFromMatch(line, TokenType.Special, match)),
				(new Regex(@"\G\s*([a-zA-Z_][a-zA-Z0-9_]*)"), (line, match) => TokenFromMatch(line, TokenType.Identifier, match)),
				(new Regex(@"\G\s*(\d+)"), (line, match) => TokenFromMatch(line, TokenType.Number, match)),
				(new Regex(@"\G\s*'((?:([^']|(?<=\\)')+))'"), (line, match) => TokenFromMatch(line, TokenType.Literal, match)),
				(new Regex(@"\G\s*([|#?*+.,(){}<>&!]|:=)"), (line, match) => TokenFromMatch(line, TokenType.Special, match)),
			};
		}

		public IEnumerable<Token> ReadTokens()
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

			yield return Token.Eos;
		}

		private Token TokenFromMatch(uint line, TokenType type, Match match)
		{
			return new Token(type.ToString())
			{
				Value = _escapes.Replace(match.Groups[1].Value, m => m.Groups[1].Value),
				Line = line,
				Start = (uint)match.Groups[1].Index,
			};
		}
	}
}
