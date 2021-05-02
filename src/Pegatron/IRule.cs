using Pegatron.Core;
using System.Collections.Generic;
using System.Text;

namespace Pegatron
{
	public enum DisplayMode
	{
		Short,
		Long,
		Definition,
		Recursive,
	}

	public interface IRule
	{
		string? Name { get; }
		RuleType RuleType { get; }
		IEnumerable<RuleOperation> Grab(IRuleContext ctx);
		public string DisplayText(DisplayMode mode);
	}

	public static class RuleExtensions
	{
		public static string ToDisplayText(this IRule rule)
		{
			return rule.ToDisplayText(DisplayMode.Short);
		}

		public static string ToDisplayText(this IRule rule, DisplayMode mode)
		{
			var builder = new StringBuilder();
			var childMode = mode == DisplayMode.Recursive ? DisplayMode.Recursive : DisplayMode.Short;

			if (mode == DisplayMode.Definition && rule.Name != null)
			{
				builder.Append($"{rule.Name} := ");
			}
			if (mode == DisplayMode.Short && rule.Name != null)
			{
				builder.Append(rule.Name);
			}
			else
			{
				builder.Append(rule.DisplayText(childMode));
			}

			return builder.ToString();
		}
	}
}
