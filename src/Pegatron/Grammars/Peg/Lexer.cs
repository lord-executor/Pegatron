using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Pegatron.Grammars.Peg
{
	public class Lexer : RegexPriorityLexer<Token>
	{
		private static readonly IList<(Regex exp, Func<uint, Match, Token> factory)> _lexerExpressions = new List<(Regex exp, Func<uint, Match, Token> factory)>
			{
				(new Regex(@"\G\s*(T)(?=<)"), (line, match) => TokenFromMatch(line, TokenType.Special, match)),
				(new Regex(@"\G\s*([a-zA-Z_][a-zA-Z0-9_]*)"), (line, match) => TokenFromMatch(line, TokenType.Identifier, match)),
				(new Regex(@"\G\s*(\d+)"), (line, match) => TokenFromMatch(line, TokenType.Number, match)),
				(new Regex(@"\G\s*('(?:([^']|(?<=\\)')+)')"), (line, match) => TokenFromMatch(line, TokenType.Literal, match)),
				(new Regex(@"\G\s*(:=|#!|[|#?*+.,(){}<>&!])"), (line, match) => TokenFromMatch(line, TokenType.Special, match)),
			};

		private static readonly Regex _escapes = new Regex(@"\\(.)");

		public Lexer(TextReader reader)
			: base(reader, _lexerExpressions, Token.Eos)
		{
		}

		private static Token TokenFromMatch(uint line, TokenType type, Match match)
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
