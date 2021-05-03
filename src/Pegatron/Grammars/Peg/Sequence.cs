using Pegatron.Grammars.Peg.Ast;

namespace Pegatron.Grammars.Peg
{
	public class Sequence : IGrammarRule<INode>
	{
		public static string DefinitionText => "sequence  :=  namedAtom+";

		public void Register(IGrammarBuilder<INode> grammar)
		{
			grammar.OneOrMore("sequence",
				grammar.Ref("namedAtom")
			).ReduceWith(Reduce);
		}

		public INode Reduce(IRule rule, INodeContext<INode> page)
		{
			// sequence with a single item => no sequence needed
			if (page.Count == 1)
			{
				return page.Get(0);
			}

			var result = new ProtoRule(nameof(Core.Rules.Sequence), (grammar, rule) => grammar.Sequence(rule.RuleName, rule.All(grammar)));
			result.AddChildren(page.GetAll().Of<ProtoRule>());

			return result;
		}
	}
}
