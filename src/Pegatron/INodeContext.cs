using System.Collections.Generic;

namespace Pegatron
{
	public interface INodeContext<TNode>
	{
		int Count { get; }
		bool Has(string name);
		TNode Get(string name);
		TNode Get(int index);
		TNode MaybeGet(string name);
		TNode MaybeGet(int index);
		IEnumerable<TNode> GetAll();
	}
}
