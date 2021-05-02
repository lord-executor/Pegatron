using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Pegatron.Core.Rules
{
	[DebuggerDisplay(IRule.DebugExpression)]
	public class Repeat : IRule
	{
		public string? Name { get; }
		public RuleType RuleType => RuleType.MultiMatch;
		public IRuleRef Rule { get; }
		private readonly int _min;
		private readonly int _max;

		public Repeat(string? name, IRuleRef rule, int min = 0, int max = -1)
		{
			Name = name;
			Rule = rule;
			_min = min;
			_max = max;
		}

		public IEnumerable<RuleOperation> Grab(IRuleContext ctx)
		{
			var current = ctx.Index;
			var count = 0;
			CoroutineResult<RuleResult> result;

			while (_max == -1 || count < _max + 1)
			{
				yield return ctx.Call(Rule, current, out result);
				if (!result.Value.IsSuccess)
				{
					if (count >= _min)
					{
						yield return ctx.Success(current);
						yield break;
					}
					else
					{
						yield return ctx.Failure();
						yield break;
					}
				}

				count++;
				current = result.Value.Index;
			}

			yield return ctx.Failure();
		}

		public string DisplayText(DisplayMode mode)
		{
			return $"{Rule.ToDisplayText(mode)}{{{_min},{(_max < 0 ? string.Empty : _max)}}}";
		}
	}
}
