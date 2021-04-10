using System.Collections.Generic;

namespace Pegatron
{
	public interface ILexer<out TToken>
		where TToken : IToken
	{
		/// <summary>
		/// The last token that is returned before the enumerable sequence is terminated has to be an
		/// end-of-stream token (i.e. <see cref="IToken.IsEndOfStream"/> has to return <c>true</c>).
		/// </summary>
		/// <returns>A sequence of tokens ending with an EOS token</returns>
		IEnumerable<TToken> ReadTokens();
	}
}
