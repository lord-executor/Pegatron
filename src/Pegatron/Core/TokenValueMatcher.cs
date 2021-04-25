namespace Pegatron.Core
{
	public class TokenValueMatcher : ITokenMatcher
	{
		private string _tokenValue;

		public TokenValueMatcher(string value)
		{
			_tokenValue = value;
		}

		public string Name => $"'{_tokenValue}'";

		public bool Match(IToken token) => token.Value == _tokenValue;
	}
}
