using Pegatron.Grammars.Peg.Ast;
using System;

namespace Pegatron.Grammars.Peg
{
	public class Terminal : IGrammarRule<INode>
	{
		public static string DefinitionText => "terminal  :=  terminalType | terminalLiteral | terminalAny";

		public void Register(IGrammarBuilder<INode> grammar)
		{
			grammar.Choice("terminal",
				grammar.Ref("terminalType"),
				grammar.Ref("terminalLiteral"),
				grammar.Ref("terminalAny")
			);
		}

		public INode Reduce(IRule rule, INodeContext<INode> page)
		{
			throw new NotImplementedException();
		}
	}
}
