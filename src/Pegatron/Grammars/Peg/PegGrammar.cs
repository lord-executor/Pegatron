using Pegatron.Core;
using Pegatron.Grammars.Peg.Ast;
using System.Collections.Generic;

namespace Pegatron.Grammars.Peg
{
	public class PegGrammar : Grammar<INode>
	{
		public PegGrammar()
		{
			new Definition().Register(this);
			new Choice().Register(this);
			new Sequence().Register(this);
			new NamedAtom().Register(this);
			new AtomExpression().Register(this);
			new RangeDef().Register(this);
			new MinMaxDef().Register(this);
			new Atom().Register(this);
			new RuleRefDef().Register(this);
			new Terminal().Register(this);
			new TerminalType().Register(this);
			new TerminalLiteral().Register(this);
			new TerminalAny().Register(this);

			StartWith("definition");
		}

		public override IEnumerable<INode> TerminalReducer(IRule rule, IToken token)
		{
			ArgAssert.NotNull(nameof(token.Value), token.Value);
			yield return new Value(token.Value);
		}

		public override IEnumerable<INode> DefaultReducer(IRule rule, INodeContext<INode> page)
		{
			if (page.HasLift())
			{
				return page.GetLift();
			}

			return page.GetAll();
		}
	}
}
