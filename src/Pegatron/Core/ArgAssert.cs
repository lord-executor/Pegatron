using System;

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
	}
}
