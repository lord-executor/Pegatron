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
				if (level > 0 && name != null)
				{
					if (rule.RefName != null)
					{
						throw new Exception($"Cannot propagate ref name inside of named RuleRef. {rule.DisplayText} is already named {rule.RefName}");
					}

					rule.RefName = name;
				}
			}

			if (level > 0)
			{
				var namedChildren = rule.Children.Where(r => r.RefName != null).ToList();
				if (namedChildren.Count > 1)
				{
					throw new Exception($"Cannot propagate multiple ref names inside of {rule.DisplayText}");
				}

				if (namedChildren.Count == 1)
				{
					var name = namedChildren.Single().RefName;
					if (name != IRuleRef.LiftRefName)
					{
						namedChildren.Single().RefName = IRuleRef.LiftRefName;
						return name;
					}
				}
			}

			return null;
		}
	}
}
