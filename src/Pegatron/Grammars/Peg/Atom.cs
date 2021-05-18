using Pegatron.Grammars.Peg.Ast;
using System;

namespace Pegatron.Grammars.Peg
{
	public class Atom : IGrammarRule<INode>
	{
		public static string DefinitionText => "atom  :=  ruleRef | terminal | '(' choice #! ')'";

		public void Register(IGrammarBuilder<INode> grammar)
		{
			grammar.Choice("atom",
				grammar.Ref("ruleRef"),
				grammar.Ref("terminal"),
				grammar.Sequence(null, grammar.TerminalValue("("), grammar.Ref("choice").Lift(), grammar.TerminalValue(")"))
			);
		}
	}
}
