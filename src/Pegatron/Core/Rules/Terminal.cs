using System.Collections.Generic;

namespace Pegatron.Core.Rules
{
	public class Terminal : IRule
	{
		private readonly ITokenMatcher _matcher;
		private readonly string _defaultDisplayText;

		public string? Name { get; }
		public RuleType RuleType => RuleType.SingleMatch;

		public Terminal(string tokenType)
			: this(null, new TokenTypeMatcher(tokenType))
		{
		}

		public Terminal(string? name, string tokenType)
			: this(name, new TokenTypeMatcher(tokenType))
		{
		}

		public Terminal(string? name, ITokenMatcher matcher)
		{
			Name = name;
			_matcher = matcher;
			_defaultDisplayText = $"T<{_matcher.Name}>";
		}

		public IEnumerable<RuleOperation> Grab(IRuleContext ctx)
		{
			var token = ctx.Index.Get();
			if (_matcher.Match(token))
			{
				yield return ctx.Token(token);
				yield return ctx.Success(ctx.Index.Next());
			}
			else
			{
				yield return ctx.Failure();
			}
		}

		public string DisplayText(DisplayMode mode)
		{
			return _defaultDisplayText;
		}
	}
}
