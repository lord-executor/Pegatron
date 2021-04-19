using System.Collections.Generic;

namespace Pegatron.Core.Rules
{
	public class AnyTerminal : IRule
	{
		public string? Name { get; }

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

		public override string ToString()
		{
			return ".";
		}
	}
}
