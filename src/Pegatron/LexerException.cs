using System;
using System.Collections.Generic;

namespace Pegatron
{
	public enum LexerExceptionId
	{
		MissingEosToken,
		UnrecognizedInput,
	}

	public class LexerException : Exception
	{
		private static readonly IDictionary<LexerExceptionId, string> _messages = new Dictionary<LexerExceptionId, string>
		{
			[LexerExceptionId.MissingEosToken] = "Lexer token stream terminated without returning an EOS token as its last result",
			[LexerExceptionId.UnrecognizedInput] = "No expression matched the end of the string from line {0} position {1} - '{2}'",
		};

		public LexerExceptionId Id { get; }

		public LexerException(LexerExceptionId lexerExceptionId, params object[] args)
			: base(string.Format(_messages[lexerExceptionId], args))
		{
			Id = lexerExceptionId;
		}
	}
}
