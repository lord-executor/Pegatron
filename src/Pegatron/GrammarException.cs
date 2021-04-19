using System;
using System.Collections.Generic;

namespace Pegatron
{
	public enum GrammarExceptionId
	{
		StartRuleNotDefined,
	}

	public class GrammarException : Exception
	{
		private static readonly IDictionary<GrammarExceptionId, string> _messages = new Dictionary<GrammarExceptionId, string>
		{
			[GrammarExceptionId.StartRuleNotDefined] = "The given grammar does not define a StartRule",
		};

		public GrammarExceptionId Id { get; }

		public GrammarException(GrammarExceptionId grammarExceptionId) : base(_messages[grammarExceptionId])
		{
			Id = grammarExceptionId;
		}
	}
}
