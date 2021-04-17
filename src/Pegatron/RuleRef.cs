using Pegatron.Core;
using System;
using System.Collections.Generic;

namespace Pegatron
{
	public class RuleRef<TNode> : IRuleRef<TNode>
	{
		private IRule? _rule;
		public string Name => _rule?.Name ?? "UNRESOLVED";
		public string? RefName { get; private set; }
		public Reducer<TNode>? Reducer { get; private set; }
		public bool IsResolved => _rule != null;

		public RuleRef(IRule rule)
		{
			_rule = rule;
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
			return _rule!.Grab(ctx);
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
			_rule = parent._rule;
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
			var refClone = new RuleRef<TNode>(_rule!);
			// we keep the same reducer, but the RefName is reset
			refClone.ReduceWith(Reducer);
			return refClone;
		}

		public override string? ToString()
		{
			if (_rule == null)
			{
				return Name;
			}

			return string.IsNullOrEmpty(RefName)
				? _rule.ToString()
				: $"({_rule}) => {RefName}";
		}
	}
}
