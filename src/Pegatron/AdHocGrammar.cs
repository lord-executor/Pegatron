using Pegatron.Core;

namespace Pegatron
{
	public class AdHocGrammar : Grammar<CstNode>
	{
		public override CstNode DefaultReducer(IRule rule, INodeContext<CstNode> page)
		{
			return new CstNode(rule.Name, page.GetAll());
		}

		public override CstNode TokenNodeFactory(IRule rule, IToken token)
		{
			ArgAssert.NotNull(nameof(token.Value), token.Value);
			return new CstNode(rule.Name, token.Value);
		}
	}
}
