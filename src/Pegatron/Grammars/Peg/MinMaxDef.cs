using Pegatron.Grammars.Peg.Ast;

namespace Pegatron.Grammars.Peg
{
	public class MinMaxDef : IGrammarRule<INode>
	{
		public static string DefinitionText => "minmax  :=  '{' T<Number> #min ','? #sep T<Number>? #max '}'";

		public void Register(IGrammarBuilder<INode> grammar)
		{
			grammar.Sequence("minmax",
				grammar.TerminalValue("{"),
				grammar.Terminal(TokenType.Number).As("min"),
				grammar.Optional(null, grammar.TerminalValue(",")).As("sep"),
				grammar.Optional(null, grammar.Terminal(TokenType.Number)).As("max"),
				grammar.TerminalValue("}")
			).ReduceWith(Reduce);
		}

		public INode Reduce(IRule rule, INodeContext<INode> page)
		{
			var hasSeparator = page.Get("sep").Optional() != null;
			var min = int.Parse(page.Get("min").Single<Value>().Text ?? "0");
			var max = int.Parse(page.Get("max").Optional<Value>()?.Text ?? "-1");
			if (!hasSeparator)
			{
				max = min;
			}
			return new Ast.Range(min, max);
		}
	}
}
