using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Pegatron.UnitTests.Json
{
	public class JsonLexer : RegexPriorityLexer<Token>
	{
		private static readonly IList<(Regex exp, Func<uint, Match, Token> factory)> _lexerExpressions = new List<(Regex exp, Func<uint, Match, Token> factory)>
			{
				(new Regex(@"\G\s*("".*?(?<!\\)"")", RegexOptions.Compiled), (line, match) => TokenFromMatch(line, JsonTokenType.String, match)),
				(new Regex(@"\G\s*(true|false)", RegexOptions.Compiled), (line, match) => TokenFromMatch(line, JsonTokenType.Boolean, match)),
				(new Regex(@"\G\s*(null)", RegexOptions.Compiled), (line, match) => TokenFromMatch(line, JsonTokenType.Null, match)),
				(new Regex(@"\G\s*((?:\+|-)?\d+(?:\.\d+)?(?:(e|E)(\+|-)?\d+)?)", RegexOptions.Compiled), (line, match) => TokenFromMatch(line, JsonTokenType.Number, match)),
				(new Regex(@"\G\s*([{}[\]:,])", RegexOptions.Compiled), (line, match) => TokenFromMatch(line, JsonTokenType.Special, match)),
			};

		private static readonly Regex _escapes = new Regex(@"\\([""\\/])", RegexOptions.Compiled);

		public JsonLexer(string text)
			: this(new StringReader(text))
		{
		}

		public JsonLexer(TextReader reader)
			: base(reader, _lexerExpressions, Token.Eos)
		{
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
