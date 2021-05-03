using Pegatron.Grammars.Peg.Ast;

namespace Pegatron.Grammars.Peg
{
	public class TerminalType : IGrammarRule<INode>
	{
		public static string DefinitionText => "terminalType  :=  'T' '<' T<Identifier> #type '>'";

		public void Register(IGrammarBuilder<INode> grammar)
		{
			grammar.Sequence("terminalType",
				grammar.TerminalValue("T"),
				grammar.TerminalValue("<"),
				grammar.Terminal(TokenType.Identifier).As("type"),
				grammar.TerminalValue(">")
			).ReduceWith(Reduce);
		}

		public INode Reduce(IRule rule, INodeContext<INode> page)
		{
			var type = page.Get("type").Single<Value>().Text ?? string.Empty;
			return new ProtoRule(nameof(TerminalType), (grammar, rule) => grammar.TerminalType(type, rule.RuleName));
		}
	}
}
