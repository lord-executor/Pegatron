using Pegatron.Grammars.Peg.Ast;

namespace Pegatron.Grammars.Peg
{
	public class TerminalLiteral : IGrammarRule<INode>
	{
		public static string DefinitionText => "terminalLiteral  :=  T<Literal> | 'T' '<' T<Literal> #! '>'";

		public void Register(IGrammarBuilder<INode> grammar)
		{
			grammar.Choice("terminalLiteral",
				grammar.Terminal(TokenType.Literal),
				grammar.Sequence(null,
					grammar.TerminalValue("T"),
					grammar.TerminalValue("<"),
					grammar.Terminal(TokenType.Literal).Lift(),
					grammar.TerminalValue(">")
				)
			).ReduceWith(Reduce);
		}

		public INode Reduce(IRule rule, INodeContext<INode> page)
		{
			var value = ((Value)page.Get(0)).Text ?? "''";
			return new ProtoRule(nameof(TerminalLiteral), (grammar, rule) => grammar.TerminalValue(value.Substring(1, value.Length - 2), rule.RuleName));
		}
	}
}
