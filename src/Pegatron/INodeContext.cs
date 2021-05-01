namespace Pegatron
{
	public interface INodeContext<TNode>
	{
		int Count { get; }
		bool Has(string refName);
		NodeContextValue<TNode> Get(string refName);
		TNode Get(int index);
		NodeContextValue<TNode> GetAll();
	}

	public static class NodeContextExtensions
	{
		public static bool HasLift<TNode>(this INodeContext<TNode> context)
		{
			return context.Has(IRuleRef.LiftRefName);
		}

		public static NodeContextValue<TNode> GetLift<TNode>(this INodeContext<TNode> context)
		{
			return context.Get(IRuleRef.LiftRefName);
		}
	}
}
