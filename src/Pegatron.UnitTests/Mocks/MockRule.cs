using Pegatron.Core;
using System.Collections.Generic;
using System.Diagnostics;

namespace Pegatron.UnitTests.Mocks
{
	public enum MockRuleBehavior
	{
		Nothing,
		Success,
		Failure,
	}

	[DebuggerDisplay(IRule.DebugExpression)]
	public class MockRule : IRule
	{
		public string? Name { get; }
		public RuleType RuleType => RuleType.SingleMatch;
		public MockRuleBehavior Behavior { get; }
		public bool DidGrab { get; private set; }

		public MockRule(string name)
			: this(name, MockRuleBehavior.Nothing)
		{
		}

		public MockRule(string name, MockRuleBehavior behavior)
		{
			Name = name;
			Behavior = behavior;
		}

		public IEnumerable<RuleOperation> Grab(IRuleContext ctx)
		{
			if (Behavior == MockRuleBehavior.Success)
			{
				yield return ctx.Success(ctx.Index);
			}
			else if (Behavior == MockRuleBehavior.Failure)
			{
				yield return ctx.Failure();
			}
			DidGrab = true;
			yield break;
		}

		public string DisplayText(DisplayMode mode)
		{
			return Name ?? "UNNAMED";
		}
	}
}
