using System.Collections.Generic;

namespace Pegatron
{
	public interface INodeContext<TNode>
	{
		int Count { get; }
		bool Has(string refName);
		TNode Get(string refName);
		TNode Get(int index);
		TNode? MaybeGet(string refName);
		TNode? MaybeGet(int index);
		IEnumerable<TNode> GetAll();
	}
}
