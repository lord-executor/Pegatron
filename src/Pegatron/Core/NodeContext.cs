using System.Collections.Generic;
using System.Linq;

namespace Pegatron.Core
{
	public class NodeContext<TNode> : INodeContext<TNode>
	{
		private readonly IList<(string? refName, TNode node)> _children = new List<(string? refName, TNode node)>();

		public int Count => _children.Count;

		public bool Has(string refName)
		{
			return _children.Where(item => item.refName == refName).Count() == 1;
		}

		public TNode Get(string refName)
		{
			return _children.Single(item => item.refName == refName).node;
		}

		public TNode? MaybeGet(string refName)
		{
			return _children.SingleOrDefault(item => item.refName == refName).node;
		}

		public TNode Get(int index)
		{
			return _children[index].node;
		}

		public TNode? MaybeGet(int index)
		{
			return _children.Count > index ? _children[index].node : default(TNode);
		}

		public IEnumerable<TNode> GetAll()
		{
			return _children.Select(item => item.node);
		}

		public TNode Add(string? refName, TNode node)
		{
			_children.Add((refName, node));
			return node;
		}
	}
}
