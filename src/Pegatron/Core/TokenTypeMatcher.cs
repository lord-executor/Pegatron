namespace Pegatron.Core
{
	public class TokenTypeMatcher : ITokenMatcher
	{
		private string _tokenType;

		public TokenTypeMatcher(string tokenType)
		{
			_tokenType = tokenType;
		}

		public string Name => _tokenType;

		public bool Match(IToken token) => token.Type == _tokenType;
	}
}
