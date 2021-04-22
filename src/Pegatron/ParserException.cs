using System;
using System.Collections.Generic;

namespace Pegatron
{

	public enum ParserExceptionId
	{
		StartRuleMissingResult,
		MissingResult,
		ParsingFailed,
		PartialMatch,
		ReducerError,
	}

	public class ParserException : Exception
	{
		private static readonly IDictionary<ParserExceptionId, string> _messages = new Dictionary<ParserExceptionId, string>
		{
			[ParserExceptionId.StartRuleMissingResult] = "The grammar start rule does not have a result after parsing - it likely didn't complete properly",
			[ParserExceptionId.ParsingFailed] = "Failed to parse the given token stream; rule matching was unsuccessful",
			[ParserExceptionId.PartialMatch] = "Rule matched, but there are still tokens left in the stream at {0}",
			[ParserExceptionId.ReducerError] = "Expecting start rule call to result in a single node but found {0} nodes",
			[ParserExceptionId.MissingResult] = "Rule {0} did not return a result",
		};

		public ParserExceptionId Id { get; }

		public ParserException(ParserExceptionId ParserExceptionId, params object[] args)
			: base(string.Format(_messages[ParserExceptionId], args))
		{
			Id = ParserExceptionId;
		}
	}
}
