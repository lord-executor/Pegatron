using Pegatron.Core;
using System.Threading.Tasks;

namespace Pegatron
{
	public interface IDebugHooks<TNode>
	{
		Task OnBeforeCall(RuleState<TNode> state, IRuleRef rule, TokenStreamIndex idx);
		Task OnAfterCall(RuleState<TNode> state, IRuleRef rule, TokenStreamIndex idx, RuleResult res);
		Task OnToken(RuleState<TNode> state, IToken token);
	}

	public class NullDebugger<TNode> : IDebugHooks<TNode>
	{
		public Task OnAfterCall(RuleState<TNode> state, IRuleRef rule, TokenStreamIndex idx, RuleResult res)
		{
			return Task.CompletedTask;
		}

		public Task OnBeforeCall(RuleState<TNode> state, IRuleRef rule, TokenStreamIndex idx)
		{
			return Task.CompletedTask;
		}

		public Task OnToken(RuleState<TNode> state, IToken token)
		{
			return Task.CompletedTask;
		}
	}
}
