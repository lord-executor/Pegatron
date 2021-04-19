using System;
using System.Collections.Generic;

namespace Pegatron.Core.Rules
{
	public class Terminal : IRule
	{
		private readonly Predicate<IToken> _matcher;
		private readonly string _defaultDisplayText;

		public string? Name { get; }

		public Terminal(string tokenType)
			: this(null, tokenType, token => token.Type == tokenType)
		{
		}

		public Terminal(string name, string tokenType)
			: this(name, tokenType, token => token.Type == tokenType)
		{
		}

		public Terminal(string? name, string? terminalName, Predicate<IToken> matcher)
		{
			Name = name;
			_defaultDisplayText = $"T<{terminalName ?? "?"}>";
			_matcher = matcher;
		}

		public IEnumerable<RuleOperation> Grab(IRuleContext ctx)
		{
			var token = ctx.Index.Get();
			if (_matcher(token))
			{
				yield return ctx.Token(token);
				yield return ctx.Success(ctx.Index.Next());
			}
			else
			{
				yield return ctx.Failure();
			}
		}

		public override string ToString()
		{
			return _defaultDisplayText;
		}
	}
}
