using System.Collections.Generic;
using System.Diagnostics;

namespace Pegatron.Core.Rules
{
	[DebuggerDisplay(IRule.DebugExpression)]
	public class AnyTerminal : IRule
	{
		public string? Name { get; }
		public RuleType RuleType => RuleType.SingleMatch;

		public AnyTerminal(string? name)
		{
			Name = name;
		}

		public IEnumerable<RuleOperation> Grab(IRuleContext ctx)
		{
			var token = ctx.Index.Get();
			if (token.IsEndOfStream)
			{
				yield return ctx.Failure();
			}
			else
			{
				yield return ctx.Token(token);
				yield return ctx.Success(ctx.Index.Next());
			}
		}

		public string DisplayText(DisplayMode mode)
		{
			return ".";
		}
	}
}
