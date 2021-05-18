using Pegatron.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Pegatron.UnitTests.Mocks
{
	[DebuggerDisplay(IRule.DebugExpression)]
	public class SimpleRef : IRuleRef
	{
		private readonly IRule _rule;

		public string RefName { get; }

		public string? Name => _rule.Name;
		public RuleType RuleType => _rule.RuleType;

		public SimpleRef(IRule rule)
			: this(null, rule)
		{ }

		public SimpleRef(string? refName, IRule rule)
		{
			RefName = refName ?? String.Empty;
			_rule = rule;
		}

		public IEnumerable<RuleOperation> Grab(IRuleContext ctx)
		{
			return _rule.Grab(ctx);
		}

		public string DisplayText(DisplayMode mode)
		{
			return _rule.ToDisplayText(mode);
		}

		public IRuleRef As(string refName)
		{
			throw new InvalidOperationException();
		}
	}
}
