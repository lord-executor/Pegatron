using Pegatron.Grammars.Peg.Ast;
using System;
using System.Linq;

namespace Pegatron.Grammars.Peg
{
	public class Definition : IGrammarRule<INode>
	{
		public static string DefinitionText => "definition  :=  T<Identifier> #name ':=' choice #rule";

		public void Register(IGrammarBuilder<INode> grammar)
		{
			grammar.Sequence("definition",
				grammar.Terminal(TokenType.Identifier).As("name"),
				grammar.TerminalValue(":="),
				grammar.Ref("choice").As("rule")
			).ReduceWith(Reduce);
		}

		public INode Reduce(IRule rule, INodeContext<INode> page)
		{
			var name = page.Get("name").Single<Value>().Text;
			var definition = page.Get("rule").Single<ProtoRule>();
			definition.RuleName = name;

			RefNamePropagation(definition);

			return definition;
		}

		private string? RefNamePropagation(ProtoRule rule, int level = 0)
		{
			foreach (var child in rule.Children)
			{
				var name = RefNamePropagation(child, level + 1);
				if (name != null)
				{
					if (child.RefName != null)
					{
						throw new Exception($"Cannot propagate ref name inside of named RuleRef. {child.DisplayText} is already named {child.RefName}");
					}

					child.RefName = name;
				}
			}

			if (level > 0)
			{
				var namedChildren = rule.Children.Where(r => r.RefName != null).ToList();
				if (namedChildren.Select(c => c.RefName).Distinct().Count() > 1)
				{
					throw new Exception($"Cannot propagate multiple distinct ref names inside of {rule.DisplayText}");
				}
				else if (namedChildren.Count > 0)
				{
					var name = namedChildren.First().RefName;
					if (name != IRuleRef.LiftRefName)
					{
						foreach (var child in namedChildren)
						{
							child.RefName = IRuleRef.LiftRefName;
						}
						return name;
					}
				}
			}

			return null;
		}
	}
}
