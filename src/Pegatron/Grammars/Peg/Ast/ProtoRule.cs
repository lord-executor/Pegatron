using Pegatron.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pegatron.Grammars.Peg.Ast
{
	public delegate IRule RuleFactory(ProtoRule rule);

	public class ProtoRule : INode
	{
		private static GrammarBuilderAdapter? _grammarBuilderAdapter = null;
		public string DisplayText { get; }
		public IList<ProtoRule> Children { get; }
		public string? RuleName { get; set; }
		public string? RefName { get; set; }
		public RuleFactory? Factory { get; }

		public ProtoRule(string type, RuleFactory ruleFactory)
		{
			DisplayText = type;
			Factory = ruleFactory;
			Children = new List<ProtoRule>();
		}

		public void AddChildren(IEnumerable<ProtoRule> children)
		{
			foreach (var child in children)
			{
				Children.Add(child);
			}
		}

		public IRuleRef Ref(string name)
		{
			ArgAssert.NotNull(nameof(_grammarBuilderAdapter), _grammarBuilderAdapter);
			return _grammarBuilderAdapter.GetRef(name);
		}

		public IRuleRef Single()
		{
			return Children.Single().Create();
		}

		public IEnumerable<IRuleRef> All()
		{
			return Children.Select(c => c.Create()).ToList();
		}

		private IRuleRef Create()
		{
			ArgAssert.NotNull(nameof(_grammarBuilderAdapter), _grammarBuilderAdapter);
			ArgAssert.NotNull(nameof(Factory), Factory);

			var rule = Factory(this);
			var ruleRef = rule is IRuleRef ? (IRuleRef)rule : _grammarBuilderAdapter.Define(rule);

			if (RefName != null)
			{
				ruleRef.As(RefName);
			}

			return ruleRef;
		}

		public IRuleRef<TNode> Create<TNode>(IGrammarBuilder<TNode> grammar)
		{
			_grammarBuilderAdapter = new GrammarBuilderAdapter(rule => grammar.DefineRule(rule.Name, rule), name => grammar.Ref(name));

			ArgAssert.NotNull(nameof(Factory), Factory);
			var rule = Factory(this);
			var ruleRef = grammar.DefineRule(rule.Name, rule);

			_grammarBuilderAdapter = null;

			if (RefName != null)
			{
				ruleRef.As(RefName);
			}

			return ruleRef;
		}

		private class GrammarBuilderAdapter
		{
			public Func<IRule, IRuleRef> Define { get; }
			public Func<string, IRuleRef> GetRef { get; }

			public GrammarBuilderAdapter(
				Func<IRule, IRuleRef> define,
				Func<string, IRuleRef> getRef
			) {
				Define = define;
				GetRef = getRef;
			}
		}
	}
}
