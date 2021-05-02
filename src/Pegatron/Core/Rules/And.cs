using System.Collections.Generic;

namespace Pegatron.Core.Rules
{
	public class And : IRule
	{
		public string? Name { get; }
		public RuleType RuleType => RuleType.SingleMatch;
		public IRuleRef Target { get; }

		public And(string? name, IRuleRef target)
		{
			Name = name;
			Target = target;
		}

		public IEnumerable<RuleOperation> Grab(IRuleContext ctx)
		{
			yield return ctx.Call(Target, ctx.Index, out var result);
			if (result.Value.IsSuccess)
			{
				yield return ctx.Success(ctx.Index);
			}
			else
			{
				yield return ctx.Failure();
			}
		}

		public string DisplayText(DisplayMode mode)
		{
			return $"&{Target.ToDisplayText(mode)}";
		}
	}
}
