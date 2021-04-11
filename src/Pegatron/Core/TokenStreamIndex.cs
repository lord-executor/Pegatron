using System;

namespace Pegatron.Core
{
	public class TokenStreamIndex
	{
		public TokenStream Stream { get; }
		public int Index { get; }

		public TokenStreamIndex(TokenStream stream, int index)
		{
			ArgAssert.NotNegative(nameof(index), index);

			Stream = stream;
			Index = index;
		}

		public IToken Get()
		{
			return Stream.Get(Index);
		}

		public TokenStreamIndex Next()
		{
			if (Get().IsEndOfStream)
			{
				throw new IndexOutOfRangeException("Trying to read past the end of the token stream");
			}
			return new TokenStreamIndex(Stream, Index + 1);
		}

		public TokenStreamSpan Until(TokenStreamIndex end)
		{
			if (end.Index < Index)
			{
				throw new IndexOutOfRangeException();
			}

			return new TokenStreamSpan(Stream, Index, end.Index);
		}
	}
}
