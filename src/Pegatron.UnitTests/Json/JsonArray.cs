using System.Collections;
using System.Collections.Generic;

namespace Pegatron.UnitTests.Json
{
	public class JsonArray : JsonValue, IEnumerable<JsonValue>
	{
		private readonly IList<JsonValue> _items;

		public int Count => _items.Count;

		public JsonArray(IList<JsonValue> items)
		{
			_items = items;
		}

		public IEnumerator<JsonValue> GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
