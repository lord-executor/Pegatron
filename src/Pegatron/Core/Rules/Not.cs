using System.Collections.Generic;

namespace Pegatron.Core.Rules
{
	public class Not : IRule
	{
		public string? Name { get; }
		public RuleType RuleType => RuleType.SingleMatch;
		public IRuleRef Target { get; }

		public Not(string? name, IRuleRef target)
		{
			Name = name;
			Target = target;
		}

		public IEnumerable<RuleOperation> Grab(IRuleContext ctx)
		{
			yield return ctx.Call(Target, ctx.Index, out var result);
			if (result.Value.IsSuccess)
			{
				yield return ctx.Failure();
			}
			else
			{
				yield return ctx.Success(ctx.Index);
			}
		}

		public override string ToString()
		{
			return $"!{Target.DisplayText}";
		}
	}
}
