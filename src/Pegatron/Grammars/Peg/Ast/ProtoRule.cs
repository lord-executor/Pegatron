using Pegatron.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pegatron.Grammars.Peg.Ast
{
	public delegate IRuleRef<INode> RuleFactory(IGrammarBuilder<INode> grammar, ProtoRule rule);

	public class ProtoRule : INode
	{
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

		public IRuleRef<INode> Single(IGrammarBuilder<INode> grammar)
		{
			return Children.Single().Create(grammar);
		}

		public IEnumerable<IRuleRef<INode>> All(IGrammarBuilder<INode> grammar)
		{
			return Children.Select(c => c.Create(grammar));
		}

		public IRuleRef<INode> Create(IGrammarBuilder<INode> grammar)
		{
			ArgAssert.NotNull(nameof(Factory), Factory);
			return Factory(grammar, this);
		}
	}
}
