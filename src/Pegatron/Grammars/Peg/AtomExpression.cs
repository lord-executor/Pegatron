using Pegatron.Grammars.Peg.Ast;
using System;
using System.Collections.Generic;

namespace Pegatron.Grammars.Peg
{
	public class AtomExpression : IGrammarRule<INode>
	{
		public static string DefinitionText => "atomExpression  :=  ('&' | '!')? #prefix atom range? #suffix";

		private static readonly IDictionary<string, Func<ProtoRule>> _prefixMap = new Dictionary<string, Func<ProtoRule>>
		{
			["&"] = () => new ProtoRule(nameof(Core.Rules.And), (grammar, rule) => grammar.And(rule.RuleName, rule.Single(grammar))),
			["!"] = () => new ProtoRule(nameof(Core.Rules.Not), (grammar, rule) => grammar.Not(rule.RuleName, rule.Single(grammar))),
		};

		public void Register(IGrammarBuilder<INode> grammar)
		{
			grammar.Sequence("atomExpression",
				grammar.Optional(null,
					grammar.Choice(null,
						grammar.TerminalValue("&"),
						grammar.TerminalValue("!")
					)
				).As("prefix"),
				grammar.Ref("atom"),
				grammar.Optional(null, grammar.Ref("range")).As("suffix")
			).ReduceWith(Reduce);
		}

		public INode Reduce(IRule rule, INodeContext<INode> page)
		{
			var prefix = page.Get("prefix").Optional<Value>()?.Text;
			var range = page.Get("suffix").Optional<Ast.Range>();
			var atom = page.Get("atom").Single<ProtoRule>();
			var result = atom;

			if (range != null)
			{
				result = new ProtoRule(nameof(Core.Rules.Repeat), (grammar, rule) => grammar.Repeat(rule.RuleName, rule.Single(grammar), range.Min, range.Max));
				result.Children.Add(atom);
				atom = result;
			}

			if (prefix != null)
			{
				result = _prefixMap[prefix]();
				result.Children.Add(atom);
			}

			return result;
		}
	}
}
