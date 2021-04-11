namespace Pegatron
{
	public delegate TNode Reducer<TNode>(IRule rule, INodeContext<TNode> page);
}
