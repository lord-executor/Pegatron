using System.Collections.Generic;

namespace Pegatron
{
	public interface INodeContext<TNode>
	{
		public static string MainKey = "main";

		int Count { get; }
		bool Has(string refName);
		NodeContextValue<TNode> Get(string refName);
		TNode Get(int index);
		NodeContextValue<TNode> GetAll();
	}

	public static class NodeContextExtensions
	{
		public static NodeContextValue<TNode> Main<TNode>(this INodeContext<TNode> context)
		{
			return context.Has(INodeContext<TNode>.MainKey)
				? context.Get(INodeContext<TNode>.MainKey)
				: new NodeContextValue<TNode>(new List<TNode>() { context.Get(0) });
		}
	}
}
