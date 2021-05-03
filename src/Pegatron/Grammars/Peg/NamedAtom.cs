using Pegatron.Core;
using Pegatron.Grammars.Peg.Ast;
using System.Linq;

namespace Pegatron.Grammars.Peg
{
	public class NamedAtom : IGrammarRule<INode>
	{
		public static string DefinitionText => "namedAtom  :=  atomExpression #atom ('#!' | '#' T<Identifier>)? #name";

		public void Register(IGrammarBuilder<INode> grammar)
		{
			grammar.Sequence("namedAtom",
				grammar.Ref("atomExpression").As("atom"),
				grammar.Optional(null,
					grammar.Choice(null,
						grammar.TerminalValue("#!"),
						grammar.Sequence(null,
							grammar.TerminalValue("#"),
							grammar.Terminal(TokenType.Identifier)
						))
				).As("name")
			).ReduceWith(Reduce);
		}

		public INode Reduce(IRule rule, INodeContext<INode> page)
		{
			if (page.Count > 1)
			{
				var atom = page.Get("atom").Single<ProtoRule>();
				var name = page.Get("name").Of<Value>().Select(v => v.Text).StrJoin(string.Empty);
				atom.RefName = name.Substring(1);
				return atom;
			}
			else
			{
				return page.Get(0);
			}
		}
	}
}
