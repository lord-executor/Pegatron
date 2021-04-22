using System;
using System.Collections.Generic;

namespace Pegatron
{
	public enum GrammarExceptionId
	{
		StartRuleNotDefined,
		GrammarContainsUnresolvedRule,
	}

	public class GrammarException : Exception
	{
		private static readonly IDictionary<GrammarExceptionId, string> _messages = new Dictionary<GrammarExceptionId, string>
		{
			[GrammarExceptionId.StartRuleNotDefined] = "The given grammar does not define a StartRule",
			[GrammarExceptionId.GrammarContainsUnresolvedRule] = "Cannot resolve rule {0}. It was never defined",
		};

		public GrammarExceptionId Id { get; }

		public GrammarException(GrammarExceptionId grammarExceptionId, params object[] args)
			: base(string.Format(_messages[grammarExceptionId], args))
		{
			Id = grammarExceptionId;
		}
	}
}
