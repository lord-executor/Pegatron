using Pegatron.Grammars.Peg.Ast;
using System;

namespace Pegatron.Grammars.Peg
{
	public class RangeDef : IGrammarRule<INode>
	{
		public static string DefinitionText => "range  :=  ('*' | '+' | '?' | minmax)";

		public void Register(IGrammarBuilder<INode> grammar)
		{
			grammar.Choice("range",
				grammar.TerminalValue("*"),
				grammar.TerminalValue("+"),
				grammar.TerminalValue("?"),
				grammar.Ref("minmax")
			).ReduceWith(Reduce);
		}

		public INode Reduce(IRule rule, INodeContext<INode> page)
		{
			var node = page.Get(0);
			if (node is Ast.Range)
			{
				return node;
			}

			var special = ((Value)node).Text;

			switch (special)
			{
				case "*":
					return new Ast.Range(0, -1);
				case "+":
					return new Ast.Range(1, -1);
				case "?":
					return new Ast.Range(0, 1);
				default:
					throw new InvalidOperationException();
			}
		}
	}
}
