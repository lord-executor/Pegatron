using System.Collections.Generic;
using System.Linq;

namespace Pegatron.Core.Rules
{
	public class Choice : IRule
	{
		public string? Name { get; }
		public RuleType RuleType => RuleType.SingleMatch;
		public IEnumerable<IRuleRef> Rules { get; }

		public Choice(string? name, params IRuleRef[] rules)
		{
			Name = name;
			Rules = rules;
		}

		public IEnumerable<RuleOperation> Grab(IRuleContext ctx)
		{
			var current = ctx.Index;
			CoroutineResult<RuleResult> result;

			foreach (var rule in Rules)
			{
				yield return ctx.Call(rule, current, out result);
				if (result.Value.IsSuccess)
				{
					yield return ctx.Success(result.Value.Index);
					yield break;
				}

				current = result.Value.Index;
			}

			yield return ctx.Failure();
		}

		public override string ToString()
		{
			return $"({Rules.Select(r => r.DisplayText).StrJoin(" | ")})";
		}
	}
}
