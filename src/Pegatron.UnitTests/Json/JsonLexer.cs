using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Pegatron.UnitTests.Json
{
	public class JsonLexer : ILexer<Token>
	{
		private static readonly IList<(Regex exp, Func<uint, Match, Token> factory)> _matchers = new List<(Regex exp, Func<uint, Match, Token> factory)>
			{
				(new Regex(@"\G\s*("".*?(?<!\\)"")", RegexOptions.Compiled), (line, match) => TokenFromMatch(line, JsonTokenType.String, match)),
				(new Regex(@"\G\s*(true|false)", RegexOptions.Compiled), (line, match) => TokenFromMatch(line, JsonTokenType.Boolean, match)),
				(new Regex(@"\G\s*(null)", RegexOptions.Compiled), (line, match) => TokenFromMatch(line, JsonTokenType.Null, match)),
				(new Regex(@"\G\s*((?:\+|-)?\d+(?:\.\d+)?(?:(e|E)(\+|-)?\d+)?)", RegexOptions.Compiled), (line, match) => TokenFromMatch(line, JsonTokenType.Number, match)),
				(new Regex(@"\G\s*([{}[\]:,])", RegexOptions.Compiled), (line, match) => TokenFromMatch(line, JsonTokenType.Special, match)),
			};

		private static readonly Regex _escapes = new Regex(@"\\([""\\/])", RegexOptions.Compiled);

		private readonly TextReader _reader;

		public JsonLexer(string text)
			: this(new StringReader(text))
		{
		}

		public JsonLexer(TextReader reader)
		{
			_reader = reader;
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

		private static Token TokenFromMatch(uint line, JsonTokenType type, Match match)
		{
			return new Token(type.ToString())
			{
				Value = _escapes.Replace(match.Groups[1].Value, "$1"),
				Line = line,
				Start = (uint)match.Groups[1].Index,
			};
		}
	}
}
