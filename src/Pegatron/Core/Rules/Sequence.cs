using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Pegatron.Core.Rules
{
	[DebuggerDisplay(IRule.DebugExpression)]
	public class Sequence : IRule
	{
		public string? Name { get; }
		public RuleType RuleType => RuleType.MultiMatch;
		public IEnumerable<IRuleRef> Rules { get; }

		public Sequence(string? name, IEnumerable<IRuleRef> rules)
		{
			Name = name;
			Rules = rules;
		}

		public IEnumerable<RuleOperation> Grab(IRuleContext ctx)
		{
			var current = ctx.Index;

			foreach (var r in Rules)
			{
				yield return ctx.Call(r, current, out var result);
				if (!result.Value.IsSuccess)
				{
					yield return ctx.Failure();
					yield break;
				}

				current = result.Value.Index;
			}

			yield return ctx.Success(current);
		}

		public string DisplayText(DisplayMode mode)
		{
			return $"({Rules.Select(r => r.ToDisplayText(mode)).StrJoin(" ")})";
		}
	}
}
