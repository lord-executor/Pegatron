using Pegatron.Core;
using System.Collections.Generic;

namespace Pegatron
{
	public interface IRule
	{
		string? Name { get; }
		IEnumerable<RuleOperation> Grab(IRuleContext ctx);

		public string DisplayText => Name ?? ToString()!;
	}

	public static class RuleExtensions
	{
		public static string ToDisplayText(this IRule rule) => rule.DisplayText;
	}
}
