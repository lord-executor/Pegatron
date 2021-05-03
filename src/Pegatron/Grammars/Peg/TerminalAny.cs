using Pegatron.Grammars.Peg.Ast;

namespace Pegatron.Grammars.Peg
{
	public class TerminalAny : IGrammarRule<INode>
	{
		public static string DefinitionText => "terminalAny  :=  '.'";

		public void Register(IGrammarBuilder<INode> grammar)
		{
			grammar.TerminalValue(".", "terminalAny")
				.ReduceWith(Reduce);
		}

		public INode Reduce(IRule rule, INodeContext<INode> page)
		{
			return new ProtoRule(nameof(Core.Rules.AnyTerminal), (grammar, rule) => grammar.Any(rule.RuleName));
		}
	}
}
