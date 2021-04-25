using System;

namespace Pegatron.Core
{
	public class TokenPredicateMatcher : ITokenMatcher
	{
		private Predicate<IToken> _predicate;

		public string Name { get; }

		public TokenPredicateMatcher(Predicate<IToken> predicate)
			: this("?", predicate)
		{
		}

		public TokenPredicateMatcher(string name, Predicate<IToken> predicate)
		{
			Name = name;
			_predicate = predicate;
		}

		public bool Match(IToken token) => _predicate(token);
	}
}
