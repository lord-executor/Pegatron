using System;
using System.Diagnostics.CodeAnalysis;

namespace Pegatron.Core
{
	public static class ArgAssert
	{
		public static void NotNegative(string name, int arg)
		{
			if (arg < 0)
			{
				throw new ArgumentOutOfRangeException(name, $"'{name}' cannot be negative");
			}
		}

		public static void NotNull(string name, [NotNull] object? value)
		{
			if (value == null)
			{
				throw new ArgumentException(name, $"'{name}' cannot be null");
			}
		}
	}
}
