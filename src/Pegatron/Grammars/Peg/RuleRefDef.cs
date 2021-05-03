using Pegatron.Grammars.Peg.Ast;

namespace Pegatron.Grammars.Peg
{
	public class RuleRefDef : IGrammarRule<INode>
	{
		public static string DefinitionText => "ruleRef  :=  T<Identifier>";

		public void Register(IGrammarBuilder<INode> grammar)
		{
			grammar.Terminal(TokenType.Identifier, "ruleRef")
				.ReduceWith(Reduce);
		}

		public INode Reduce(IRule rule, INodeContext<INode> page)
		{
			var ruleName = ((Value)page.Get(0)).Text ?? string.Empty;
			var result = new ProtoRule("Ref", (grammar, rule) => grammar.Ref(ruleName));
			result.RuleName = ruleName;
			return result;
		}
	}
}
