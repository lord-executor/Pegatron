using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Pegatron.Core
{
	public class TokenStreamSpan : IEnumerable<IToken>
	{
		public static TokenStreamSpan Empty { get; } = new TokenStreamSpan();

		private readonly TokenStream? _stream;
		public int Start { get; }
		public int End { get; }
		public int Count => End - Start;

		public TokenStreamSpan(TokenStream stream, int start, int end)
		{
			_stream = stream;
			Start = start;
			End = end;
		}

		private TokenStreamSpan()
		{
		}

		public IEnumerator<IToken> GetEnumerator()
		{
			if (_stream == null || Count == 0)
			{
				return Enumerable.Empty<IToken>().GetEnumerator();
			}
			else
			{
				return Enumerable.Range(Start, End - Start).Select(i => _stream.Get(i)).GetEnumerator();
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
