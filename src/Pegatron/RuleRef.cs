using Pegatron.Core;
using System;
using System.Collections.Generic;

namespace Pegatron
{
	public class RuleRef<TNode> : IRuleRef<TNode>
	{
		internal IRule? Rule { get; private set; }
		public string? Name => Rule?.Name;
		public RuleType RuleType => Rule?.RuleType ?? RuleType.SingleMatch;
		public string? RefName { get; private set; }
		public Reducer<TNode>? Reducer { get; private set; }
		public bool IsResolved => Rule != null;

		public RuleRef(IRule rule)
		{
			Rule = rule;
		}

		public RuleRef()
		{
		}

		public IEnumerable<RuleOperation> Grab(IRuleContext ctx)
		{
			if (!IsResolved)
			{
				throw new InvalidOperationException("Only fully resolved rule refs can be executed");
			}
			return Rule!.Grab(ctx);
		}

		public IRuleRef<TNode> As(string refName)
		{
			RefName = refName;
			return this;
		}

		public IRuleRef<TNode> ReduceWith(Reducer<TNode>? reducer)
		{
			Reducer = reducer;
			return this;
		}

		public void Resolve(RuleRef<TNode> parent)
		{
			Rule = parent.Rule;
			Reducer = Reducer ?? parent.Reducer;
		}

		/// <summary>
		/// Creates a new rule ref that points to the same underlying rule and has the same reducer as this one.
		/// </summary>
		public IRuleRef<TNode> CloneRule()
		{
			if (!IsResolved)
			{
				throw new InvalidOperationException("Only fully resolved rule refs can be cloned");
			}
			var refClone = new RuleRef<TNode>(Rule!);
			// we keep the same reducer, but the RefName is reset
			refClone.ReduceWith(Reducer);
			return refClone;
		}

		public string DisplayText(DisplayMode mode)
		{
			var ruleText = Rule?.ToDisplayText(mode) ?? IRuleRef.UndefinedRef;

			return string.IsNullOrEmpty(RefName) || RefName == Rule?.Name
				? ruleText
				: $"{ruleText} #{RefName}";
		}
	}
}
