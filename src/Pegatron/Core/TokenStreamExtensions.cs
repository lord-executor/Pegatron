using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pegatron.Core
{
	public static class TokenStreamExtensions
	{
		public static TokenStreamIndex Start(this TokenStream stream)
		{
			return new TokenStreamIndex(stream, 0);
		}
	}
}
