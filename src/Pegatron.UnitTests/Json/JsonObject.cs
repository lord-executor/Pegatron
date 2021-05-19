using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Pegatron.UnitTests.Json
{
	public class JsonObject : JsonValue, IEnumerable<JsonProperty>
	{
		private readonly IDictionary<string, JsonProperty> _properties;
		public int Count => _properties.Count;

		public JsonValue? this[string name]
		{
			get
			{
				return _properties.ContainsKey(name) ? _properties[name].Value : null;
			}
		}

		public JsonObject(IList<JsonProperty> properties)
		{
			_properties = properties.ToDictionary(p => p.Name);
		}

		public IEnumerator<JsonProperty> GetEnumerator()
		{
			return _properties.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
