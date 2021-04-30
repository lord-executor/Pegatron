using System.Collections.Generic;
using System.Linq;

namespace Pegatron.Core
{
	public static class EnumSequence
	{
		public static string StrJoin<T>(this IEnumerable<T> values, string separator)
		{
			return string.Join(separator, values.Where(v => v != null));
		}

		public static IEnumerable<T> Of<T>(params T[] items)
		{
			return items;
		}
	}
}
