using System.Collections.Generic;

namespace Pegatron.Core
{
	/// <summary>
	/// This is an implementation detail of the <see cref="Parser"/> and should not be used by any
	/// other class than the <see cref="Parser"/> and <see cref="IDebugHooks{TNode}"/>.
	/// </summary>
	public class RuleState<TNode>
	{
		public IRuleRef<TNode> Rule { get; }
		public IEnumerator<RuleOperation> Iterator { get; }
		public CoroutineResult<RuleResult> Result { get; set; }
		public NodeContext<TNode> NodeContext { get; }
		public IRuleContext RuleContext { get; }

		internal RuleState(IRuleRef rule, TokenStreamIndex index, IRuleOperations coroutines)
		{
			Rule = (IRuleRef<TNode>)rule;
			Result = new CoroutineResult<RuleResult>();
			NodeContext = new NodeContext<TNode>();
			RuleContext = new RuleContext(Rule, index, coroutines);
			Iterator = rule.Grab(RuleContext).GetEnumerator();
		}
	}
}
