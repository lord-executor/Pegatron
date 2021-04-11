using System;
using System.Collections.Generic;

namespace Pegatron
{
	public enum LexerExceptionId
	{
		MissingEosToken,
	}

	public class LexerException : Exception
	{
		private static readonly IDictionary<LexerExceptionId, string> _messages = new Dictionary<LexerExceptionId, string>
		{
			[LexerExceptionId.MissingEosToken] = "Lexer token stream terminated without returning an EOS token as its last result",
		};

		public LexerExceptionId Id { get; }

		public LexerException(LexerExceptionId lexerExceptionId) : base(_messages[lexerExceptionId])
		{
			Id = lexerExceptionId;
		}
	}
}
