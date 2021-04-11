using System.Collections.Generic;
using System.Linq;

namespace Pegatron.Core.Rules
{
	public class Sequence : IRule
	{
		public string Name { get; }
		public IEnumerable<IRuleRef> Rules { get; }

		public Sequence(string name, params IRuleRef[] rules)
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

		public override string ToString()
		{
			return Rules.Select(r => string.IsNullOrEmpty(r.Name) ? r.ToString() : r.Name).StrJoin(" ");
		}
	}
}
