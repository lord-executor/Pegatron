using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Pegatron
{
	public class NodeContextValue<TNode> : IEnumerable<TNode>
	{
		private readonly IList<TNode> _items;

		public NodeContextValue(IList<TNode> items)
		{
			_items = items;
		}

		public TNode Single()
		{
			return _items.Single();
		}

		public T Single<T>()
			where T : TNode
		{
			return (T)Single()!;
		}

		public TNode? Optional()
		{
			return _items.FirstOrDefault();
		}

		public T? Optional<T>()
			where T : TNode
		{
			return (T?)Optional();
		}

		public IEnumerable<T> Of<T>()
			where T : TNode
		{
			return _items.OfType<T>();
		}

		public IEnumerable<T> As<T>()
			where T : TNode
		{
			return _items.Cast<T>();
		}

		public IEnumerator<TNode> GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		[ExcludeFromCodeCoverage]
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
