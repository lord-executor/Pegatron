using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Pegatron.Core
{
	[DebuggerDisplay("Count = {Count}")]
	public class NodeContext<TNode> : INodeContext<TNode>
	{
		private readonly IList<(string? refName, TNode node)> _children = new List<(string? refName, TNode node)>();

		public int Count => _children.Count;

		public bool Has(string refName)
		{
			return _children.Any(item => item.refName == refName);
		}

		public NodeContextValue<TNode> Get(string refName)
		{
			return new NodeContextValue<TNode>(_children.Where(item => item.refName == refName).Select(item => item.node).ToList());
		}

		public TNode Get(int index)
		{
			return _children[index].node;
		}

		public NodeContextValue<TNode> GetAll()
		{
			return new NodeContextValue<TNode>(_children.Select(item => item.node).ToList());
		}

		public void Add(string? refName, TNode node)
		{
			_children.Add((refName, node));
		}

		public void Add(string? refName, IEnumerable<TNode> nodes)
		{
			foreach (var n in nodes)
			{
				Add(refName, n);
			}
		}
	}
}
