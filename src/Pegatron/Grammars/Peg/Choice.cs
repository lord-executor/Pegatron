using Pegatron.Grammars.Peg.Ast;

namespace Pegatron.Grammars.Peg
{
	public class Choice : IGrammarRule<INode>
	{
		public static string DefinitionText => "choice  :=  sequence ('|' sequence #!)*";

		public void Register(IGrammarBuilder<INode> grammar)
		{
			grammar.Sequence("choice",
				grammar.Ref("sequence"),
				grammar.ZeroOrMore(null, grammar.Sequence(null,
					grammar.TerminalValue("|"),
					grammar.Ref("sequence").Lift()
				))
			).ReduceWith(Reduce);
		}

		public INode Reduce(IRule rule, INodeContext<INode> page)
		{
			// choice with a single item => no choice needed
			if (page.Count == 1)
			{
				return page.Get(0);
			}

			var result = new ProtoRule(nameof(Core.Rules.Choice), rule => new Core.Rules.Choice(rule.RuleName, rule.All()));
			// filter out the "|" values
			result.AddChildren(page.GetAll().Of<ProtoRule>());

			return result;
		}
	}
}
