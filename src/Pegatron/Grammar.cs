using System;
using System.Collections.Generic;

namespace Pegatron
{
	public abstract class Grammar<TNode> : IGrammar<TNode>, IGrammarBuilder<TNode>
	{
		protected readonly IDictionary<string, RuleRef<TNode>> _definedRules = new Dictionary<string, RuleRef<TNode>>();
		protected readonly IDictionary<string, IList<RuleRef<TNode>>> _unresolvedRules = new Dictionary<string, IList<RuleRef<TNode>>>();
		public IRuleRef<TNode>? StartRule { get; private set; }

		public IRuleRef<TNode> DefineRule(string? name, IRule rule)
		{
			var ruleRef = new RuleRef<TNode>(rule);
			if (!string.IsNullOrEmpty(name))
			{
				if (_definedRules.ContainsKey(name))
				{
					throw new InvalidOperationException($"Cannot define a rule with the same name twice: {name}");
				}
				_definedRules[name] = ruleRef;
			}

			return ruleRef;
		}

		public IRuleRef<TNode> Ref(string name)
		{
			if (_definedRules.ContainsKey(name))
			{
				return _definedRules[name].CloneRule().As(name);
			}

			if (!_unresolvedRules.ContainsKey(name))
			{
				_unresolvedRules[name] = new List<RuleRef<TNode>>();
			}

			var ruleRef = new RuleRef<TNode>();
			_unresolvedRules[name].Add(ruleRef);
			return ruleRef.As(name);
		}

		public void StartWith(string name)
		{
			foreach (var kvp in _unresolvedRules)
			{
				if (!_definedRules.ContainsKey(kvp.Key))
				{
					throw new GrammarException(GrammarExceptionId.GrammarContainsUnresolvedRule, kvp.Key);
				}

				var ruleRef = _definedRules[kvp.Key];
				foreach (var unresolved in kvp.Value)
				{
					unresolved.Resolve(ruleRef);
				}
			}
			_unresolvedRules.Clear();

			StartRule = Ref(name);
		}

		public abstract IEnumerable<TNode> TerminalReducer(IRule rule, IToken token);

		public abstract IEnumerable<TNode> DefaultReducer(IRule rule, INodeContext<TNode> page);
	}
}
