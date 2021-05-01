using Pegatron.Core;
using System.Collections.Generic;

namespace Pegatron
{
	public class AdHocGrammar : Grammar<CstNode>
	{
		public override IEnumerable<CstNode> DefaultReducer(IRule rule, INodeContext<CstNode> page)
		{
			return EnumSequence.Of(new CstNode(rule.Name, page.GetAll()));
		}

		public override IEnumerable<CstNode> TerminalReducer(IRule rule, IToken token)
		{
			ArgAssert.NotNull(nameof(token.Value), token.Value);
			return EnumSequence.Of(new CstNode(rule.Name, token.Value));
		}
	}
}
