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

			var result = new ProtoRule(nameof(Core.Rules.Sequence), rule => new Core.Rules.Sequence(rule.RuleName, rule.All()));
			result.AddChildren(page.GetAll().Of<ProtoRule>());

			return result;
		}
	}
}
