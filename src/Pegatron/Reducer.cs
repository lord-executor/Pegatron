using System.Collections.Generic;

namespace Pegatron
{
	public delegate IEnumerable<TNode> Reducer<TNode>(IRule rule, INodeContext<TNode> page);
	public delegate TNode SingleReducer<TNode>(IRule rule, INodeContext<TNode> page);
}
