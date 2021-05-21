using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Pegatron.Grammars.Math
{
	public class Lexer : RegexPriorityLexer<Token>
	{
		private static readonly IList<(Regex exp, Func<uint, Match, Token> factory)> _lexerExpressions = new List<(Regex exp, Func<uint, Match, Token> factory)>
			{
				(new Regex(@"\G\s*([a-zA-Z]+)"), (line, match) => TokenFromMatch(line, TokenType.Identifier, match)),
				(new Regex(@"\G\s*(\d+(\.\d+)?)"), (line, match) => TokenFromMatch(line, TokenType.Number, match)),
				(new Regex(@"\G\s*(\*|/)"), (line, match) => TokenFromMatch(line, TokenType.MultiplicativeOperator, match)),
				(new Regex(@"\G\s*(\+|-)"), (line, match) => TokenFromMatch(line, TokenType.AdditiveOperator, match)),
				(new Regex(@"\G\s*(\()"), (line, match) => TokenFromMatch(line, TokenType.ParenLeft, match)),
				(new Regex(@"\G\s*(\))"), (line, match) => TokenFromMatch(line, TokenType.ParenRight, match)),
			};

		public Lexer(TextReader reader)
			: base(reader, _lexerExpressions, Token.Eos)
		{
		}

		private static Token TokenFromMatch(uint line, TokenType type, Match match)
		{
			return new Token(type.ToString())
			{
				Value = match.Groups[1].Value,
				Line = line,
				Start = (uint)match.Groups[1].Index,
			};
		}
	}
}
